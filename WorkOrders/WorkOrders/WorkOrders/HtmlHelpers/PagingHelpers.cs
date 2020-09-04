using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace WorkOrders.HtmlHelpers
{
    public class PagingInfo
    {
      public int TotalItems { get; set; }
      public int ItemsPerPage { get; set; }
      public int CurrentPage { get; set; }

      public int TotalPages
      {
        // Use this formula, instead of the one found in the book
        // This is the correct way to perform rounding with integer division
        get { return (TotalItems + ItemsPerPage - 1) / ItemsPerPage; }
      }
    }

    public delegate string PageUrlFunction(int page);

    public static class PagingHelpers
    {
      public static MvcHtmlString PageLinks(
        this HtmlHelper html,
        PagingInfo pagingInfo,
        PageUrlFunction pageUrl)
      {
        var result = new StringBuilder();
        result.Append(@"<nav><ul class=""pagination"">");

        for (int i = 1, n = pagingInfo.TotalPages; i <= n; i++)
        {
          var a = new TagBuilder("a");
          a.InnerHtml = i.ToString();
          a.MergeAttribute("href", pageUrl(i));
          a.AddCssClass("page-link");

          var li = new TagBuilder("li");
          li.InnerHtml = a.ToString();
          li.AddCssClass(i == pagingInfo.CurrentPage ? "page-item active" : "page-item");
          result.Append(li.ToString());
        }

        result.Append(@"</ul></nav>");
        return MvcHtmlString.Create(result.ToString());
      }
    }
  }