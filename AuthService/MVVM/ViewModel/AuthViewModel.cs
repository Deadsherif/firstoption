using System;
using System.Windows.Input;
using System.Windows.Media;
using AuthService.Commands;
using AuthService.Services;

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

        public ICommand SubmitCommand { get; }
        public ICommand CloseCommand { get; }

        public AuthViewModel()
        {
            _subscriptionService = new SubscriptionService();
            SubmitCommand = new RelayCommand(ExecuteSubmit, CanExecuteSubmit);
            CloseCommand = new RelayCommand(ExecuteClose, CanExecuteClose);
            StatusForeground = Brushes.Black;
        }

        private async void ExecuteSubmit(object parameter)
        {
            if (string.IsNullOrWhiteSpace(Code))
                return;

            IsLoading = true;
            ServerResponse = "Processing...";

            try
            {
                ApiResponseGet response = await _subscriptionService.GetRedemptionsAsync(Code);
                
                // Check if response is null (timeout or connection error)
                if (response == null)
                {
                    ServerResponse = string.Empty;
                    SubscriptionService.Status = SubscriptionService.SubscriptionStatus.Error;
                    Status = "Failed to connect";
                    StatusForeground = Brushes.Red;
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
                    }
                    else
                    {
                        SubscriptionService.Status = SubscriptionService.SubscriptionStatus.Valid;
                        StatusForeground = Brushes.Green;
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
            return SubscriptionService.Status == SubscriptionService.SubscriptionStatus.Valid;
        }
    }
}
