using System.Collections.Generic;
using Newtonsoft.Json;

namespace FcmClient.Contracts
{
    public class FcmResponse
    {
        [JsonProperty("multicast_id")]
        public string MulticastId { get; set; }

        [JsonProperty("success")]
        public int Success { get; set; }

        [JsonProperty("failure")]
        public int Failure { get; set; }

        [JsonProperty("canonical_ids")]
        public int CanonicalIds { get; set; }

        [JsonProperty("results")]
        public IEnumerable<FcmResponseResult> Results { get; set; }
    }
}
