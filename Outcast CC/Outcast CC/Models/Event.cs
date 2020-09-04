namespace Outcast_CC.Models
{
  using System;
  using System.Collections.Generic;
  using System.ComponentModel.DataAnnotations;
  using System.ComponentModel.DataAnnotations.Schema;
  using System.Data.Entity.Spatial;

  public partial class Event
  {
    [Key]
    public int EventId { get; set; }

    [Required]
    [StringLength(100)]
    [Display(Name = "Title")]
    public string Title { get; set; }

    [Required]
    [StringLength(100)]
    [Display(Name = "Location")]
    public string Location { get; set; }

    [Column(TypeName = "date")]
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    [Display(Name = "Event Date")]
    public DateTime EventDate { get; set; }

    [Required]
    [StringLength(4000)]
    [Display(Name = "Event Description")]
    public string Text { get; set; }

    public int? Going { get; set; }

    [StringLength(100)]
    public string PdfFileType { get; set; }

    [StringLength(100)]
    public string PdfFileName { get; set; }

    public List<string> UsersGoing { get; set; }
  }
}
