using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Outcast_CC.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Outcast_CC.Controllers
{
  public class ChatController : Controller
  {
    private OutcastCCDatabase _db = new OutcastCCDatabase();
    // GET: Chat
    [Authorize]
    public ActionResult Chat()
    {
      ViewBag.user = User.Identity.Name;
      return View();
    }

    [HttpPost]
    public async Task<ActionResult> GetMessages(
      int page = 1, int pageSize = 100)
    {
      var messages = await _db.Messages
        .OrderByDescending(x => x.Sent)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

      var messagesFormatted =
          messages.Select(x => new
          {
            x.Id,
            x.User,
            x.Text,
            Sent = x.Sent.ToString("HH:mm")
          });
      return Json(messagesFormatted);
    }
    [HttpPost]
    public async Task<ActionResult> SendMessage(string user, string text)
    {
      var msg = new Message
      {
        Id = Guid.NewGuid(),
        User = user,
        Text = text,
        Sent = DateTime.Now
      };

      try
      {
        _db.Messages.Add(msg);
        await _db.SaveChangesAsync();
        return Json(new { Success = true, Message = "Message Sent" });
      }
      catch (Exception ex)
      {
        Debug.WriteLine("Error: " + ex.Message);
        return new HttpStatusCodeResult(System.Net.HttpStatusCode.InternalServerError);
      }
    }



    private AppUserManager UserManager
    {
      get
      {
        return HttpContext.GetOwinContext().GetUserManager<AppUserManager>();
      }
    }
  }
}