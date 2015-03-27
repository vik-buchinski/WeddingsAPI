using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace WeddingAPI.Models.Auth
{
    public class UserModel
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Display(ShortName = "id")]
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        [Required]
        [JsonProperty(PropertyName = "email")]
        [EmailAddress]
        public String Email { get; set; }

        [Required]
        [JsonProperty(PropertyName = "password_hash")]
        public String PasswordHash { get; set; }
    }
}