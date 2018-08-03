using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FcmClient.Contracts
{
    [DataContract]
    public class FcmResponse
    {
        [DataMember(Name = "multicast_id")]
        public string MulticastId { get; set; }

        [DataMember(Name = "success")]
        public int Success { get; set; }

        [DataMember(Name = "failure")]
        public int Failure { get; set; }

        [DataMember(Name = "canonical_ids")]
        public int CanonicalIds { get; set; }

        [DataMember(Name = "results")]
        public IEnumerable<FcmResponseResult> Results { get; set; }
    }
}
