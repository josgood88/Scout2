using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Scout2.Utility;

namespace Scout2.Report {
   class RemainingSchedule {
      private static DateTime last_monday = default(DateTime);
      private static StringBuilder sb = new StringBuilder();

      public static string AsString() {
         last_monday = MiscUtils.LastMonday();
         sb = new StringBuilder();
         sb.Append("   <table border=\"1\">");
         sb.Append($"      <caption>Remaining Legislative Schedule, {DateTime.Now.ToString("yyyy")}</caption>");
         sb.Append("      <tr>");
         sb.Append("         <th>Date</th>");
         sb.Append("         <th>Event</th>");
         sb.Append("      </tr>");
         Append_If("      <tr><td>Jan. 1  </td><td>Statutes take effect(Art.IV, Sec. 8(c)).</td></tr>");
         Append_If("      <tr><td>Jan. 6  </td><td>Legislature Reconvenes(J.R. 51(a)(4)). </td></tr>");
         Append_If("      <tr><td>Jan. 10 </td><td>Budget must be submitted by Governor(Art.IV, Sec. 12(a)). </td></tr>");
         Append_If("      <tr><td>Jan. 17 </td><td>Last day for policy committees to hear and report to fiscal committees fiscal bills introduced in their house in the odd-numbered year(J.R. 61(b)(1)).</td></tr>");
         Append_If("      <tr><td>Jan. 20 </td><td>Martin Luther King, Jr.System.Windows.Forms.Day.</td></tr>");
         Append_If("      <tr><td>Jan. 24 </td><td>Last day for any committee to hear and report to the floor bills introduced in that house in the odd-numbered year(J.R. 61(b)(2)).Last day to submit bill requests to the Office of Legislative Counsel.</td></tr>");
         Append_If("      <tr><td>Jan. 31 </td><td>Last day for each house to pass bills introduced in that house in the odd-numbered year(Art.IV, Sec. 10(c)), (J.R. 61(b)(3)).</td></tr>");
         Append_If("      <tr><td>Feb. 17 </td><td>Presidents' System.Windows.Forms.Day.</td></tr>");
         Append_If("      <tr><td>Feb. 21 </td><td>Last day for bills to be introduced(J.R. 61(b)(4)), (J.R. 54(a)).</td></tr>");
         Append_If("      <tr><td>Mar. 27 </td><td>Cesar Chavez Day observed</td></tr>");
         Append_If("      <tr><td>Apr. 2  </td><td>Spring Recess begins upon adjournment of this day's session(J.R. 51(b)(1)). </td></tr>");
         Append_If("      <tr><td>Apr. 13 </td><td>Legislature reconvenes from Spring Recess (J.R. 51(b)(1)). </td></tr>");
         Append_If("      <tr><td>Apr. 24 </td><td>Last day for policy committees to hear and report to fiscal committees fiscal bills introduced in their house(J.R. 61(b)(5)).</td></tr>");
         Append_If("      <tr><td>May 1   </td><td>Last day for policy committees to hear and report to the floor nonfiscal bills introduced in their house(J.R. 61(b)(6)).</td></tr>");
         Append_If("      <tr><td>May 8   </td><td>Last day for policy committees to meet prior to June 1(J.R. 61(b)(7)).</td></tr>");
         Append_If("      <tr><td>May 15  </td><td>Last day for fiscal committees to hear and report to the floor bills introduced in their house(J.R. 61(b)(8)).Last day for fiscal committees to meet prior to June 1(J.R. 61(b)(9)).</td></tr>");
         Append_If("      <tr><td>May 25  </td><td>Memorial Day May 26 -29 Floor Session Only.No committees, other than conference or Rules Committees, may meet for any purpose (J.R. 61(b)(10)). </td></tr>");
         Append_If("      <tr><td>May 29  </td><td>Last day for each house to pass bills introduced in that house(J.R. 61(b)(11)).</td></tr>");
         Append_If("      <tr><td>June 1  </td><td>Committee meetings may resume(J.R. 61(b)(12)).</td></tr>");
         Append_If("      <tr><td>June 15 </td><td>Budget Bill must be passed by midnight(Art.IV, Sec. 12(c)(3)).</td></tr>");
         Append_If("      <tr><td>June 25 </td><td>Last day for a legislative measure to qualify for the November 3 General Election ballot(Election code Sec. 9040).</td></tr>");
         Append_If("      <tr><td>June 26 </td><td>Last day for policy committees to hear and report fiscal bills to fiscal committees(J.R. 61(b)(13)).</td></tr>");
         Append_If("      <tr><td>July 2  </td><td>Last day for policy committees to meet and report bills (J.R. 61(b)(14)).Summer Recess begins upon adjournment provided Budget Bill has been passed(J.R. 51(b)(2)).</td></tr>");
         Append_If("      <tr><td>July 3  </td><td>Independence Day observed.</td></tr>");
         Append_If("      <tr><td>Aug. 3  </td><td>Legislature reconvenes from Summer Recess (J.R. 51(b)(2)).</td></tr>");
         Append_If("      <tr><td>Aug. 14 </td><td>Last day for fiscal committees to meet and report bills (J.R. 61(b)(15)).</td></tr>");
         Append_If("      <tr><td>Aug. 17-31 </td><td>Floor Session only.No committees, other than conference and Rules committees, may meet for any purpose (J.R. 61(b)(16)).</td></tr>");
         Append_If("      <tr><td>Aug. 21  </td><td>Last day to amend bills on the Floor(J.R. 61(b)(17)).</td></tr>");
         Append_If("      <tr><td>Aug. 31  </td><td>Last day for each house to pass bills(Art.IV, Sec. 10(c), (J.R. 61(b)(18)). Final recess begins upon adjournment(J.R. 51(b)(3)).</td></tr>");
         Append_If("      <tr><td>Sept. 30 </td><td>Last day for Governor to sign or veto bills passed by the Legislature before Sept. 1 and in the Governor's possession on or after Sept. 1(Art.IV, Sec. 10(b)(2)).</td></tr>");
         Append_If("      <tr><td>Nov. 3   </td><td>General Election</td></tr>");
         Append_If("      <tr><td>Nov. 30  </td><td>Adjournment Sine Die at midnight(Art.IV, Sec. 3(a)).</td></tr>");
         Append_If("      <tr><td>Dec. 7   </td><td>Convening of 2021-22 Regular Session(Art.IV, Sec. 3(a)).</td></tr>");
         Append_If("      <tr><td>Jan. 1   </td><td>Statutes take effect(Art.IV, Sec. 8(c)).</td></tr>");
         sb.Append("   </table>");
         sb.Append("   <br />");
         sb.Append("   <br />");
         return sb.ToString();
      }

      private static void Append_If(string candidate) {
         string preface = "^\\<tr\\>\\<td\\>";
         string of_interest = Regex.Replace(candidate.Trim(), $"{preface}([^<]+).*", "$1"); // Extract day and month
         of_interest += $" {DateTime.Now.Year.ToString()}";    // Append year
         if (DateTime.TryParse(of_interest, out DateTime event_date)) {
            if (event_date > last_monday) {  // If the candidate date islater than last Monday
               sb.Append(candidate);         // Then add the candidate to the StringBuilder
            }
         }
      }
   }
}
