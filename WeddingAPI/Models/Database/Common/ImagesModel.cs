using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeddingAPI.Models.Database.Common
{
    public class ImagesModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public String LocalFileName { get; set; }

        [Required]
        public String MimeType { get; set; }

        public String Description { get; set; }

        public int? AlbumId { get; set; }

        public int? Width { get; set; }

        public int? Height { get; set; }
    }
}