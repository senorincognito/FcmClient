using System.Runtime.Serialization;

namespace FcmClient.Contracts
{
    [DataContract]
    public class FcmRequestNotification
    {

        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "body")]
        public string Body { get; set; }
    }
}
