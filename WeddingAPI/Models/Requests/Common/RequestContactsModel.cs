using System;
using Newtonsoft.Json;

namespace WeddingAPI.Models.Requests.Common
{
    public class RequestContactsModel
    {
        [JsonProperty(PropertyName = "description", NullValueHandling = NullValueHandling.Ignore)]
        public String Description { get; set; }
    }
}