using FluentScheduler;
using Microsoft.AspNet.Identity.Owin;
using Outcast_CC.Models;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;

namespace Outcast_CC.Jobs
{
  public class DailyNewsLetter : IJob, IRegisteredObject
  {
    public DailyNewsLetter()
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

      var from = new EmailAddress("wombleben224@gmail.com", "Ben Womble");
      var subject =
          "OutCast Daily Reminder";
      var plainTextContent =
          "Dont Forget to check for new messages in the chat!.\n\n\n\n{unsubscribe}";
      var htmlContent =
          "<strong>Dont Forget to check for new messages in the chat!</strong>" +
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
          users.Select(x => new EmailAddress(x.Email, x.UserName)).ToList(),
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