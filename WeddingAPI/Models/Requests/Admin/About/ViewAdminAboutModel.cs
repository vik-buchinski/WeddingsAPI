using System;
using Newtonsoft.Json;

namespace WeddingAPI.Models.Requests.Admin.About
{
    public class ViewAdminAboutModel
    {
        [JsonProperty(PropertyName = "image_url")]
        public String ImageUrl { get; set; }

        [JsonProperty(PropertyName = "description")]
        public String Description { get; set; }
    }
}