using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Outcast_CC.Models
{
  [Table("Messages")]
  public class Message
  {
    [Key]
    public Guid Id { get; set; }

    [Required]
    [StringLength(20)]
    public string User { get; set; }
    [Required]
    [StringLength(140)]
    public string Text { get; set; }
    [Required]
    [DataType(DataType.DateTime)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}", ApplyFormatInEditMode = true)]
    public DateTime Sent { get; set; }
  }
}