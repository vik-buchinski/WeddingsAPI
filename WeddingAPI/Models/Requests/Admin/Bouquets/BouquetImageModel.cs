using System;
using Newtonsoft.Json;

namespace WeddingAPI.Models.Requests.Admin.Bouquets
{
    public class BouquetImageModel
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "image_url")]
        public String ImageUrl { get; set; }

        [JsonProperty(PropertyName = "description")]
        public String Description { get; set; }
    }
}