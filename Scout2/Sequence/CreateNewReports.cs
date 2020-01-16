using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Library;
using Library.Database;
using Scout2.Utility;

namespace Scout2.Sequence {
   public class CreateNewReports : BaseController {
      ///
      /// Give the user an opportunity to create new reports
      /// for those bills having the highest scores.
      public void Run(Form1 form1, UnreportedBillsForm upreported_form) {
         var start_time = DateTime.Now;
         try {
            LogAndDisplay(form1.txtCreatesProgress, "Showing bills that have no report.");
            var bills_with_no_position = CollectNoPositionBills();
            var bills_for_display = Convert(bills_with_no_position);
            // Display those bills that have no report, or else a MessageBox saying all bills have a report.
            if (bills_for_display.Any()) {
               upreported_form.PrepareDataGridView();
               upreported_form.AddRows(bills_for_display);
               upreported_form.ShowDialog();
            } else {
               MessageBox.Show("All bills apparently have an associated report.  I do not believe this.");
            }
         } catch (Exception ex) {
            LogAndThrow($"CreateNewReports.Run: {ex.Message}."); //
         }
         var elapsed = DateTime.Now - start_time;
         LogAndDisplay(form1.txtCreatesProgress, $"Through with bill report creation. {elapsed.ToString("c")} ");
      }
      /// <summary>
      /// Bills with no recorded positions are considered bills with no reports.
      /// </summary>
      /// <returns></returns>
      private IOrderedEnumerable<BillRow> CollectNoPositionBills() {
         var all_bills = BillRow.RowSet();   // All bills for the current biennium.
         var result =
            from item in all_bills
            where ((item.Position == string.Empty) && (item.NegativeScore > 0))
            orderby item.NegativeScore descending select item;
         return result;
      }
      /// <summary>
      /// Convert the display list to the type acceptable to the form.
      /// </summary>
      /// <param name="source_rows"></param>
      /// <returns></returns>
      private List<UnreportedBillForDisplay> Convert(IOrderedEnumerable<BillRow> source_rows) {
         var result = new List<UnreportedBillForDisplay>();
         foreach (var bill in source_rows) {
            result.Add(new UnreportedBillForDisplay(bill.Bill, bill.NegativeScore.ToString(), bill.Title, bill.Author));
         }
         return result;
      }
      public static void GenerateCanonicalReport(string measure) {
         BillRow row = BillRow.Row(BillUtils.Ensure4DigitNumber(measure));
         var path = $"{Path.Combine(Config.Instance.HtmlFolder, measure)}.html";
         List<string> contents = ReportContents(row, path);
         WriteTextFile(contents, path);
         var process = Process.Start("notepad.exe", path);
         if (process != null) process.WaitForExit();
         else LogAndShow($"CreateNewReports.GenerateCanonicalReport: Failed to start Notepad for {path}.");
         LogAndShow($"Completed {path}");
      }

      private static void WriteTextFile(List<string> contents, string path) {
         using (System.IO.StreamWriter file = new System.IO.StreamWriter(path)) {
            foreach (string line in contents) { file.WriteLine(line); }
         }
      }

      private static List<string> ReportContents(BillRow row, string path) {
         List<string> result = new List<string>();
         string name_ext = Path.GetFileName(row.Lob);                   // BillVersionTable bill_xml is unique
         BillVersionRow bv_row = GlobalData.VersionTable.Scalar(name_ext);
         List<BillHistoryRow> history = GlobalData.HistoryTable.RowSet(bv_row.BillID);
         var location_code = history.First().TernaryLocation;
         var location_code_row = GlobalData.LocationTable.Scalar(location_code);

         string appropriation = bv_row.Appropriation;
         string author = row.Author;
         string bill_id = row.Bill;
         string fiscal = bv_row.FiscalCommittee;
         string house = history.First().PrimaryLocation;
         string last_action = history.First().Action;
         string location = location_code_row == null ? string.Empty : location_code_row.Description;
         string local_pgm = bv_row.LocalProgram;
         string number = row.MeasureNum.TrimStart('0');
         string title = row.Title;
         string type_house = $"{bill_id.First()}B";
         string vers_id = row.BillVersionID;
         string vote = bv_row.VoteRequired;

         // These data come from the previous version of the bill report
         var summary = new List<string>();
         var position = new List<string>();
         PreviousReport.From(path, summary, position);

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
         result.Add("<p>");
         if (summary.Count > 0) {
            foreach (var line in summary) result.Add(line);
         } else {
            result.Add($"<b>Summary</b>: (Reviewed {DateTime.Now.ToShortDateString()})");
            result.Add("   <br /> (Quotations taken directly from the bill's language, or from current code)");
            result.Add("   <br />");
            result.Add("   <br /> This is my review");
            result.Add("</p>");
         }

         // Short Summary
         result.Add("<p>");
         result.Add("   <b>ShortSummary</b>: ");
         result.Add("</p>");

         // Position
         if (position.Count > 0) {
            foreach (var line in position) result.Add(line);
         } else {
            if (summary.Count == 0) result.Add("<p>");
            result.Add("   <b>Position</b>: ");
            result.Add("   <br /> This is my reason.");
            result.Add("</p>");
         }

         // Status, Location, etc
         if (position.Count == 0) result.Add("<p>");
         result.Add("<b>Status</b>:");
         result.Add($"<br /> Location: {location}");
         string str_date = String.Empty;
         if (history.Count > 0) str_date = history.First().ActionDate;
         result.Add($"<br /> Last Action:  {FormatDate(str_date)} {last_action}");
         result.Add("<table cellspacing=\"0\" cellpadding=\"0\">");
         result.Add($"   <tr><td> Vote: {vote}             </td><td> &nbsp; &nbsp; Appropriation: {appropriation}</td></tr>");
         result.Add($"   <tr><td> Fiscal committee: {fiscal} </td><td> &nbsp; &nbsp; State-mandated local program: {local_pgm} </td></tr>");
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
         if (DateTime.TryParse(str_date, out date_result))
            result = $"{date_result.ToString("MMM d yyyy")}";
         else
            result = "(invalid date)";
         return result;
      }
   }
}