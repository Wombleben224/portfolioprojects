using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Outcast_CC.HtmlHelpers;

namespace Outcast_CC.Models
{
  public class MemberViewModel
  {
    public List<Member> Members { get; set; }
    public PagingInfo pagingInfo { get; set; }
    public string Query { get; set; }
  }
}