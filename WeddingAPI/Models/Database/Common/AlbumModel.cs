using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeddingAPI.Models.Database.Common
{
    public class AlbumModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public String AlbumName { get; set; }

        public String AlbumDescription { get; set; }

        public String AlbumType { get; set; }

        public Boolean IsExpanded { get; set; }

        public int? ImageId { get; set; }

        public Boolean IsVisible { get; set; }
    }
}