using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkOrders.Models
{
  public class ImportViewModel
  {
    public List<Order> orders { get; set; }
    public List<Customer> customers { get; set; }
  }
}