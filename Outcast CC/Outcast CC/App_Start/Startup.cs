using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Outcast_CC.Models;
using Microsoft.Owin.Security.Cookies;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;

public partial class Startup
{
  public void Configuration(IAppBuilder app)
  {
    app.CreatePerOwinContext(OutcastCCDatabase.Create);
    app.CreatePerOwinContext<AppUserManager>(AppUserManager.Create);
    app.CreatePerOwinContext<AppRoleManager>(AppRoleManager.Create);
    app.UseCookieAuthentication(new CookieAuthenticationOptions
    {
      AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
      LoginPath = new PathString("/Account/Login")
    });
  }
}