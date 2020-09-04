using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Outcast_CC.Models
{
  public class CustomUserValidator : UserValidator<AppUser>
  {
    public CustomUserValidator(AppUserManager manager)
      : base(manager)
    {

    }

    public override async Task<IdentityResult> ValidateAsync(AppUser user)
    {
      IdentityResult result = await base.ValidateAsync(user);

      if (user.Email.EndsWith("@example.com", StringComparison.OrdinalIgnoreCase))
      {
        var errors = result.Errors.Append("example.com email address are not allowed");
        result = new IdentityResult(errors);
      }

      return result;
    }
  }
}