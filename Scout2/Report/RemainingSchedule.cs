using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Scout2.Report {
   public class LegislativeEvent {
      public LegislativeEvent(bool a, string b, string c) { IsRange=a;  EventDate=b; EventWhat=c; }
      public bool   IsRange   { get; private set; } // true if EventDate is really a range, such as "Aug. 17 – 31"
      public string EventDate { get; private set; }
      public string EventWhat { get; private set; }
   }

   class RemainingSchedule {
      private static readonly LegislativeEvent[] events = {
         new LegislativeEvent(false, "Jan. 1, 2020", "Statutes take effect(Art.IV, Sec. 8(c)).Jan. 6 Legislature Reconvenes(J.R. 51(a)(4)). "),
         new LegislativeEvent(false, "Jan. 10, 2020", "Budget must be submitted by Governor(Art.IV, Sec. 12(a)). "),
         new LegislativeEvent(false, "Jan. 17, 2020", "Last day for policy committees to hear and report to fiscal committees fiscal bills introduced in their house in the odd-numbered year(J.R. 61(b)(1))."),
         new LegislativeEvent(false, "Jan. 20, 2020", "Martin Luther King, Jr.Day.Jan. 24 Last day for any committee to hear and report to the floor bills introduced in that house in the odd-numbered year(J.R. 61(b)(2)).Last day to submit bill requests to the Office of Legislative Counsel."),
         // Replaced apostrophe
         new LegislativeEvent(false, "Jan. 31, 2020", "Last day for each house to pass bills introduced in that house in the odd-numbered year(Art.IV, Sec. 10(c)), (J.R. 61(b)(3)).Feb. 17 Presidents' Day.Feb. 21 Last day for bills to be introduced(J.R. 61(b)(4)), (J.R. 54(a))."),
         new LegislativeEvent(false, "Mar. 27, 2020", "Cesar Chavez Day observed"),
         // Replaced apostrophe
         new LegislativeEvent(false, "Apr. 2, 2020",  "Spring Recess begins upon adjournment of this day's session(J.R. 51(b)(1)). "),
         new LegislativeEvent(false, "Apr. 13, 2020", "Legislature reconvenes from Spring Recess (J.R. 51(b)(1)). "),
         new LegislativeEvent(false, "Apr. 24, 2020", "Last day for policy committees to hear and report to fiscal committees fiscal bills introduced in their house(J.R. 61(b)(5))."),
         new LegislativeEvent(false, "May 1, 2020",   "Last day for policy committees to hear and report to the floor nonfiscal bills introduced in their house(J.R. 61(b)(6))."),
         new LegislativeEvent(false, "May 8, 2020",   "Last day for policy committees to meet prior to June 1(J.R. 61(b)(7))."),
         new LegislativeEvent(false, "May 15, 2020",  "Last day for fiscal committees to hear and report to the floor bills introduced in their house(J.R. 61(b)(8)).Last day for fiscal committees to meet prior to June 1(J.R. 61(b)(9))."),
         new LegislativeEvent(false, "May 25, 2020",  "Memorial Day May 26 -29 Floor Session Only.No committees, other than conference or Rules Committees, may meet for any purpose (J.R. 61(b)(10)). "),
         new LegislativeEvent(false, "May 29, 2020",  "Last day for each house to pass bills introduced in that house(J.R. 61(b)(11))."),
         new LegislativeEvent(false, "June 1, 2020",  "Committee meetings may resume(J.R. 61(b)(12))."),
         new LegislativeEvent(false, "June 15, 2020", "Budget Bill must be passed by midnight(Art.IV, Sec. 12(c)(3))."),
         new LegislativeEvent(false, "June 25, 2020", "Last day for a legislative measure to qualify for the November 3 General Election ballot(Election code Sec. 9040)."),
         new LegislativeEvent(false, "June 26, 2020", "Last day for policy committees"),
         new LegislativeEvent(false, "July 2, 2020",  "Last day for policy committees to meet and report bills(J.R. 61(b)(14)).Summer Recess begins upon adjournment provided Budget Bill has been passed(J.R. 51(b)(2))."),
         new LegislativeEvent(false, "July 3, 2020",  "Independence Day observed."),
         new LegislativeEvent(false, "Aug. 3, 2020",  "Legislature reconvenes from Summer Recess (J.R. 51(b)(2))."),
         new LegislativeEvent(false, "Aug. 14, 2020", "Last day for fiscal committees to meet and report bills(J.R. 61(b)(15))."),
         // Had to type the dash -- from the website the dash is unicode
         new LegislativeEvent(true,  "Aug. 17 - 31, 2020", "Floor Session only.No committees, other than conference and Rules committees, may meet for any purpose (J.R. 61(b)(16)). "),
         new LegislativeEvent(false, "Aug. 21, 2020", "Last day to amend bills on the Floor(J.R. 61(b)(17))."),
         new LegislativeEvent(false, "Aug. 31, 2020", "Last day for each house to pass bills(Art.IV, Sec. 10(c),"),
         // Replaced apostrophe
         new LegislativeEvent(false, "Sept. 30, 2020","Last day for Governor to sign or veto bills passed by the Legislature before Sept. 1 and in the Governor's possession on or after Sept. 1(Art.IV, Sec. 10(b)(2))."),
         new LegislativeEvent(false, "Nov. 3, 2020",  "General Election "),
         new LegislativeEvent(false, "Nov. 30, 2020", "Adjournment Sine Die at midnight(Art.IV, Sec. 3(a))."),
         new LegislativeEvent(false, "Dec. 7, 2020",  "12 pm. convening of 2021-22 Regular Session(Art.IV, Sec. 3(a)). "),
         new LegislativeEvent(false, "Jan. 1, 2021",  "Statutes take effect(Art.IV, Sec. 8(c)).")
      };
      /// <summary>
      /// Answer an HTML string showing the remaining legislative events for the year.
      /// </summary>
      /// <returns></returns>
      public static string AsString() {
         var sb = new StringBuilder();
         sb.Append("   <table border=\"1\">");
         var date_first_event = DateTime.Parse(events[0].EventDate);
         sb.Append($"      <caption>Remaining Legislative Schedule, {date_first_event.ToString("yyyy")}</caption>");
         sb.Append("      <tr>");
         sb.Append("         <th>Date</th>");
         sb.Append("         <th>Event</th>");
         sb.Append("      </tr>");

         DateTime starting_date = StartingDate();     // This report starts on the previous Monday
         foreach (var row in events) {
            if (row.IsRange) {
               // Append an event that occurs over a range of dates
               DateRange(row, out DateTime range_start_date, out DateTime range_end_date);
               sb.Append($"      <tr><td>{row.EventDate}</td><td>{row.EventWhat}</td></tr>");
            } else {
               // Append an event that occurs on a single date
               if (DateOf(row) >= starting_date) {
                  sb.Append($"      <tr><td>{row.EventDate}</td><td>{row.EventWhat}</td></tr>");
               }
            }
         }

         sb.Append("   </table>");
         sb.Append("   <br />");
         sb.Append("   <br />");
         return sb.ToString();
      }
      /// <summary>
      /// The reported list of legislature events includes the previous week and includes all events from
      /// that date through the end of the year.  There is one caveat: to assist development, if todays date is later
      /// than November 1, then Jan 1 of the following year is used as the starting date.
      /// </summary>
      /// <returns></returns>
      private static DateTime StartingDate() {
         DateTime result = DateTime.Now;
         if (result.Month == 11 || result.Month == 12) {
            // Special case: Nov and Dec rolled forward to first of next year
            int next_year = result.Year + 1;
            result = new DateTime(next_year, 1, 1);
         } else {
            // Normal case
            result = PreviousMonday();
         }
         return result;
      }
      /// <summary>
      /// Answer the date which is the Monday previous to the passed date.
      /// </summary>
      /// <returns></returns>
      private static DateTime PreviousMonday() {
         DateTime now = DateTime.Now;
         int adjustment = 0;
         switch (now.DayOfWeek) {
            case DayOfWeek.Sunday:    adjustment = -6; break;
            case DayOfWeek.Saturday:  adjustment = -5; break;
            case DayOfWeek.Friday:    adjustment = -4; break;
            case DayOfWeek.Thursday:  adjustment = -3; break;
            case DayOfWeek.Wednesday: adjustment = -2; break;
            case DayOfWeek.Tuesday:   adjustment = -1; break;
            default: /*Monday*/       adjustment = -7; break;
         }
         return now.AddDays(adjustment);
      }
      /// <summary>
      /// Ensure event date is canonical in that "Sept" is replaced by "Seep"
      /// </summary>
      /// <param name="row"></param>
      private static string CanonicalDate(string event_date) {
         string parse_this = event_date;
         // Legislature uses "Sept", DateTime parser recognizes "Sep"
         if (parse_this.Contains("Sept")) parse_this = parse_this.Replace("Sept", "Sep");
         return parse_this;
      }
      /// <summary>
      /// Answer the date of the passed legislative event
      /// </summary>
      /// <param name="row"></param>
      private static DateTime DateOf(LegislativeEvent row) {
         if (DateTime.TryParse(CanonicalDate(row.EventDate), out DateTime result)) {
            return result;
         } else {
            throw new ApplicationException($"RemainingSchedule.DateOf: {row.EventDate} is not a valid date.");
         }
      }
      /// <summary>
      /// Answer the starting and ending dates of the passed legislative event when that event occurs over
      /// a range of dates.
      /// </summary>
      /// <param name="row">The LegislativeEvent row defining the event</param>
      /// <param name="lhs_start">The starting date of the event's range of dates</param>
      /// <param name="rhs_end">The ending date of the event's range of dates</param>
      private static void DateRange(LegislativeEvent row, out DateTime lhs_start, out DateTime rhs_end) {
         // Example event date with range: "Aug. 17 – 31, 2020";
         // Extract the year from the input string and then trim it from the input string.
         string s0 = row.EventDate;
         string year = Regex.Replace(s0, @".*?(\d+)$", "$1");
         if (!Regex.Match(year, @"\d{4}").Success) 
            throw new ApplicationException($"RemainingSchedule.DateRange: {row.EventDate} has invalid date.");
         string s1 = Regex.Replace(s0, "(.*?),.*", "$1");

         // Extract the last day from the input string and then trim it from the input string.
         string last_day = Regex.Replace(s1, @".*?(\d+)$", "$1");
         if (!Regex.Match(last_day, @"\d{2}").Success)
            throw new ApplicationException($"RemainingSchedule.DateRange: {row.EventDate} has invalid ending day.");
         string s2 = s1.Substring(0, s1.Length-last_day.Length-2); // 2 is for dash and preceding blank

         // Starting date
         if (DateTime.TryParse(s2, out DateTime lhs_month_day)) {
            lhs_start = new DateTime(Convert.ToInt16(year), lhs_month_day.Month, lhs_month_day.Day);
            // Ending date
            // TODO Code assumes start and end days are the same month and year
            rhs_end = new DateTime(Convert.ToInt16(year), lhs_month_day.Month, Convert.ToInt16(last_day));
         } else {
            throw new ApplicationException($"RemainingSchedule.DateRange: start of {row.EventDate} is not a valid date.");
         }
      }
   }
}
