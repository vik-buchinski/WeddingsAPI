using System;
using Newtonsoft.Json;

namespace WeddingAPI.Models.Requests.Admin.About
{
    public class ViewAdminAboutModel
    {
        [JsonProperty(PropertyName = "title_image_url")]
        public String TitleImageUrl { get; set; }

        [JsonProperty(PropertyName = "image_url")]
        public String ImageUrl { get; set; }

        [JsonProperty(PropertyName = "description")]
        public String Description { get; set; }
    }
}