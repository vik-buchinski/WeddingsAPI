using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeddingAPI.Models.Database.Admin.About
{
    public class AdminAboutModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int? ImageModelId { get; set; }

        public String Description { get; set; }
    }
}