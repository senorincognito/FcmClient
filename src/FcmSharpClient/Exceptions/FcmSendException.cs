using System;
using FcmSharpClient.Contracts;
using Newtonsoft.Json;

namespace FcmSharpClient.Exceptions
{
    public class FcmSendException : Exception
    {
        public FcmSendException(string message, FcmRequest request) 
            : base($"FcmMessenger failed with message {message} for {JsonConvert.SerializeObject(request)}")
        {
            
        }
    }
}
