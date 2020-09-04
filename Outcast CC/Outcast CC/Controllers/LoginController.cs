using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Outcast_CC.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Security;

namespace Outcast_CC.Controllers
{
  [Authorize]
  public class LoginController : Controller
  {
    private OutcastCCDatabase _db = new OutcastCCDatabase();

    [HttpGet]
    public async Task<ActionResult> EditMember(string id = null)
    {
      if (id == null)
      {
        var userManager = this.UserManager;
        var userId = User.Identity.GetUserId();
        var user = await UserManager.FindByIdAsync(userId);
        Member member = await _db.Members.SingleOrDefaultAsync(x => x.memberId.Equals(user.MemberId));
        return View(member);
      }
      else
      {
        Member member = await _db.Members.SingleOrDefaultAsync(x => x.memberId == id);
        return View(member);
      }
    }
    [HttpPost]
    public async Task<ActionResult> EditMember(Member member, HttpPostedFileBase ProfileImage = null)
    {
      if (!ModelState.IsValid)
      {
        ModelState.AddModelError("", "Please ensure all data is valid");
        return View();
      }

      Member dbMember = await _db.Members.SingleOrDefaultAsync(x => x.memberId == member.memberId);
      dbMember.Name = member.Name;
      dbMember.Username = member.Username;
      dbMember.Bio = member.Bio;
      dbMember.VehicleMake = member.VehicleMake;
      dbMember.VehicleModel = member.VehicleModel;
      dbMember.VehicleYear = member.VehicleYear;
      dbMember.memberId = member.memberId;

      string photos = Path.Combine(Server.MapPath("~/Content/Photos"), "0");
      string thumbnailFolder = Path.Combine(photos, "Thumbnails");
      if (!Directory.Exists(photos))
      {
        Directory.CreateDirectory(photos);
      }
      if (!Directory.Exists(thumbnailFolder))
      {
        Directory.CreateDirectory(thumbnailFolder);
      }

      if (ProfileImage != null)
      {
        dbMember.ProfileImageType = ProfileImage.ContentType;
        dbMember.ProfileImageName = Path.GetFileName(ProfileImage.FileName);

        var ext = Path.GetExtension(ProfileImage.FileName).ToLower();

        if (ext != ".png" && ext != ".jpg" && ext != ".jpeg")
        {
          throw new Exception("File must .jpg, .jpeg or .png format");
        }
        if (ProfileImage.ContentLength > 16 * 1024 * 1024)
        {
          throw new Exception("Profile image is too big!");
        }

        WebImage img = new WebImage(ProfileImage.InputStream);
        if (img.Width > 2048 || img.Height > 2048)
        {
          throw new Exception("Profile Image is too big!");
        }
        else if (img.Width < 256 || img.Height < 256)
        {
          throw new Exception("Profile Image is too small!");
        }
        if (img.Width > 512 || img.Height > 512)
        {
          img.Resize(512, 512);
        }
        img.Save(Path.Combine(photos, dbMember.ProfileImageName));

        img.Resize(128, 128);
        img.Save(Path.Combine(thumbnailFolder, dbMember.ProfileImageName));
      }
      await _db.SaveChangesAsync();
      return Json(new { Success = true, Message = $"Member #{dbMember.Name} updated" });
    }

    public async Task<ActionResult> GetProductByName(string Username)
    {
      var member =
        await _db.Members
        .Select(x => new { x.memberId, x.Username })
        .FirstOrDefaultAsync(x => x.Username == Username);

      return Json(member);
    }

    private AppUserManager UserManager
    {
      get { return HttpContext.GetOwinContext().GetUserManager<AppUserManager>(); }
    }
  }
}