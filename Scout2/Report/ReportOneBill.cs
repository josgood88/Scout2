using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Text;

namespace Scout2.Report {
   public class ReportOneBill {
      public static void AsString(string bill_name) {
         var sb = new StringBuilder();
         sb.Append("<tr>");
         sb.Append("  <td>SB-12(Beall) Mental health services: youth. </td>");
         sb.Append("  <td>No</td>");
         sb.Append("  <td>No</td>");
         sb.Append("  <td>Oppose</td>");
         sb.Append("  <td>");
         sb.Append("     \"Integrated Youth Mental Health Program\" centers across California. \"A one-stop site for access to integrated care services, including mental health\". Annual appropriation.");
         sb.Append("    <br /> Expectation.No opposition shown.  Likely pass on consent calendar.");
         sb.Append("  </td>");
         sb.Append("  <td>Aug 30 2019 August 30 hearing: Held in Sen Appropriations and under submission</td>");
         sb.Append("</tr>");
      }
   }
}
