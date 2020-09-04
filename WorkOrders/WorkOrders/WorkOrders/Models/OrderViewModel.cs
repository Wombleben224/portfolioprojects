using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WorkOrders.HtmlHelpers;

namespace WorkOrders.Models
{
  public class OrderViewModel
  {
    public List<Order> orders { get; set; }
    public List<Part> parts { get; set; }
    public PagingInfo PagingInfo { get; set; }
    public string Query { get; set; }
  }
}