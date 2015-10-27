using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace WeddingAPI.Models.Requests.Admin.Common
{
    public class RequestAlbumModel
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "album_name")]
        public String AlbumName { get; set; }

        [JsonProperty(PropertyName = "album_description")]
        public String AlbumDescription { get; set; }
        
        [JsonProperty(PropertyName = "is_expaned")]
        public Boolean IsExpanded { get; set; }

        [JsonProperty(PropertyName = "is_visible")]
        public Boolean IsVisible { get; set; }

        [JsonProperty(PropertyName = "album_main_image")]
        public String MainImage { get; set; }

        [JsonProperty(PropertyName = "album_type")]
        public String AlbumType { get; set; }

        [JsonProperty(PropertyName = "images")]
        public List<RequestImageModel> Images { get; set; }
    }
}