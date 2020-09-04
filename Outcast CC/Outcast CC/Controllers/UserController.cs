using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Outcast_CC.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Outcast_CC.Controllers
{
  [Authorize(Roles = "Admin")]
  public class UserController : Controller
  {
    private OutcastCCDatabase _db = new OutcastCCDatabase();
    // GET: User
    [Authorize]
    public ActionResult Index()
    {
      var userManager = UserManager;
      var users = userManager.Users;
      return View(users.OrderBy(x => x.UserName));
    }

    public ActionResult Create()
    {
      return View();
    }

    [HttpPost]
    public async Task<ActionResult> Create(CreateUserModel model)
    {
      if (!ModelState.IsValid)
      {
        return View(model);
      }

      var userManager = this.UserManager;
      var user = new AppUser
      {
        UserName = model.UserName,
        Email = model.Email,
      };
      IdentityResult result = await UserManager.CreateAsync(user, model.Password);
      if (result.Succeeded)
      {
        TempData["Message"] = $"{model.UserName} created!";
        return RedirectToAction("Index");
      }
      else
      {
        AddErrorsFromResult(result);
        return View(model);
      }
    }

    [HttpGet]
    public async Task<ActionResult> Edit(string id)
    {
      var userManager = this.UserManager;
      var user = await UserManager.FindByIdAsync(id);
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
    public async Task<ActionResult> Edit(string id, string email, string password)
    {
      var userManager = this.UserManager;
      var user = await UserManager.FindByIdAsync(id);
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
    [HttpPost]
    public async Task<ActionResult> Delete(string id)
    {
      var userManager = this.UserManager;
      var user = await UserManager.FindByIdAsync(id);
      if (user == null)
      {
        return View("Error", new string[] { "User not found!" });
      }

      Member member = await _db.Members.SingleOrDefaultAsync(x => x.memberId == user.MemberId);

      IdentityResult result = await userManager.DeleteAsync(user);
      if (result.Succeeded)
      {
        if (member != null)
        {
          _db.Members.Remove(member);
        }
        await _db.SaveChangesAsync();
        TempData["Message"] = $"{user.UserName} deleted!";
        return RedirectToAction("Index");
      }
      else
      {
        return View("Error", result.Errors);
      }
    }

    private void AddErrorsFromResult(IdentityResult result)
    {
      foreach(string err in result.Errors)
      {
        ModelState.AddModelError("", err);
      }
    }

    private AppUserManager UserManager
    {
      get { return HttpContext.GetOwinContext().GetUserManager<AppUserManager>(); }
    }
  }
}