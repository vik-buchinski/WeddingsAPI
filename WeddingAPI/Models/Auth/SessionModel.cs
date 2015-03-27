using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace WeddingAPI.Models.Auth
{
    public class SessionModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        [Required]
        [JsonProperty(PropertyName = "user_id")]
        public int UserId { get; set; }

        [Required]
        [JsonProperty(PropertyName = "token")]
        public String Token { get; set; }

        [Required]
        [JsonProperty(PropertyName = "is_active")]
        public Boolean IsActive { get; set; }
    }
}