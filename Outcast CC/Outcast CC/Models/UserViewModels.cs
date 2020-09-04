using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Outcast_CC.Models
{
  public class CreateUserModel
  {
    [Display(Name ="User Name")]
    [Required(ErrorMessage ="Please enter your Username.")]
    public string UserName { get; set; }
    [Required(ErrorMessage = "Please enter your email address")]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; }
    [Required(ErrorMessage = "Please enter your password")]
    [DataType(DataType.Password)]
    public string Password { get; set; }
  }

  public class CreateRoleModel
  {
    [Required(ErrorMessage = "Please Enter A Name")]
    public string Name { get; set; }
  }

  public class LoginModel
  {
    [Display(Name = "User Name")]
    [Required(ErrorMessage = "Please enter you Username.")]
    public string UserName { get; set; }

    [Required(ErrorMessage = "Please enter your password")]
    [DataType(DataType.Password)]
    public string Password { get; set; }
  }

  public class RoleEditModel
  {
    public AppRole Role { get; set; }
    public IEnumerable<AppUser> Members { get; set; }
    public IEnumerable<AppUser> NonMembers { get; set; }
  }

}