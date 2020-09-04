using FluentScheduler;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace Outcast_CC.Jobs
{
  public class JobRegistry : Registry
  {
    public JobRegistry()
    {
      //Schedule<WeeklyNewsletter>().ToRunNow();
      Schedule<WeeklyNewsletter>().ToRunEvery(1).Weeks().On(DayOfWeek.Monday).At(9, 0);
      Schedule<DailyNewsLetter>().ToRunEvery(1).Days().At(9, 0);
      Debug.WriteLine("JobRegistry Started");
    }
  }
}