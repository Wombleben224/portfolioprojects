using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Outcast_CC.Models
{
  public class AppUserManager : UserManager<AppUser>
  {
    public AppUserManager(UserStore<AppUser> store)
      :base(store)
    {

    }
    public static AppUserManager Create(
      IdentityFactoryOptions<AppUserManager> options,
      IOwinContext context)
    {
      var db = context.Get<OutcastCCDatabase>();
      var manager = new AppUserManager(new UserStore<AppUser>(db));

      manager.PasswordValidator = new PasswordValidator
      {
        RequiredLength = 6,
        RequireNonLetterOrDigit = false,
        RequireDigit = false,
        RequireLowercase = false,
        RequireUppercase = true,
      };

      manager.UserValidator = new CustomUserValidator(manager)
      {
        AllowOnlyAlphanumericUserNames = true,
        RequireUniqueEmail = true,

      };

      return manager;
    }
  }
}