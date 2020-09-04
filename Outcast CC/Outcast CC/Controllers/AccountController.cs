using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Outcast_CC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Outcast_CC.Controllers
{
  [Authorize]
  public class AccountController : Controller
  {
    [AllowAnonymous]
    [HttpGet]
    public ActionResult Login(string returnUrl)
    {
      ViewBag.returnUrl = returnUrl;
      return View();
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<ActionResult> Login(LoginModel model, string returnUrl)
    {
      ViewBag.returnUrl = returnUrl;
      if (!ModelState.IsValid)
      {
        return View(model);
      }

      var userManager = this.UserManager;
      var user = await userManager.FindByNameAsync(model.UserName);
      if(user == null)
      {
        user = await userManager.FindByEmailAsync(model.UserName);
      }
      if (user != null)
      {
        PasswordVerificationResult validPassword =
          UserManager.PasswordHasher.VerifyHashedPassword(
            user.PasswordHash, model.Password);
        if (validPassword == PasswordVerificationResult.Failed)
        {
          user = null;
        }
      }
      if (user == null)
      {
        ModelState.AddModelError("", "Invalid Username or Password");
        return View(model);
      }
      var identity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);

      AuthManager.SignOut();
      AuthManager.SignIn(new AuthenticationProperties
      {
        IsPersistent = false,
      }, identity);

      return Redirect(Url.Action("Index", "Outcast"));
    }

    [HttpGet]
    public async Task<ActionResult> Edit()
    {
      var userManager = this.UserManager;
      var userId = User.Identity.GetUserId();
      var user = await UserManager.FindByIdAsync(userId);
      if (user == null)
      {
        return View("Error", new string[] { "User Not Found" });
      }
      else
      {
        return View(user);
      }
    }

    [HttpPost]
    public async Task<ActionResult> Edit(string email, string password)
    {
      var userManager = this.UserManager;
      var userId = User.Identity.GetUserId();
      var user = await UserManager.FindByIdAsync(userId);
      if (user == null)
      {
        return View("Error", new string[] { "User Not Found" });
      }
      user.Email = email;
      IdentityResult validEmail = await userManager.UserValidator.ValidateAsync(user);
      if (!validEmail.Succeeded)
      {
        AddErrorsFromResult(validEmail);
      }

      if (string.IsNullOrWhiteSpace(password))
      {
        IdentityResult validPass = await userManager.PasswordValidator.ValidateAsync(password);
        if (validPass.Succeeded)
        {
          user.PasswordHash = UserManager.PasswordHasher.HashPassword(password);
        }
        else
        {
          AddErrorsFromResult(validPass);
        }
      }

      if (ModelState.IsValid)
      {
        IdentityResult saved = await UserManager.UpdateAsync(user);
        if (saved.Succeeded)
        {
          TempData["Message"] = $"{user.UserName} Updated!";
          return RedirectToAction("Index");
        }
        else
        {
          AddErrorsFromResult(saved);
          return View(user);
        }
      }
      else
      {
        return View(user);
      }
    }
    private void AddErrorsFromResult(IdentityResult result)
    {
      foreach (string err in result.Errors)
      {
        ModelState.AddModelError("", err);
      }
    }
    private IAuthenticationManager AuthManager
    {
      get { return HttpContext.GetOwinContext().Authentication; }
    }

    private AppUserManager UserManager
    {
      get { return HttpContext.GetOwinContext().GetUserManager<AppUserManager>(); }
    }

    [AllowAnonymous]
    public ActionResult Logout()
    {
      AuthManager.SignOut();
      TempData["Message"] = $"Logged Out.";
      return RedirectToAction("Index", "Home");
    }
  }
}