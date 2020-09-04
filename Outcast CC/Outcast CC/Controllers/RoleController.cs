using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Outcast_CC.Models;
namespace Outcast_CC.Controllers
{
  [Authorize(Roles = "Admin")]
  public class RoleController : Controller
  {
    public async Task<ActionResult> Index()
    {
      var roleManager = this.RoleManager;
      var userManager = this.UserManager;

      List<AppRole> roles =
        await RoleManager.Roles.Include(x => x.Users).OrderBy(x => x.Name).ToListAsync();
      Dictionary<string, AppUser> users =
        await userManager.Users.ToDictionaryAsync(x => x.Id);

      ViewBag.users = users;

      return View(roles);
    }
    public ActionResult Create()
    {
      return View();
    }
    [HttpPost]
    public async Task<ActionResult> Create([Required]string name)
    {
      if (ModelState.IsValid)
      {
        IdentityResult result
        = await RoleManager.CreateAsync(new AppRole(name));
        if (result.Succeeded)
        {
          return RedirectToAction("Index");
        }
        else
        {
          AddErrorsFromResult(result);
        }
      }
      return View(name);
    }
    [HttpPost]
    public async Task<ActionResult> Delete(string id)
    {
      AppRole role = await RoleManager.FindByIdAsync(id);
      if (role != null)
      {
        IdentityResult result = await RoleManager.DeleteAsync(role);
        if (result.Succeeded)
        {
          return RedirectToAction("Index");
        }
        else
        {
          return View("Error", result.Errors);
        }
      }
      else
      {
        return View("Error", new string[] { "Role Not Found" });
      }
    }
    [HttpGet]
    public async Task<ActionResult> Edit(string id)
    {
      var roleManager = this.RoleManager;
      var role = await RoleManager.FindByIdAsync(id);
      if (role == null)
      {
        return View("Error", new string[] { "Role Not Found" });
      }

      return View(GetEditModel(role));
    }
    [HttpPost]
    public async Task<ActionResult> Edit(
      string id,
      [Required] string name,
      string[] idstoadd,
      string[] idstodelete)
    {
      var roleManager = this.RoleManager;
      var role = await RoleManager.FindByIdAsync(id);
      if (role == null)
      {
        return View("Error", new string[] { "Role Not Found" });
      };
      if (role.Name != name)
      {
        role.Name = name;
        var result = await roleManager.UpdateAsync(role);
        AddErrorsFromResult(result);
      };
      var userManager = this.UserManager;
      if (idstoadd != null)
      {
        foreach(var userId in idstoadd)
        {
          var result = await UserManager.AddToRoleAsync(userId, name);
          AddErrorsFromResult(result);
        }
      }
      if (idstodelete != null)
      {
        foreach (var userId in idstodelete)
        {
          var result = await UserManager.RemoveFromRoleAsync(userId, name);
          AddErrorsFromResult(result);
        }
      }
      if (ModelState.IsValid)
      {
        return RedirectToAction("Index");
      }
      else
      {
        return View(GetEditModel(role));
      }
    }

    private RoleEditModel GetEditModel(AppRole role)
    {
      var userManager = this.UserManager;
      var users = UserManager.Users.ToList();
      var memberIds = role.Users.Select(x => x.UserId).ToList();
      var members = users.Where(x => memberIds.Contains(x.Id));
      var nonMembers = users.Except(members);

      var model = new RoleEditModel
      {
        Role = role,
        Members = members,
        NonMembers = nonMembers,
      };
      return model;
    }

    private OutcastCCDatabase database
    {
      get
      {
        return HttpContext.GetOwinContext().Get<OutcastCCDatabase>();
      }
    }

    private void AddErrorsFromResult(IdentityResult result)
    {
      foreach (string error in result.Errors)
      {
        ModelState.AddModelError("", error);
      }
    }
    private AppUserManager UserManager
    {
      get
      {
        return HttpContext.GetOwinContext().GetUserManager<AppUserManager>();
      }
    }
    private AppRoleManager RoleManager
    {
      get
      {
        return HttpContext.GetOwinContext().GetUserManager<AppRoleManager>();
      }
    }
  }
}