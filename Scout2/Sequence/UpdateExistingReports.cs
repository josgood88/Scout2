using System;
using System.Collections.Generic;
using System.Linq;
using Library;
using Library.Database;
using Scout2.Report;

namespace Scout2.Sequence {
   public class UpdateExistingReports : BillReportBase {
      ///
      /// Give the user an opportunity to update existing reports
      /// due to changes that have ocurred.
      public void Run(Form1 form1, UpdatedBillsForm update_form) {
         var start_time = DateTime.Now;
         try {
            LogAndDisplay(form1.txtBillUpdatesProgress, "Showing reports that need updating.");
            List<ChangedBillForDisplay> updated_bills = CollectUpdatedBills(form1,update_form);
            // Display those bills that have changed, or else a MessageBox saying nothing has changed
            if (updated_bills.Any()) {
               update_form.PrepareDataGridView();
               update_form.AddRows(updated_bills);
               update_form.ShowDialog();
            } else {
               //MessageBox.Show("No bills have changed.  There is nothing to update");
            }
         } catch (Exception ex) {
            LogAndThrow($"UpdateExistingReports.Run: {ex.Message}.");
         }
         var elapsed = DateTime.Now - start_time;
         LogAndDisplay(form1.txtBillUpdatesProgress, $"Through with updating bill reports. Elapsed Time: {elapsed.ToString("c")} ");
      }

      private List<ChangedBillForDisplay> CollectUpdatedBills(Form1 form1, UpdatedBillsForm update_form) {
         // Collect all bill history for the current biennium.
         // Collect all bill reports written for the current biennium.
         var history = BillHistoryRow.RowSet();
         var individual_bill_reports = new BillReportCollection(Config.Instance.HtmlFolder);

         // Collect those bills that have been updated since the last report written on that bill.
         var updated_bills = new List<ChangedBillForDisplay>();
         foreach (var bill in individual_bill_reports) {
            if (IsUpdated(bill, history, out string history_latest_action)) {
               string last_action_date = ExtractLeadingDate(bill.LastAction);
               updated_bills.Add(new ChangedBillForDisplay(bill.Measure, bill.Position, last_action_date, history_latest_action));
            }
         }
         return updated_bills;
      }
   }
}