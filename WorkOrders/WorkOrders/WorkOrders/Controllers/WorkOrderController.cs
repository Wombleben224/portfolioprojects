using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WorkOrders.HtmlHelpers;
using WorkOrders.Models;

namespace WorkOrders.Controllers
{
  public class WorkOrderController : Controller
  {
    private WorkOrdersDatabase _db = new WorkOrdersDatabase();
    // GET: WorkOrder
    public async Task<ActionResult> Index(
    int page = 1,
    int pagesize = 3,
    int? OrderNum = null,
    string CustName = null,
    DateTime? StartDate = null,
    DateTime? EndDate = null
    )
    {
      List<Order> orders =
        await _db.Orders
          .Where(x => CustName == null || CustName == "" || x.Customer.customername.Contains(CustName))
          .Where(x => OrderNum == null || x.ordernumber == OrderNum)
          .Where(x => StartDate == null || x.repairdate >= StartDate)
          .Where(x => EndDate == null || x.repairdate >= EndDate)
          .OrderByDescending(x => x.Customer.customername)
          .Skip((page - 1) * pagesize)
          .Take(pagesize)
          .ToListAsync();

      int count =
      await _db.Orders
          .Where(x => CustName == null || CustName == "" || x.Customer.customername.Contains(CustName))
          .Where(x => OrderNum == null || x.ordernumber == OrderNum)
          .Where(x => StartDate == null || x.repairdate >= StartDate)
          .Where(x => EndDate == null || x.repairdate >= EndDate)
        .CountAsync();

      var model = new OrderViewModel
      {
        orders = orders,
        PagingInfo = new PagingInfo
        {
          CurrentPage = page,
          ItemsPerPage = pagesize,
          TotalItems = count
        }
      };
      return View("Index", model);
    }

    [HttpGet]
    public ActionResult CreateOrder()
    {
      Order order = new Order();
      order.repairdate = DateTime.Now;

      return View(order);
    }

    [HttpPost]
    public async Task<ActionResult> SubmitOrder(Order order)
    {
      if (!ModelState.IsValid)
      {
        return View("CreateOrder", order);
      }
      else if (order.OrderID == 0)
      {
        _db.Orders.Add(order);
      }
      else
      {

      }

      await _db.SaveChangesAsync();
      TempData["message"] = $"{order.ordernumber} Has Been Inserted";
      return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<ActionResult> ViewOrder(int OrderId)
    {
      Order order = await _db.Orders.SingleOrDefaultAsync(x => x.OrderID == OrderId);

      return View(order);
    }

    [HttpGet]
    public async Task<ActionResult> EditOrder(int OrderId)
    {
      Order order = await _db.Orders.SingleOrDefaultAsync(x => x.OrderID == OrderId);

      return View(order);
    }

    [HttpPost]
    public async Task<ActionResult> EditOrder(Order order)
    {
      Order dbEntry = await _db.Orders.SingleOrDefaultAsync(x => x.OrderID == order.OrderID);
      dbEntry.Customer = order.Customer;
      dbEntry = order;
      await _db.SaveChangesAsync();
      TempData["message"] = $"{order.ordernumber} has been inserted";
      return RedirectToAction("Index");
    }

    [HttpGet]
    public ActionResult AddParts(int OrderId)
    {
      Part part = new Part();

      part.OrderID = OrderId;

      return View(part);
    }
    [HttpPost]
    public async Task<ActionResult> AddParts(Part part)
    {
      Order dbEntry = await _db.Orders.SingleOrDefaultAsync(x => x.OrderID == part.OrderID);
      dbEntry.Parts.Add(part);
      await _db.SaveChangesAsync();
      TempData["Success"] = $"{part.partsname} has been inserted";
      return RedirectToAction("AddParts", new { dbEntry.OrderID });
    }
    public async Task<ActionResult> DeletePart(int partId)
    {
      Part Dbentry = await _db.Parts.SingleOrDefaultAsync(x => x.Partsid == partId);

      _db.Parts.Remove(Dbentry);
      await _db.SaveChangesAsync();

      return Json(new { Success = true, Message = "Message Sent" });
    }
    public async Task<ActionResult> Keywords(string term)
    {
      List<string> names =
        _db.Customers
        .Where(x => x.customername.Contains(term))
        .Select(x => x.customername.ToLower())
        .Distinct()
        .ToList();

      var spelling = new NHunspell.Hunspell("en_US.aff", "en_US.dic");
      var suggestions =
        spelling.Spell(term) ?
        new List<string>() { term } :
        spelling.Suggest(term).Take(5);

      return Json(names, JsonRequestBehavior.AllowGet);
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

    public class ImportedCustomer
    {
      public int RowNumber { get; set; }
      public string customername { get; set; }
      public string PhoneNumber { get; set; }
      public string email { get; set; }
    }

    public class ImportedOrder
    {
      public int RowNumber { get; set; }
      public int ordernumber { get; set; }
      public DateTime repairdate { get; set; }
      public int vehicleyear { get; set; }
      public string vehiclemake { get; set; }
      public string vehiclemodel { get; set; }
      public string vehicleliscenseplate { get; set; }
      public decimal vehiclemileage { get; set; }
      public decimal orderestimate { get; set; }
      public string techname { get; set; }
      public decimal laborhours { get; set; }
      public decimal laborcost { get; set; }
      public decimal labortotals { get; set; }
      public decimal GrandTotal { get; set; }
      public int CustomerID { get; set; }
    }

    private void ImportCustomerData(DataSet dataSet)
    {
      // Verify that there is only one tab in the spreadsheet
      if (dataSet.Tables.Count != 1)
      {
        throw new Exception($"Expected one sheet in Excel file, found {dataSet.Tables.Count}");
      }
      // Get the first tab of the spreadsheet
      DataTable table = dataSet.Tables[0];
      // Verify that the columns are what we expect
      if (table.Columns.Count != 3 ||
      table.Columns[0].ColumnName != "Full Name" ||
      table.Columns[1].ColumnName != "Phone Number" ||
      table.Columns[2].ColumnName != "Email")
      {
        throw new Exception("Expected columns Full Name, Phone Number, Email");
      }
      // Load data from the table into memory
      var importedCustomers = new List<ImportedCustomer>();
      int rowNumber = 1;
      foreach (DataRow row in table.Rows)
      {
        ++rowNumber;
        var customer = new ImportedCustomer
        {
          RowNumber = rowNumber,
          customername = row[0].ToString().Trim(),
          PhoneNumber = row[1].ToString().Trim(),
          email = row[2].ToString().Trim(),
        };
        importedCustomers.Add(customer);
      }
      // Load existing customer data from the database
      var PhoneNumbers = importedCustomers.Select(x => x.PhoneNumber);
      var dbCustomers = _db.Customers.Where(x => PhoneNumbers.Contains(x.PhoneNumber)).ToList();
      var existingCustomers = (
      from c in dbCustomers
      join i in importedCustomers
      on new { c.customername, c.PhoneNumber, c.email }
      equals new { i.customername, i.PhoneNumber, i.email}
      select c
      ).ToList();
      // Filter out existing customers
      var newCustomers =
      from i in importedCustomers
      join e in existingCustomers
      on new { i.customername, i.PhoneNumber, i.email }
      equals new { e.customername, e.PhoneNumber, e.email}
      into g
      where !g.Any()
      select new Customer { customername = i.customername, PhoneNumber = i.PhoneNumber, email = i.email };
      // Save only the new customers

      _db.Customers.AddRange(newCustomers);
      _db.SaveChanges();
    }

    //private void ImportOrderData(DataSet dataSet)
    //{
    //  // Verify that there is only one tab in the spreadsheet
    //  if (dataSet.Tables.Count != 1)
    //  {
    //    throw new Exception($"Expected one sheet in Excel file, found {dataSet.Tables.Count}");
    //  }
    //  // Get the first tab of the spreadsheet
    //  DataTable table = dataSet.Tables[0];
    //  // Verify that the columns are what we expect
    //  if (table.Columns.Count != 3 ||
    //  table.Columns[0].ColumnName != "Full Name" ||
    //  table.Columns[1].ColumnName != "Phone Number" ||
    //  table.Columns[2].ColumnName != "Email" ||
    //  table.Columns[3].ColumnName != "ordernumber" ||
    //  table.Columns[4].ColumnName != "repairdate" ||
    //  table.Columns[5].ColumnName != "vehicleyear" ||
    //  table.Columns[6].ColumnName != "vehiclemake" ||
    //  table.Columns[7].ColumnName != "vehiclemodel" ||
    //  table.Columns[8].ColumnName != "vehicleliscenseplate" ||
    //  table.Columns[9].ColumnName != "vehiclemileage" ||
    //  table.Columns[10].ColumnName != "orderestimate" ||
    //  table.Columns[11].ColumnName != "techname" ||
    //  table.Columns[12].ColumnName != "laborhours" ||
    //  table.Columns[13].ColumnName != "laborcost" ||
    //  table.Columns[14].ColumnName != "labortotals" ||
    //  table.Columns[15].ColumnName != "GrandTotal")
    //  {
    //    throw new Exception("Columns do not match what was expected");
    //  }
    //  // Load data from the table into memory
    //  var importedCustomers = new List<ImportedCustomer>();
    //  var importedOrders = new List<ImportedOrder>();
    //  int rowNumber = 1;
    //  foreach (DataRow row in table.Rows)
    //  {
    //    ++rowNumber;
    //    var customer = new ImportedCustomer
    //    {
    //      RowNumber = rowNumber,
    //      customername = row[0].ToString().Trim(),
    //      PhoneNumber = row[1].ToString().Trim(),
    //      email = row[2].ToString().Trim(),
    //    };
    //    var order = new ImportedOrder
    //    {
    //      RowNumber = rowNumber,
    //      ordernumber = int.Parse(row[3].ToString().Trim()),
    //      repairdate = DateTime.Parse(row[4].ToString().Trim()),
    //      vehicleyear = int.Parse(row[5].ToString().Trim()),
    //      vehiclemake = row[6].ToString().Trim(),
    //      vehiclemodel = row[7].ToString().Trim(),
    //      vehicleliscenseplate = row[8].ToString().Trim(),
    //      vehiclemileage = decimal.Parse(row[9].ToString().Trim()),
    //      orderestimate = decimal.Parse(row[10].ToString().Trim()),
    //      techname = row[11].ToString().Trim(),
    //      laborhours = decimal.Parse(row[12].ToString().Trim()),
    //      laborcost = decimal.Parse(row[13].ToString().Trim()),
    //      labortotals = decimal.Parse(row[14].ToString().Trim()),
    //      GrandTotal = decimal.Parse(row[15].ToString().Trim())
    //    };
    //    importedCustomers.Add(customer);
    //    importedOrders.Add(order);
    //  }
    //  // Load existing customer data from the database
    //  var PhoneNumbers = importedCustomers.Select(x => x.PhoneNumber);
    //  var dbCustomers = _db.Customers.Where(x => PhoneNumbers.Contains(x.PhoneNumber)).ToList();
    //  var existingCustomers = (
    //  from c in dbCustomers
    //  join i in importedCustomers
    //  on new { c.customername, c.PhoneNumber, c.email }
    //  equals new { i.customername, i.PhoneNumber, i.email }
    //  select c
    //  ).ToList();
    //  // Filter out existing customers
    //  var newCustomers =
    //  from i in importedCustomers
    //  join e in existingCustomers
    //  on new { i.customername, i.PhoneNumber, i.email }
    //  equals new { e.customername, e.PhoneNumber, e.email }
    //  into g
    //  where !g.Any()
    //  select new Customer { customername = i.customername, PhoneNumber = i.PhoneNumber, email = i.email };
    //  // Save only the new customers

    //  _db.Customers.AddRange(newCustomers);
    //  _db.SaveChanges();

    //  // Load existing order data from the database
    //  var OrderNumber = importedOrders.Select(x => x.ordernumber);
    //  var dbOrders = _db.Orders.Where(x => OrderNumber.Contains(x.ordernumber)).ToList();
    //  var existingOrders = (
    //  from o in dbOrders
    //  join i in importedOrders
    //  on new { o.ordernumber, o.repairdate, o.vehicleyear, o.vehiclemake, o.vehiclemodel,
    //                  o.vehicleliscenseplate, o.vehiclemileage, o.orderestimate, o.techname,
    //                  o.laborhours, o.laborcost, o.labortotals, o.GrandTotal}
    //  equals new {i.ordernumber, i.repairdate, i.vehicleyear, i.vehiclemake, i.vehiclemodel,
    //                  i.vehicleliscenseplate, i.vehiclemileage, i.orderestimate, i.techname,
    //                  i.laborhours, i.laborcost, i.labortotals, i.GrandTotal}
    //  select o
    //  ).ToList();
    //  // Filter out existing customers
    //  var newOrders =
    //  from o in importedOrders
    //  join i in existingOrders
    //  on new { o.ordernumber, o.repairdate, o.vehicleyear, o.vehiclemake, o.vehiclemodel,
    //                  o.vehicleliscenseplate, o.vehiclemileage, o.orderestimate, o.techname,
    //                  o.laborhours, o.laborcost, o.labortotals, o.GrandTotal}
    //  equals new {i.ordernumber, i.repairdate, i.vehicleyear, i.vehiclemake, i.vehiclemodel,
    //                  i.vehicleliscenseplate, i.vehiclemileage, i.orderestimate, i.techname,
    //                  i.laborhours, i.laborcost, i.labortotals, i.GrandTotal}
    //  into g
    //  where !g.Any()
    //  select new Customer { customername = o.customername, PhoneNumber = o.PhoneNumber, email = o.email };
    //  // Save only the new customers

    //  _db.Customers.AddRange(newCustomers);
    //  _db.SaveChanges();

      //new ImportViewModel
      //{
      //  customers = newCustomers.ToList(),

      //};

  }
}