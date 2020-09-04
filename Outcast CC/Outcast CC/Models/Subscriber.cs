using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Outcast_CC.Models
{
  [Table("Subscribers")]
  public partial class Subscriber
  {
    [Key]
    [Display(Name = "Subscriber ID")]
    public int Id { get; set; }

    [Display(Name = "Full Name")]
    [Required(ErrorMessage = "Please enter your full name.")]
    [StringLength(200, ErrorMessage = "Name is too long.")]
    [MinLength(3, ErrorMessage = "Name is too short.")]
    public string FullName { get; set; }

    [Display(Name = "Email Address")]
    [Required(ErrorMessage = "Please enter your email address.")]
    [StringLength(200, ErrorMessage = "Email is too long.")]
    [MinLength(3, ErrorMessage = "Email is too short.")]
    [RegularExpression(@"^(.+)@(.+\.\w+)$", ErrorMessage = "Email address is invalid. Please check that you have entered it correctly.")]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; }

    [Display(Name = "Email Confirmed")]
    public bool IsConfirmed { get; set; }

    [Display(Name = "Subscribed to Newsletter")]
    public bool IsSubscribed { get; set; }

    [Display(Name = "URL to Unsubscribe")]
    public string UnsubscribeUrl { get; set; }
  }
}