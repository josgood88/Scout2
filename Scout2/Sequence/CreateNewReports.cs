using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Library;
using Library.Database;
using Scout2.Report;

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
   }
}