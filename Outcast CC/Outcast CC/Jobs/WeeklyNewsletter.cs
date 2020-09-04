using FluentScheduler;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Outcast_CC.Models;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Web.Mvc;
using Microsoft.Owin;
using Microsoft.AspNetCore.Identity;
using System.Web;

namespace Outcast_CC.Jobs
{
  public class WeeklyNewsletter : IJob, IRegisteredObject
  {
    public WeeklyNewsletter()
    {
      HostingEnvironment.RegisterObject(this);
    }

    public void Stop(bool immediate)
    {
      HostingEnvironment.UnregisterObject(this);
    }

    public void Execute()
    {
      try
      {
        Task task = DoWork();
        task.Wait();
      }
      catch (Exception ex)
      {
        Debug.WriteLine(ex.ToString());
      }
      finally
      {
        HostingEnvironment.UnregisterObject(this);
      }
    }

    private async Task DoWork()
    {
      var userManager = UserManager;
      var users = userManager.Users.ToList();

      var apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
      var client = new SendGridClient(apiKey);

      var from = new EmailAddress("Wombleben224@gmail.com", "Ben Womble");
      var subject =
          "OutCast Weekly Reminder";
      var plainTextContent =
          "Come see if there are any new events posted this week!.\n\n\n\n{unsubscribe}";
      var htmlContent =
          "<strong>Come see if there are any new events posted this week!</strong>" +
          "<br><br><a href='{unsubscribe}'>Unsubscribe</a>";

      var substitutions =
        users.Select(x => new Dictionary<string, string>() {
                            { "{name}", x.UserName },
                            { "{email}", x.Email },
                            { "{unsubscribe}", x.UnsubscribeUrl },
                   })
                   .ToList();

      var msg = MailHelper.CreateMultipleEmailsToMultipleRecipients(
          from,
          users.Select(x => new EmailAddress(x.UserName, x.Email)).ToList(),
          users.Select(x => subject).ToList(),
          plainTextContent,
          htmlContent,
          substitutions);
      await client.SendEmailAsync(msg);

      var result = await client.SendEmailAsync(msg);
      if (result.StatusCode != HttpStatusCode.Accepted)
      {
        throw new Exception($"Failed to send email: {result.StatusCode}");
      }

      Debug.WriteLine("Newsletter Sent");
    }
    private AppUserManager UserManager
    {
      get { return HttpContext.Current.GetOwinContext().GetUserManager<AppUserManager>(); }
    }
  }
}