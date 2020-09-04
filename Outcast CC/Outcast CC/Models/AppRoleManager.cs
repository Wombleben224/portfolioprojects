using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Outcast_CC.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;

namespace Outcast_CC.Models
{
  public class AppRoleManager : RoleManager<AppRole>
  {
    public AppRoleManager(RoleStore<AppRole> store) : base(store)
    {

    }

    public static AppRoleManager Create(
      IdentityFactoryOptions<AppRoleManager> options,
      IOwinContext context)
    {
      var db = context.Get<OutcastCCDatabase>();
      var manager = new AppRoleManager(new RoleStore<AppRole>(db));
      return manager;
    }
  }
}