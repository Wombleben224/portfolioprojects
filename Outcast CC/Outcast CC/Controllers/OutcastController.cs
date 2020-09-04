using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Outcast_CC.Models;
using Outcast_CC.HtmlHelpers;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Net;
using SendGrid.Helpers.Mail;
using SendGrid;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace Outcast_CC.Controllers
{
  public class OutcastController : Controller
  {
    private OutcastCCDatabase _db = new OutcastCCDatabase();
    // GET: Outcast
    public ActionResult Index()
    {
      return View();
    }
     
    [HttpGet]
    public async Task<ActionResult> Events(
      int page = 1,
      int pagesize = 4,
      string q = null)
    {
      var userManager = this.UserManager;
      var userId = User.Identity.GetUserId();
      var user = await UserManager.FindByIdAsync(userId);

      List<Event> events = await _db.Events
        .OrderByDescending(x => x.EventDate)
          .Where(x => x.Title.Contains(q) || q == null)
          .Skip((page - 1) * pagesize)
          .Take(pagesize)
          .ToListAsync();

      List<int> userEvents = new List<int>();

      if (user != null)
      {
        userEvents = await _db.UserEvents.Where(x => x.Id == user.Id).Select(x => x.EventId).ToListAsync();
      }
      int count =
        await _db.Events
        .Where(x => x.Title.Contains(q) || q == null)
        .CountAsync();

      var model = new EventViewModel
      {
        EventId = userEvents,
        Query = q,
        Events = events,
        pagingInfo = new PagingInfo
        {
          CurrentPage = page,
          ItemsPerPage = pagesize,
          TotalItems = count
        },
      };
      return View(model);
    }

    public async Task<ActionResult> ViewEvent(int? Eventid = null)
    {
      Event Event = await _db.Events.SingleOrDefaultAsync(x => x.EventId == Eventid);

      var userManager = UserManager;
      var users = userManager.Users;

      List<UserEvents> AllUserEvents = await _db.UserEvents.Where(x => x.EventId == Event.EventId).ToListAsync();
      List<string> usersGoing = new List<string>();
      foreach(UserEvents userEvents in AllUserEvents)
      {
        if (Event.EventId == userEvents.EventId)
        {
          var user = await UserManager.FindByIdAsync(userEvents.Id);
          usersGoing.Add(user.UserName);
        }
      }

      Event.UsersGoing = usersGoing;
      return View(Event);
    }
    [Authorize]
    [HttpPost]
    public async Task<ActionResult> Going(int EventId)
    {
      var userManager = this.UserManager;
      var userId = User.Identity.GetUserId();
      var user = await UserManager.FindByIdAsync(userId);

      UserEvents userEvents = new UserEvents();
      userEvents.Id = user.Id;
      userEvents.EventId = EventId;

      Event thisevent = await _db.Events.SingleOrDefaultAsync(x => x.EventId == EventId);
      if (!ModelState.IsValid)
      {
        return Json(new { Success = false, Message = "Invalid Data" });
      }
      else
      {
        if (thisevent.Going != null)
        {
          ++thisevent.Going;
        }
        else
        {
          thisevent.Going = 1;
        }
        _db.UserEvents.Add(userEvents);
        await _db.SaveChangesAsync();
        return Json(new { Success = true, Message = "Message Sent", Going = thisevent.Going });
      }
    }
    [Authorize]
    [HttpPost]
    public async Task<ActionResult> CancelGoing(int CancelledEventId)
    {
      var userManager = this.UserManager;
      var userId = User.Identity.GetUserId();
      var user = await UserManager.FindByIdAsync(userId);

      Event thisevent = await _db.Events.SingleOrDefaultAsync(x => x.EventId == CancelledEventId);

      List<UserEvents> AllUserEvents = await _db.UserEvents.Where(x => x.EventId == CancelledEventId).ToListAsync();
      UserEvents removeUserEvent = AllUserEvents.SingleOrDefault(x => x.Id == userId);
      if (!ModelState.IsValid)
      {
        return Json(new { Success = false, Message = "Invalid Data" });
      }
      else
      {
        if (thisevent.Going != null)
        {
          --thisevent.Going;
        }
        _db.UserEvents.Remove(removeUserEvent);
        await _db.SaveChangesAsync();
        return Json(new { Success = true, Message = "Message Sent", Going = thisevent.Going });
      }
    }

    [HttpGet]
    public async Task<ActionResult> Members(
      int page = 1,
      int pagesize = 9,
      string q = null)
    {
      List<Member> MemberList = await _db.Members
        .OrderBy(x => x.Name)
          .Where(x => x.Name.Contains(q) || q == null)
          .Skip((page - 1) * pagesize)
          .Take(pagesize)
          .ToListAsync();

      int count =
        await _db.Members
        .Where(x => x.Name.Contains(q) || q == null)
        .CountAsync();

      var model = new MemberViewModel
      {
        Query = q,
        Members = MemberList,
        pagingInfo = new PagingInfo
        {
          CurrentPage = page,
          ItemsPerPage = pagesize,
          TotalItems = count
        },
      };

      return View(model);
    }

    [HttpGet]
    public ActionResult MemberDetails(string id = null)
    {
      Member member = _db.Members.SingleOrDefault(x => x.memberId.Equals(id));

      return View(member);
    }
    public ActionResult EventKeywords(string term)
    {
      List<string> Titles =
        _db.Events
        .Where(x => x.Title.Contains(term))
        .Select(x => x.Title.ToLower())
        .Distinct()
        .ToList();

      var spelling = new NHunspell.Hunspell("en_US.aff", "en_US.dic");
      var suggestions =
        spelling.Spell(term) ?
        new List<string>() { term } :
        spelling.Suggest(term).Take(5);

      var results = suggestions.Union(Titles);

      return Json(results, JsonRequestBehavior.AllowGet);
    }
    public ActionResult MemberKeywords(string term)
    {
      List<string> Names =
        _db.Members
        .Where(x => x.Name.Contains(term))
        .Select(x => x.Name.ToLower())
        .Distinct()
        .ToList();

      var spelling = new NHunspell.Hunspell("en_US.aff", "en_US.dic");
      var suggestions =
        spelling.Spell(term) ?
        new List<string>() { term } :
        spelling.Suggest(term).Take(5);

      var results = suggestions.Union(Names);

      return Json(results, JsonRequestBehavior.AllowGet);
    }
    [HttpGet]
    public async Task<ActionResult> Subscribe()
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
    public async Task<ActionResult> Subscribe(AppUser model)
    {
      var userManager = this.UserManager;
      var userId = User.Identity.GetUserId();
      var user = await UserManager.FindByIdAsync(userId);

      if (!ModelState.IsValid)
      {
        ModelState.AddModelError("", "Please ensure all data is valid.");
        return View(user);
      }

      // Clean the data
      user.UserName = user.UserName.Trim();
      user.Email = user.Email.Trim().ToLower();
      user.IsConfirmed = false;
      user.IsSubscribed = false;
      user.UnsubscribeUrl = Url.Action("Unsubscribe", "Outcast", new { user.Email }, "https"); // include the protocol to get an absolute path
      try
      {
        // Save the subscriber info to the database
        var dbEntry = await _db.Users.FirstOrDefaultAsync(x => x.Email == user.Email);
        if (dbEntry == null)
        {
          // New subscriber
          dbEntry.IsSubscribed = true;
          dbEntry.UnsubscribeUrl = user.UnsubscribeUrl;
          await _db.SaveChangesAsync();
        }
        else
        {
          // Existing subscriber
          // don't change the email or status flags
          dbEntry.UserName = user.UserName;
          dbEntry.IsSubscribed = true;
          dbEntry.UnsubscribeUrl = user.UnsubscribeUrl;
          await _db.SaveChangesAsync();
        }
      }
      catch (Exception ex)
      {
        Debug.WriteLine(ex.Message);
        ModelState.AddModelError("", "Failed to save information to database. Please try again.");
        ModelState.AddModelError("", ex.Message);
      }

      try
      {
        // Send the confirmation email to the subscriber
        var apiKey = GetApiKey();
        var client = new SendGridClient(apiKey);

        var from = GetFromEmail();
        var to = new EmailAddress(user.Email, user.UserName);
        var subject = "Please Confirm Your Email Address";

        var confirmLink = Url.Action("Confirm", "Outcast", new { user.Email }, "https"); // include the protocol to get an absolute path
        var plainTextContent =
            "Please follow the link below to confirm your subscription\n\n" + confirmLink;
        var htmlContent =
            "<strong>Please follow the link below to confirm your subscription</strong>" +
            $"<br><a href='{confirmLink}'>{confirmLink}</a>";

        var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
        var result = await client.SendEmailAsync(msg);
        if (result.StatusCode != HttpStatusCode.Accepted)
        {
          throw new Exception($"Failed to send email: {result.StatusCode}");
        }
      }
      catch (Exception ex)
      {
        Debug.WriteLine(ex.Message);
        ModelState.AddModelError("", "Failed to send confirmation email. Please try again.");
        ModelState.AddModelError("", ex.Message);
      }

      if (ModelState.IsValid)
      {
        // Redirect the user to the Thanks action
        return RedirectToAction("Thanks", new { user.Email });
      }
      else
      {
        // Redisplay the subscribe form, so that users can try again
        return View(user);
      }
    }

    public async Task<ActionResult> Thanks(string email)
    {
      var userManager = this.UserManager;
      var userId = User.Identity.GetUserId();
      var user = await UserManager.FindByIdAsync(userId);

      return View(user);
    }

    private string GetApiKey()
    {
      return Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
    }

    private EmailAddress GetFromEmail()
    {
      string email = Environment.GetEnvironmentVariable("SENDGRID_FROM_EMAIL") ?? "wombleben224@gmail.com";
      string name = Environment.GetEnvironmentVariable("SENDGRID_FROM_NAME") ?? "Ben Womble";
      return new EmailAddress(email, name);
    }
    public async Task<ActionResult> Confirm(string email)
    {
      try
      {
        var userManager = this.UserManager;
        var user = await UserManager.FindByEmailAsync(email);
        if (user == null)
        {
          ModelState.AddModelError("", "We were unable to find your email address in the database.");
          return View(user);
        }

        var dbEntry = await _db.Users.FirstOrDefaultAsync(x => x.Email == user.Email);
        if (dbEntry != null)
        {
          dbEntry.IsSubscribed = true;
          dbEntry.UnsubscribeUrl = Url.Action("Unsubscribe", "Outcast", new { user.Email }, "https"); // include the protocol to get an absolute path
          dbEntry.IsConfirmed = true;
          await _db.SaveChangesAsync();
        }

        // Send the welcome email to the subscriber
        var apiKey = GetApiKey();
        var client = new SendGridClient(apiKey);

        var from = GetFromEmail();
        var to = new EmailAddress(user.Email, user.UserName);
        var subject = "Welcome to the group!";
        var body = "You are now subscribed to the weekly mailing list. See you soon!";
        var msg = MailHelper.CreateSingleEmail(from, to, subject, body, body);

        var result = await client.SendEmailAsync(msg);
        if (result.StatusCode != HttpStatusCode.Accepted)
        {
          throw new Exception($"Failed to send email: {result.StatusCode}");
        }

        return View(user);
      }
      catch (Exception ex)
      {
        Debug.WriteLine(ex.Message);
        ModelState.AddModelError("", "Failed to confirm email. Please try again.");
        ModelState.AddModelError("", ex.Message);
        return View(User);
      }
    }
    public async Task<ActionResult> Unsubscribe(string email)
    {
      var userManager = this.UserManager;
      var userId = User.Identity.GetUserId();
      var sub = await UserManager.FindByIdAsync(userId);
      try
      {
        if (sub == null)
        {
          ModelState.AddModelError("", "We were unable to find your email address in the database.");
          return View(sub);
        }

        var dbEntry = await _db.Users.FirstOrDefaultAsync(x => x.Id == userId);
        if (dbEntry != null)
        {
          dbEntry.IsSubscribed = false;
          dbEntry.IsConfirmed = false;
          await _db.SaveChangesAsync();
        }

        // Update the database
        sub.IsSubscribed = false;
        await _db.SaveChangesAsync();

        return View(sub);
      }
      catch (Exception ex)
      {
        Debug.WriteLine(ex.Message);
        ModelState.AddModelError("", "Failed to unsubscribe email. Please try again.");
        ModelState.AddModelError("", ex.Message);
        return View(sub);
      }
    }
    private AppUserManager UserManager
    {
      get { return HttpContext.GetOwinContext().GetUserManager<AppUserManager>(); }
    }

    public async Task<ActionResult> getAllEvents(String q)
    {
      var events = await _db.Events.OrderByDescending(x => x.EventDate).ToListAsync();
      if(q != null)
      {
        events = await _db.Events.Where(x => x.Title.Contains(q)).OrderByDescending(x => x.EventDate).ToListAsync();
      }
      return Json(events, JsonRequestBehavior.AllowGet);
    }

    public async Task<ActionResult> getAllMembers(String q)
    {
      var members = await _db.Members.ToListAsync();
      if (q != null)
      {
        members = await _db.Members.Where(x => x.Name.Contains(q)).ToListAsync();
      } 

      return Json(members, JsonRequestBehavior.AllowGet);
    }
  }
}