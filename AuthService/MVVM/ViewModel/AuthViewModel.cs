using AuthService.Commands;
using AuthService.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Text.RegularExpressions;

namespace AuthService.MVVM.ViewModel
{
    public class AuthViewModel : ViewModelBase
    {
        private string _code;
        private string _serverResponse;
        private DateTime _endDate;
        private string _status;
        private Brush _statusForeground;
        private bool _isLoading;
        private bool _isTrial;
        private string _closeButtonText;
        private readonly SubscriptionService _subscriptionService;

        public string Code
        {
            get => _code;
            set
            {
                if (SetProperty(ref _code, value))
                {
                    ((RelayCommand)SubmitCommand).RaiseCanExecuteChanged();
                }
            }
        }

        public string ServerResponse
        {
            get => _serverResponse;
            set => SetProperty(ref _serverResponse, value);
        }

        public DateTime EndDate
        {
            get => _endDate;
            set => SetProperty(ref _endDate, value);
        }

        public string Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }

        public Brush StatusForeground
        {
            get => _statusForeground;
            set => SetProperty(ref _statusForeground, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public string CloseButtonText
        {
            get => _closeButtonText;
            set => SetProperty(ref _closeButtonText, value);
        }

        public ICommand SubmitCommand { get; }
        public ICommand CloseCommand { get; }

        public AuthViewModel()
        {
            _subscriptionService = new SubscriptionService();
            SubmitCommand = new RelayCommand(ExecuteSubmit, CanExecuteSubmit);
            CloseCommand = new RelayCommand(ExecuteClose, CanExecuteClose);
            StatusForeground = Brushes.Black;
            CloseButtonText = "Close";
        }

        private async void ExecuteSubmit(object parameter)
        {
            if (string.IsNullOrWhiteSpace(Code))
                return;

            IsLoading = true;
            ServerResponse = "Processing...";

            try
            {


                bool exist = false;
                bool validFromat = true;
                ApiResponseGet response = await _subscriptionService.GetRedemptionsAsync(Code);

                // Check if response is null (timeout or connection error)
                if (response == null)
                {
                    ServerResponse = string.Empty;
                    SubscriptionService.Status = SubscriptionService.SubscriptionStatus.Error;
                    Status = "Failed to connect";
                    StatusForeground = Brushes.Red;
                    CloseButtonText = "Close";
                    ((RelayCommand)CloseCommand).RaiseCanExecuteChanged();
                    return;
                }
                if (!response.Success)
                {
                    ServerResponse = string.Empty;
                    SubscriptionService.Status = SubscriptionService.SubscriptionStatus.NotFound;
                    Status = "Code not found";
                    StatusForeground = Brushes.Red;
                    CloseButtonText = "Close";
                    ((RelayCommand)CloseCommand).RaiseCanExecuteChanged();
                    return;
                }
                // check if "data" field is populated and in the correct format
                var redemptions = response.Payload.Redemptions.FirstOrDefault();
                bool dataExists = redemptions != null && redemptions.Data != null && redemptions.Data.Any();
                if (dataExists)
                {
                    string dataStr = redemptions.Data.First();
                    if (!string.IsNullOrEmpty(dataStr))
                    {
                        var parts = dataStr.Split(',');
                        var validMacs = new List<string>();

                        // Regex to validate MAC address (XX:XX:XX:XX:XX:XX)
                        var macRegex = new Regex(@"^([0-9A-Fa-f]{2}[:]){5}([0-9A-Fa-f]{2})$");

                        foreach (var part in parts)
                        {
                            var trimmed = part.Trim();
                            if (!macRegex.IsMatch(trimmed))
                            {
                                validFromat = false;
                                break;
                            }
                            validMacs.Add(trimmed);
                        }

                        if (validFromat)
                        {
                            var currentMac = SubscriptionService.GetCurrentMacAddress();
                            exist = validMacs.IndexOf(currentMac) > 0 ;
                        }
                    }
                }
                if (!dataExists || !validFromat)
                {
                    var macs = SubscriptionService.GetAllMacAdressesFromPc();
                    var allMacs = string.Join(",", macs);
                    var postResponse = _subscriptionService.PostRedeemAsync(Code, allMacs);
                    exist = postResponse != null && postResponse.Result.Success;
                }

                if (!exist)
                {
                    ServerResponse = string.Empty;
                    SubscriptionService.Status = SubscriptionService.SubscriptionStatus.Error;
                    Status = "Device not authorized";
                    StatusForeground = Brushes.Red;
                    CloseButtonText = "Close";
                    ((RelayCommand)CloseCommand).RaiseCanExecuteChanged();
                    return;
                }

                ServerResponse = string.Empty; // Clear processing message
                
                if (response.Success && response.Payload?.Subscription != null)
                {
                    var subscription = response.Payload.Subscription;
                    EndDate = subscription.EndDate;
                    _isTrial = subscription.IsTrial;
                    
                    DateTime currentDate = await DateHelper.GetCurrentDateFromWebAsync();
                    bool isExpired = EndDate < currentDate;
                    
                    if (isExpired)
                    {
                        SubscriptionService.Status = SubscriptionService.SubscriptionStatus.Expired;
                        StatusForeground = Brushes.Red;
                        CloseButtonText = "Close";
                    }
                    else
                    {
                        SubscriptionService.Status = SubscriptionService.SubscriptionStatus.Valid;
                        StatusForeground = Brushes.Green;
                        CloseButtonText = "Continue";
                    }
                    
                    string trialText = _isTrial ? "Trial" : "Subscriped";
                    string validityText = isExpired ? "Expired" : "Valid";
                    string endDateText = EndDate.ToString("yyyy/MM/dd");
                    
                    Status = $"{trialText} - {validityText} - {endDateText}";
                }
                else
                {
                    SubscriptionService.Status = SubscriptionService.SubscriptionStatus.NotFound;
                    Status = string.Empty;
                    StatusForeground = Brushes.Black;
                    CloseButtonText = "Close";
                }
                
                ((RelayCommand)CloseCommand).RaiseCanExecuteChanged();
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void ExecuteClose(object parameter)
        {
            if (parameter is System.Windows.Window window)
            {
                window.Close();
            }
        }

        private bool CanExecuteSubmit(object parameter)
        {
            return !string.IsNullOrWhiteSpace(Code) && !IsLoading;
        }

        private bool CanExecuteClose(object parameter)
        {
            return true; // Always enabled
        }
    }
}
