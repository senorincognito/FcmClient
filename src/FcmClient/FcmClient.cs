using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using FcmClient.Contracts;
using FcmClient.Exceptions;
using Newtonsoft.Json;

namespace FcmClient
{
    public class FcmClient
    {
        private const string FcmApiUrl = "https://fcm.googleapis.com/fcm/send";
        
        private readonly FcmConfiguration _fcmConfiguration;
        private readonly HttpClient _httpClient;

        public FcmClient(FcmConfiguration fcmConfiguration,
            HttpClient httpClient)
        {
            _fcmConfiguration = fcmConfiguration ?? throw new ArgumentException("An FCM configuratino object is required", nameof(fcmConfiguration));
            _httpClient = httpClient ?? throw new ArgumentException("An HTTPClient is required", nameof(httpClient));
        }

        public async Task<FcmSendResult> Send (IEnumerable<string> tokens, string title, string body, object data)
        {
            var tokensArray = tokens?.ToArray() ?? new string[0];
            if (!tokensArray.Any())
            {
                return new FcmSendResult();
            }
            if (string.IsNullOrWhiteSpace(title))
            {
                throw new ArgumentException("title is null or empty", nameof(title));
            }
            if (string.IsNullOrWhiteSpace(body))
            {
                throw new ArgumentException("body is null or empty", nameof(body));
            }

            var fcmRequest = new FcmRequest
            {
                Notification = new FcmRequestNotification
                {
                    Title = title,
                    Body = body
                },
                RegistrationIds = tokensArray
            };
            if (data != null)
            {
                fcmRequest.Data = data;
            }

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("key", "=" + _fcmConfiguration.ApiKey);

            using (var httpResponse = await _httpClient.PostAsync(FcmApiUrl,
                new StringContent(JsonConvert.SerializeObject(fcmRequest),
                    Encoding.UTF8,
                    "application/json")))
            {
                var stringResponse = await httpResponse.Content.ReadAsStringAsync();
                if (!httpResponse.IsSuccessStatusCode)
                {
                    throw new FcmSendException(stringResponse, fcmRequest);
                }
                var response = JsonConvert.DeserializeObject<FcmResponse>(stringResponse);
                if (response.Results == null)
                {
                    throw new FcmSendException($"FcmRespose was invalid {stringResponse}", fcmRequest);
                }

                var failedTokens = FindFailedTokens(response, tokensArray).ToArray();
                var successfulTokens = tokensArray.Where(x => !failedTokens.Contains(x)).ToArray();
                return new FcmSendResult(successfulTokens, failedTokens);
            }
        }

        private IEnumerable<string> FindFailedTokens(FcmResponse response, string[] tokensArray)
        {
            for (var i = 0; i < response.Results.Count(); i++)
            {
                if (!string.IsNullOrWhiteSpace(response.Results.ElementAt(i).Error))
                {
                    yield return tokensArray[i];
                }
            }
        }
    }
}
