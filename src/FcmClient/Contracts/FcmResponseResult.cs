using Newtonsoft.Json;

namespace FcmClient.Contracts
{
    public class FcmResponseResult
    {
        [JsonProperty("message_id")]
        public string MessageId { get; set; }

        [JsonProperty("error")]
        public string Error { get; set; }
    }
}
