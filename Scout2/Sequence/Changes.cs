using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Library;
using Library.Database;
using Scout2.WeeklyReport;

namespace Scout2.Sequence {
   public class Changes : BillReportBase {
      ///
      /// Collect all bills that have been updated since the last report written on that bill.
      /// Display a report listing those bills.
      public void Run(Form1 form1, UpdatedBillsForm update_form) {
         var start_time = DateTime.Now;
         try {
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
            update_form.PrepareDataGridView();
            update_form.AddRows(updated_bills);
            update_form.ShowDialog();
         } catch (Exception ex) {
            LogAndThrow($"BillUpdates.Run: {ex.Message}.");
         }
         var elapsed = DateTime.Now - start_time;
         var message = $"Bill Updates report complete. Elapsed Time: {elapsed.ToString("c")} ";
         LogThis(message);
         form1.txtBillUpdatesProgress.Text = message;
         form1.txtBillUpdatesProgress.Update();
      }

      private List<string> HtmlFiles(string folder_path) {
         var files = Directory.GetFiles(folder_path, "*.html", SearchOption.TopDirectoryOnly).ToList();
         var report_file = Path.Combine(folder_path, "WeeklyNewsMonitoredBills.html");
         files.Remove(report_file);
         return files;
      }
   }
}
