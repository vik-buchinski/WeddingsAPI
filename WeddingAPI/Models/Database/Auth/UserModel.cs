using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeddingAPI.Models.Database.Auth
{
    public class UserModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        public String Email { get; set; }

        [Required]
        public String PasswordHash { get; set; }

        [Required]
        public String SaltValue { get; set; }
    }
}