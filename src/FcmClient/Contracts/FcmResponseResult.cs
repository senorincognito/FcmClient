using System.Runtime.Serialization;

namespace FcmClient.Contracts
{
    [DataContract]
    public class FcmResponseResult
    {
        [DataMember(Name = "message_id")]
        public string MessageId { get; set; }

        [DataMember(Name = "error")]
        public string Error { get; set; }
    }
}
