using System;
using System.Collections.Generic;
using System.Text;

namespace Scout2.Report {
   class RemainingSchedule {
      public static string AsString() {
         var sb = new StringBuilder();
         sb.Append("   <table border=\"1\">");
         sb.Append($"      <caption>Remaining Legislative Schedule, {DateTime.Now.ToString("yyyy")}</caption>");
         sb.Append("      <tr>");
         sb.Append("         <th>Date</th>");
         sb.Append("         <th>Event</th>");
         sb.Append("      </tr>");
         sb.Append("      <tr><td>Sept. 3-13 </td><td>Floor session only. No committee may meet for any purpose, except Rules Committee, bills referred pursuant to Assembly Rule 77.2, and Conference Committees (J.R. 61(a)(13)).</td></tr>");
         sb.Append("      <tr><td>Sept.  6 </td><td>Last day to amend on floor (J.R. 61(a)(14)).</td></tr>");
         sb.Append("      <tr><td>Sept. 13 </td><td>Last day for any bill to be passed (J.R. 61(a)(15)). Interim Recess begins upon adjournment (J.R. 51(a)(4)).</td></tr>");
         sb.Append("      <tr><td>Oct.  13 </td><td>Last day for Governor to sign or veto bills passed by the Legislature on or before Sept. 13 and in the Governor's possession after Sept. 13 (Art. IV, Sec. 10(b)(1)).</td></tr>");
         sb.Append("      <tr><td>Jan.   1 </td><td>Statutes take effect (Art. IV, Sec. 8(c)).</td></tr>");
         sb.Append("      <tr><td>Jan.   6 </td><td>Legislature reconvenes (J.R. 51(a)(4)).</td></tr>");
         sb.Append("   </table>");
         sb.Append("   <br />");
         sb.Append("   <br />");
         return sb.ToString();
      }
   }
}
