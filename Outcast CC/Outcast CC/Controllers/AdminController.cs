using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using ExcelDataReader;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Outcast_CC.Models;

namespace Outcast_CC.Controllers
{
  public class AdminController : Controller
  {
    private OutcastCCDatabase _db = new OutcastCCDatabase();


    // GET: Admin
    [Authorize(Roles = "Admin")]
    public ActionResult Index()
    {
      return View();
    }
    [Authorize(Roles = "Admin")]
    public ActionResult CreateEvent()
    {
      return View();
    }
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> EditEvent(int Eventid)
    {
      Event model = await _db.Events.SingleOrDefaultAsync(x => x.EventId == Eventid);
      return View(model);
    }
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult> EditEvent(Event newEvent, HttpPostedFileBase file = null)
    {
      Event DbEvent = await _db.Events.SingleOrDefaultAsync(x => x.EventId == newEvent.EventId);
      DbEvent.EventId = newEvent.EventId;
      DbEvent.EventDate = newEvent.EventDate;
      DbEvent.Going = newEvent.Going;
      DbEvent.Location = newEvent.Location;
      DbEvent.Text = newEvent.Text;
      DbEvent.Title = newEvent.Title;

      if (!ModelState.IsValid)
      {
        return Json(new { Success = false, Message = $"Failed" });
      }

      string EventDocFolder = Path.Combine(Server.MapPath("~/Content/Events"), "0");

      if (!Directory.Exists(EventDocFolder))
      {
        Directory.CreateDirectory(EventDocFolder);
      }

      if (file != null)
      {
        if (Path.GetExtension(file.FileName).ToLower() != ".pdf")
        {
          throw new Exception("File must .pdf format");
        }
        if (file.ContentLength > (16 * 1024 * 1024))
        {
          throw new Exception("Resume PDF is too big!");
        }
        file.SaveAs(Path.Combine(EventDocFolder, file.FileName));
        DbEvent.PdfFileName = file.FileName;
        DbEvent.PdfFileType = file.ContentType;
      }

      await _db.SaveChangesAsync();
      TempData["message"] = $"Event #{DbEvent.Title} Saved";
      return Json(new { Success = true, Message = $"Event #{DbEvent.Title} Saved" });
    }

    public class ImportedEvent
    {
      public int RowNumber { get; set; }
      public string Title { get; set; }
      public string Location { get; set; }
      public DateTime EventDate { get; set; }
      public string Text { get; set; }
    }

    [HttpGet]
    public ActionResult UploadEvents()
    {
      return View();
    }
    [HttpPost]
    public async Task<ActionResult> UploadEvents(HttpPostedFileBase file)
    {
      if(file == null)
      {
        return View();
      }

      string EventExcelFolder = Path.Combine(Server.MapPath("~/Content/Excel"));

      if (!Directory.Exists(EventExcelFolder))
      {
        Directory.CreateDirectory(EventExcelFolder);
      }

      if (file != null)
      {
        if (Path.GetExtension(file.FileName).ToLower() != ".xlsx")
        {
          throw new Exception("File must .xlsx format");
        }
        file.SaveAs(Path.Combine(EventExcelFolder, file.FileName));
        string path = Path.Combine((EventExcelFolder), file.FileName);
        DataSet dataSet = LoadFileIntoDataSet(path);
        await ImportEventData(dataSet);
      }
      return RedirectToAction("Events", "Outcast");
    }

    private DataSet LoadFileIntoDataSet(string path, bool hasHeaderRow = true)
    {
      Func<Stream, IExcelDataReader> createReader;
      if (Path.GetExtension(path).ToLower() == ".xlsx" ||
      Path.GetExtension(path).ToLower() == ".xls")
      {
        createReader = stream => ExcelReaderFactory.CreateReader(stream);
      }
      else if (Path.GetExtension(path).ToLower() == ".csv")
      {
        createReader = stream => ExcelReaderFactory.CreateCsvReader(stream);
      }
      else
      {
        throw new Exception("File is neither a .csv, .xls, or .xlsx file");
      }
      using (var stream =
      System.IO.File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read))
      {
        using (var reader = createReader(stream))
        {
          var dataSet = reader.AsDataSet(new ExcelDataSetConfiguration
          {
            ConfigureDataTable = _ => new ExcelDataTableConfiguration
            {
              UseHeaderRow = hasHeaderRow
            }
          });
          return dataSet;
        }
      }
    }

    private async Task ImportEventData(DataSet dataSet)
    {
      //  Verify that there is only one tab in the spreadsheet
      if (dataSet.Tables.Count != 1)
      {
        throw new Exception($"Expected one sheet in Excel file, found {dataSet.Tables.Count}");
      }
      //  Get the first tab of the spreadsheet
      DataTable table = dataSet.Tables[0];

      //Verify that the columns are what we expect
      if (table.Columns.Count != 4 ||
        table.Columns[0].ColumnName != "Title" ||
        table.Columns[1].ColumnName != "Location" ||
        table.Columns[2].ColumnName != "EventDate" ||
        table.Columns[3].ColumnName != "Text")
      {
        throw new Exception("Expected columns Title, Location, EventDate, Text");
      }
      //  Load data from the table into memory
      var importedEvents = new List<ImportedEvent>();
      int rowNumber = 1;
      foreach (DataRow row in table.Rows)
      {
        ++rowNumber;
        var evt = new ImportedEvent
        {
          RowNumber = rowNumber,
          Title = row[0].ToString().Trim(),
          Location = row[1].ToString().Trim(),
          Text = row[3].ToString().Trim(),
        };
        DateTime evtDate;
        if(DateTime.TryParse(row[2].ToString().Trim(), out evtDate))
        {
          evt.EventDate = evtDate;
        }
        else
        {
          throw new Exception("Date in improper format, expected DateTime Format");
        }
        importedEvents.Add(evt);
      }

      // check for dupli

      //  Load existing event data from the database
      var dates = importedEvents.Select(x => x.EventDate);
      var dbEvents = _db.Events.Where(x => dates.Contains(x.EventDate)).ToList();
      var existingEvents = (
        from e in dbEvents
        join i in importedEvents
          on new { e.Title, e.Location, e.EventDate }
          equals new { i.Title, i.Location, i.EventDate }
        select e
      ).ToList();

      //  Filter out existing events
      var newEvents =
        from i in importedEvents
        join e in existingEvents
          on new { i.Title, i.Location, i.EventDate }
          equals new { e.Title, e.Location, e.EventDate }
        into g
        where !g.Any()
        select new Event
        {
          Title = i.Title,
          Location = i.Location,
          EventDate = i.EventDate,
          Text = i.Text,
          Going = 0,
      };
      //  Save only the new customers
      _db.Events.AddRange(newEvents);
      _db.SaveChanges();
    }

    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteEvent(int id)
    {
      Event model = await _db.Events.SingleOrDefaultAsync(x => x.EventId == id);
      _db.Events.Remove(model);
      await _db.SaveChangesAsync();
      return RedirectToAction("Events", "Outcast");
    }
    [Authorize]
    [HttpGet]
    public ActionResult CreateMember(string id = null)
    {
      if (id == null)
      {
        var userId = User.Identity.GetUserId();
        Member member = new Member();
        member.memberId = userId;
        return View(member);
      }
      else
      {
        Member member = new Member();
        member.memberId = id;
        return View(member);
      }
    }
    [Authorize]
    [HttpPost]
    public async Task<ActionResult> CreateMember(Member member, HttpPostedFileBase ProfileImage = null)
    {
      if (!ModelState.IsValid)
      {
        ModelState.AddModelError("", "Please ensure all data is valid");
        return View();
      }

      Member dbentry = new Member();
      dbentry.Name = member.Name;
      dbentry.Username = member.Username;
      dbentry.Bio = member.Bio;
      dbentry.VehicleMake = member.VehicleMake;
      dbentry.VehicleModel = member.VehicleModel;
      dbentry.VehicleYear = member.VehicleYear;
      dbentry.memberId = member.memberId;

      var userManager = this.UserManager;
      var user = await UserManager.FindByIdAsync(member.memberId);
      user.MemberId = dbentry.memberId;
      if (member.memberId == null)
      {
        user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
        user.MemberId = dbentry.memberId;
      }

      string photos = Path.Combine(Server.MapPath("~/Content/Photos/0"));
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
        dbentry.ProfileImageType = ProfileImage.ContentType;
        dbentry.ProfileImageName = Path.GetFileName(ProfileImage.FileName);

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
        img.Save(Path.Combine(photos, dbentry.ProfileImageName));

        img.Resize(128, 128);
        img.Save(Path.Combine(thumbnailFolder, dbentry.ProfileImageName));
        _db.Members.Add(dbentry);
        await _db.SaveChangesAsync();
        await UserManager.UpdateAsync(user);
      }
      return Json(new { Success = true, Message = $"Member #{dbentry.Name} Created" });
    }

    public async Task<ActionResult> GetProductByName(string Username)
    {
      var member =
        await _db.Members
        .Select(x => new { x.memberId, x.Username })
        .FirstOrDefaultAsync(x => x.Username == Username);

      return Json(member);
    }
    [Authorize(Roles = "Admin")]
    [HttpGet]
    public ActionResult DeleteMember()
    {
      List<Member> MemberList = _db.Members.ToList();

      var model = new MemberViewModel
      {
        Members = MemberList
      };

      return View(model);
    }
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult> DeleteMember(string memberId)
    {
      Member deleteMember = _db.Members.SingleOrDefault(x => x.memberId.Equals(memberId));

      _db.Members.Remove(deleteMember);
      await _db.SaveChangesAsync();

      return RedirectToAction("Members", "Outcast", null);
    }
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult> CreateEvent(HttpPostedFileBase file, Event newEvent)
    {
      if (!ModelState.IsValid)
      {
        return View(newEvent);
      }

      string EventDocFolder = Path.Combine(Server.MapPath("~/Content/Events"), newEvent.EventId.ToString());

      if (!Directory.Exists(EventDocFolder))
      {
        Directory.CreateDirectory(EventDocFolder);
      }

      if (file != null)
      {
        newEvent.PdfFileName = file.FileName;
        newEvent.PdfFileType = file.ContentType;
        if (Path.GetExtension(file.FileName).ToLower() != ".pdf")
        {
          throw new Exception("File must .pdf format");
        }
        if (file.ContentLength > (16 * 1024 * 1024))
        {
          throw new Exception("Resume PDF is too big!");
        }
        file.SaveAs(Path.Combine(EventDocFolder, file.FileName));
      }
      _db.Events.Add(newEvent);
      await _db.SaveChangesAsync();
      TempData["message"] = $"Event {newEvent.Title} created";
      return RedirectToAction("Events", "Outcast");
    }
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteMessage(Guid id)
    {
      Message message = await _db.Messages.SingleOrDefaultAsync(x => x.Id == id);

      _db.Messages.Remove(message);
      await _db.SaveChangesAsync();
      return Json(null);
    }


    private AppUserManager UserManager
    {
      get { return HttpContext.GetOwinContext().GetUserManager<AppUserManager>(); }
    }
  }
}