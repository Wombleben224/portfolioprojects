using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using WorkOrders.HtmlHelpers;

namespace WorkOrders.Models
{
  [Table("WorkOrders")]
  public class WorkOrder
  {
    [Key]
    [Column(Order = 0)]
    public int? CustomerID { get; set; }

    [Key]
    [Column(Order = 0)]
    public int? OrderID { get; set; }

    [Key]
    [Column(Order = 0)]
    public int? PartsID { get; set; }

    public PagingInfo PagingInfo { get; set; }

    public virtual Customer Customer { get; set; }
    public virtual Order Order { get; set; }
    public virtual Part Part { get; set; }
  }
}