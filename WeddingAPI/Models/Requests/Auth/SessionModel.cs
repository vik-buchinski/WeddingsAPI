using System;
using Newtonsoft.Json;

namespace WeddingAPI.Models.Requests.Auth
{
    public class SessionModel
    {
        [JsonProperty(PropertyName = "token")]
        public String Token { get; set; }
    }
}