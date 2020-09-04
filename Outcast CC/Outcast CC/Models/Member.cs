namespace Outcast_CC.Models
{
  using System;
  using System.Collections.Generic;
  using System.ComponentModel.DataAnnotations;
  using System.ComponentModel.DataAnnotations.Schema;
  using System.Data.Entity.Spatial;

  public partial class Member
  {
    [Required]
    [StringLength(128)]
    public string memberId { get; set; }

    [Required]
    [StringLength(50)]
    [Display(Name = "UserName")]
    public string Username { get; set; }

    [Required]
    [StringLength(50)]
    [Display(Name = "Name")]
    public string Name { get; set; }

    [Required]
    [Display(Name = "Year of Vehicle")]
    public int VehicleYear { get; set; }

    [Required]
    [StringLength(50)]
    [Display(Name = "Make Of Vehicle")]
    public string VehicleMake { get; set; }

    [Required]
    [StringLength(50)]
    [Display(Name = "Model Of Vehicle")]
    public string VehicleModel { get; set; }

    [Required]
    [StringLength(1500)]
    [Display(Name = "User Bio")]
    public string Bio { get; set; }

    [StringLength(100)]
    public string ProfileImageType { get; set; }

    [StringLength(100)]
    public string ProfileImageName { get; set; }
  }
}
