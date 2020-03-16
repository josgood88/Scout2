using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Library;
using Library.Database;
using Scout2.Utility;

namespace Scout2.Sequence {
   public class CreateNewReports : Scout2.IndividualReport.PreviousReport {
      ///
      /// Give the user an opportunity to create new reports
      /// for those bills having the highest scores.
      public void Run(Form1 form1, UnreportedBillsForm upreported_form) {
         var start_time = DateTime.Now;
         try {
            LogAndDisplay(form1.txtCreatesProgress, "Showing bills that have no report.");
            var bills_with_no_position = CollectNoPositionBills();
            var bills_for_display = Convert(bills_with_no_position);
            // Display those bills that have no report.
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
         LogAndDisplay(form1.txtCreatesProgress, $"Through with bill report creation. Elapsed Time: {elapsed.ToString("c")} ");
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
      /// <summary>
      /// When the initial report on a bill is generated, it is in so-called "canonical" form, meaning that
      /// no position is taken, there is no meaningful summary, and so on.  In essence, a blank form is generated.
      /// The user is then given the opportunity to edit the blank form, turning it into an actual report on the bill.
      /// Once that editing is complete, the database BillRow table is updated with the position given in the
      /// edited bill report.
      /// </summary>
      /// <param name="measure"></param>
      public static void GenerateCanonicalReport(string measure) {
         // Generate the canonical bill
         BillRow row = BillRow.Row(BillUtils.Ensure4DigitNumber(measure));
         List<string> contents = BaseReportContents(row, string.Empty);
         string path = $"{Path.Combine(Config.Instance.HtmlFolder, BillUtils.EnsureNoLeadingZerosBill(measure))}.html";
         WriteTextFile(contents, path);
         // Let the user edit the canonical bill
         var process = Process.Start("notepad.exe", path);
         if (process != null) process.WaitForExit();
         else LogAndShow($"CreateNewReports.GenerateCanonicalReport: Failed to start Notepad for {path}.");
         // Update the database position
         BillRow.UpdatePosition(BillUtils.Ensure4DigitNumber(measure), "");
         GetPositionAndSummary(path, out List<string> summary, out List<string> position_list);
         string first_line = position_list.FirstOrDefault();
         string position = first_line != null ? Regex.Replace(first_line, ".*?:(.*)", "$1") : "None Specified";
         BillRow.UpdatePosition(measure, position.Trim());
         LogAndShow($"Completed {path}");
      }

      private static void WriteTextFile(List<string> contents, string path) {
         using (System.IO.StreamWriter file = new System.IO.StreamWriter(path)) {
            foreach (string line in contents) { file.WriteLine(line); }
         }
      }

      /// <summary>
      /// Obtain the position and summary information from the specified bill report.
      /// </summary>
      /// <param name="path">Path to the bill report</param>
      /// <param name="summary">Return summary information here</param>
      /// <param name="position">Return position information here</param>
      private static void GetPositionAndSummary(string path, out List<string> summary, out List<string> position) {
         summary = new List<string>();
         position = new List<string>();
         // Position and Summary data come from the previous version of the bill report
         // If the passed path is null or empty, then this method was called when no previous report exists.
         if (string.IsNullOrEmpty(path)) {
            // Do nothing
         } else {
            From(path, summary, position);
         }
      }
   }
}