using Outcast_CC.HtmlHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Outcast_CC.Models
{
  public class EventViewModel
  {
    public List<Event> Events { get; set; }
    public PagingInfo pagingInfo { get; set; }
    public string Query { get; set; }
    public List<int> EventId { get; set; }
  }
}