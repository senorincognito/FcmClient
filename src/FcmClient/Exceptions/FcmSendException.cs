using System;
using FcmClient.Contracts;
using Newtonsoft.Json;

namespace FcmClient.Exceptions
{
    public class FcmSendException : Exception
    {
        public FcmSendException(string message, FcmRequest request) 
            : base($"FcmMessenger failed with message {message} for {JsonConvert.SerializeObject(request)}")
        {
            
        }
    }
}
