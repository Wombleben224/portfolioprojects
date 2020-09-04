using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Outcast_CC.Models
{
  public class CustomPasswordValidator : PasswordValidator
  {
    public override async Task<IdentityResult> ValidateAsync(string pass)
    {
      IdentityResult result = await base.ValidateAsync(pass);

      if (pass.Contains("12345"))
      {
        var errors = result.Errors.Append("Passwords cannot contain numeric sequences");
        result = new IdentityResult(errors);
      }

      return result;
    }
  }
}