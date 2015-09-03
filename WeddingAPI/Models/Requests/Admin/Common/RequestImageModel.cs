using System;
using Newtonsoft.Json;

namespace WeddingAPI.Models.Requests.Admin.Common
{
    public class RequestImageModel
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "image_url")]
        public String ImageUrl { get; set; }

        [JsonProperty(PropertyName = "description", NullValueHandling = NullValueHandling.Ignore)]
        public String Description { get; set; }

        [JsonProperty(PropertyName = "image_width", NullValueHandling = NullValueHandling.Ignore)]
        public int? Width { get; set; }

        [JsonProperty(PropertyName = "image_height", NullValueHandling = NullValueHandling.Ignore)]
        public int? Height { get; set; }
    }
}