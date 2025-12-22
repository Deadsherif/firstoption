using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace AuthService.Services
{
    public class SubscriptionService
    {
        public static SubscriptionStatus Status { get; set; } = SubscriptionStatus.Error;

        private readonly HttpClient _httpClient;
        private const string RedemptionsUrl = "https://marketplace.firstoption-es.com/api/subscriptions/redemptions";

        public SubscriptionService()
        {
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(30);
            // Add User-Agent header - some APIs require this
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
            // Add Accept header for JSON
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        }

        public async Task<ApiResponseGet> GetRedemptionsAsync(string code)
        {
            try
            {
                string encodedCode = Uri.EscapeDataString(code);
                string url = $"{RedemptionsUrl}?code={encodedCode}";
                HttpResponseMessage response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode(); // Throws exception if status code is not 2xx
                string responseContent = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                return JsonSerializer.Deserialize<ApiResponseGet>(responseContent, options);
            }
            catch (TaskCanceledException)
            {
                // Timeout occurred
                return null;
            }
            catch (System.TimeoutException)
            {
                // Timeout occurred
                return null;
            }
            catch (HttpRequestException)
            {
                // Network or HTTP error
                return null;
            }
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }

        public enum SubscriptionStatus
        {
            Valid,
            Expired,
            NotFound,
            Error
        }
    }
}
