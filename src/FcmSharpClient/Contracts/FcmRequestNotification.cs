﻿using Newtonsoft.Json;

namespace FcmSharpClient.Contracts
{
    public class FcmRequestNotification
    {

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("body")]
        public string Body { get; set; }
    }
}
