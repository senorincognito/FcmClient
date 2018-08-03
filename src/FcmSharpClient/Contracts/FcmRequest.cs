using Newtonsoft.Json;

namespace FcmSharpClient.Contracts
{
    public class FcmRequest
    {
        [JsonProperty("notification")]
        public FcmRequestNotification Notification { get; set; }

        [JsonProperty("registration_ids")]
        public string[] RegistrationIds { get; set; }

        [JsonProperty("data")]
        public object Data { get; set; }
    }
}
