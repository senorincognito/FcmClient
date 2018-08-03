using System.Runtime.Serialization;

namespace FcmClient.Contracts
{
    [DataContract]
    public class FcmRequest
    {
        [DataMember(Name = "notification")]
        public FcmRequestNotification Notification { get; set; }

        [DataMember(Name = "registration_ids")]
        public string[] RegistrationIds { get; set; }

        [DataMember(Name = "data")]
        public object Data { get; set; }
    }
}
