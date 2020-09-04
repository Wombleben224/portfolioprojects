using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Outcast_CC.Models
{
  public class AppUser : IdentityUser
  {
    public string MemberId { get; set; }

    [Display(Name = "Email Confirmed")]
    public bool IsConfirmed { get; set; }

    [Display(Name = "Subscribed to Newsletter")]
    public bool IsSubscribed { get; set; }

    [Display(Name = "URL to Unsubscribe")]
    public string UnsubscribeUrl { get; set; }
  }
}