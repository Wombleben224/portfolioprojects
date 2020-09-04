namespace Outcast_CC.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Blog
    {
        [Key]
        public int BlogPostId { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        public DateTime Posted { get; set; }

        [Required]
        [StringLength(4000)]
        public string Text { get; set; }

        public int? Likes { get; set; }

        [StringLength(100)]
        public string PdfFileType { get; set; }

        [StringLength(100)]
        public string PdfFileName { get; set; }
    }
}
