using System;

namespace FcmSharpClient
{
    public class FcmConfiguration
    {
        public FcmConfiguration(string apiKey)
        {
            if(string.IsNullOrWhiteSpace(apiKey))
            {
                throw new ArgumentException("An FCM API-Key is required. Check https://console.firebase.google.com/project/(your-project-id)/settings/cloudmessaging for your messaging key", nameof(apiKey));
            }
            ApiKey = apiKey;
        }
        public string ApiKey { get; }
    }
}
