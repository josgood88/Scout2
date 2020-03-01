using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Library;
using Library.Database;
using Scout2.Sequence;

namespace Scout2.IndividualReport {
   public class CreateIndividualReport {
      // Create a Bill Report, given a bill identifier such as AB12
      public static List<string> ReportContents(BillRow row, string path) {
         List<string> result = new List<string>();
         string name_ext = Path.GetFileName(row.Lob);                   // BillVersionTable bill_xml is unique
         BillVersionRow bv_row = GlobalData.VersionTable.Scalar(name_ext);
         List<BillHistoryRow> history = GlobalData.HistoryTable.RowSet(bv_row.BillID);
         var location_code = history.First().TernaryLocation;
         var location_code_row = GlobalData.LocationTable.Scalar(location_code);

         string appropriation = bv_row.Appropriation;
         string author        = row.Author;
         string bill_id       = row.Bill;
         string fiscal        = bv_row.FiscalCommittee;
         string house         = history.First().PrimaryLocation;
         string last_action   = CreateNewReports.FindLastAction(row);
         // TODO location is Desk if "Read first time.  To print." or "From printer.  May be heard in committee"
         string location      = location_code_row == null ? string.Empty : location_code_row.Description;
         string local_pgm     = bv_row.LocalProgram;
         string number        = row.MeasureNum.TrimStart('0');
         string title         = row.Title;
         string type_house    = $"{bill_id.First()}B";
         string vers_id       = row.BillVersionID;
         string vote          = bv_row.VoteRequired;

         // These data come from the previous version of the bill report
         var summary = PreviousReport.Summary(path);
         var position = PreviousReport.Position(path);
         var shortsummary = PreviousReport.ShortSummary(path);
         var committees = PreviousReport.Committees(path);
         var likelihood = PreviousReport.Likelihood(path);

         // With all necessary data obtained, generate the report file template.  This sets things up for entering the report manually.
         result.Add("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">");
         result.Add("<html xmlns=\"http://www.w3.org/1999/xhtml\" > ");
         result.Add("<head>");
         result.Add($"   <title> {type_house}-{number} ({author}) {title}</title>");
         result.Add("</head>");
         result.Add("<body>");
         result.Add("<p>");
         result.Add($"<b>Title</b>: {type_house}-{number} ({author}) {title}");
         result.Add("</p>");

         // Review
         if (summary.Count > 0) {
            foreach (var line in summary) result.Add(line);
         } else {
            result.Add("<p>");
            result.Add($"<b>Summary</b>: (Reviewed {DateTime.Now.ToShortDateString()})");
            result.Add("   <br /> (Quotations taken directly from the bill's language, or from current code)");
            result.Add("   <br />");
            result.Add("   <br /> This is my review");
            result.Add("</p>");
         }

         // Position
         if (position.Count > 0) {
            foreach (var line in position) result.Add(line);
         } else {
            if (summary.Count == 0) result.Add("<p>");
            result.Add("   <b>Position</b>: ");
            result.Add("   <br /> This is my reason.");
            result.Add("</p>");
         }

         // Short Summary, Committees and Likelihood
         result.Add($"<b>ShortSummary</b>: {shortsummary}");
         result.Add($"<br /><b>Committees</b>: {committees}");
         result.Add($"<br /><b>Likelihood</b>: {likelihood}");

         // Status, Location, etc
         result.Add("<p>");
         result.Add("<b>Status</b>:");
         // TODO location is blank when AB1275 Sep 14 2019 Ordered to inactive file at the request of Senator Bradford
         result.Add($"<br /> Location: {location}");
         string str_date = String.Empty;
         if (history.Count > 0) str_date = history.First().ActionDate;
         result.Add("<table cellspacing=\"0\" cellpadding=\"0\">");
         result.Add($"   <tr><td> Last Action: {FormatDate(str_date)} </td><td> &nbsp; &nbsp; {last_action}</td></tr>");
         result.Add($"   <tr><td> Vote: {vote}                        </td><td> &nbsp; &nbsp; Appropriation: {appropriation}</td></tr>");
         result.Add($"   <tr><td> Fiscal committee: {fiscal}          </td><td> &nbsp; &nbsp; State-mandated local program: {local_pgm} </td></tr>");
         result.Add("</table>");
         result.Add("</p>");

         // Bill History
         result.Add("<p>");
         result.Add("   <b>Bill History</b>:");
         foreach (var item in history) {
            result.Add($"   <br /> {FormatDate(item.ActionDate)} {item.Action}");
         }
         result.Add("</p>");
         result.Add("</body>");
         result.Add("</html>");
         return result;
      }

      private static string FormatDate(string str_date) {
         string result = string.Empty;
         DateTime date_result = default(DateTime);
         if (DateTime.TryParse(str_date,out date_result))
            result = $"{date_result.ToString("MMM d yyyy")}";
         else
            result = "(invalid date)";
         return result;
      }
   }
}
