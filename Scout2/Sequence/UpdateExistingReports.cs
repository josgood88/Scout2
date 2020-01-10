using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Library;
using Library.Database;
using Scout2.Report;

namespace Scout2.Sequence {
   public class UpdateExistingReports : BaseController {
      ///
      /// Give the user an opportunity to update existing reports
      /// due to changes that have ocurred.
      public void Run(Form1 form1, UpdatedBillsForm update_form, List<BillLastUpdate> whenLastUpdated) {
         string completion_message = string.Empty;
         var start_time = DateTime.Now;
         LogAndDisplay(form1.txtBillUpdatesProgress, "Showing reports that need updating.");
         try {
            List<BillForDisplay> updated_bills = CollectUpdatedBills(whenLastUpdated);
            // Display those bills that have changed, or else a MessageBox saying nothing has changed
            if (whenLastUpdated.Count > 0) {
               update_form.PrepareDataGridView();
               update_form.AddRows(updated_bills);
               update_form.ShowDialog();
            } else {
               completion_message = "No bills have changed.  There is nothing to update";
            }
         } catch (Exception ex) {
            completion_message = $"UpdateExistingReports.Run: {ex.Message}.";
         }
         var elapsed = DateTime.Now - start_time;
		 if (completion_message == string.Empty) completion_message = $"Through with updating bill reports. {elapsed.ToString("c")} ";
         LogAndDisplay(form1.txtBillUpdatesProgress, completion_message);
      }

      private List<BillForDisplay> CollectUpdatedBills(List<BillLastUpdate> whenLastUpdated) {
         var result = new List<BillForDisplay>();
         var history = BillHistoryRow.RowSet();
         var individual_bill_reports = new BillReportCollection();
         foreach (var item in whenLastUpdated) {
            var report = new BillReport(Path.Combine(Config.Instance.HtmlFolder, $"{item.bill}.html"));
            string last_action_date = ExtractLeadingDate(report.LastAction);
            result.Add(new BillForDisplay(report.Measure, report.Position, last_action_date, item.last_updated));
         }
         return result;
      }

      private bool IsUpdated(BillReport bill, List<BillHistoryRow> history, out string history_last_action) {
         bool result = false;
         string last_action_date_string = Regex.Replace(bill.LastAction, @"(\w{3} \d{1,2} \d{4}).*", "$1").ToString();
         if (!DateTime.TryParse(last_action_date_string, out DateTime bill_last_action_date)) {
            throw new ApplicationException($"BillUpdate.IsUpdated: Invalid datetime in bill - {bill.LastAction}");
         }
         // Latest change in the bill
         string bill_id = bill.Measure.Replace("-", string.Empty);   // change.BillID doesn't have the dash
         var changes = from change in history
                       where change.BillID.EndsWith(bill_id)
                       orderby change.ActionDate descending
                       select change;
         var latest_change = changes.First();
         if (latest_change != null) {
            if (!DateTime.TryParse(latest_change.ActionDate, out DateTime history_action_date)) {
               throw new ApplicationException($"BillUpdate.IsUpdated: Invalid datetime in history - {latest_change.ActionDate}");
            }
            result = history_action_date > bill_last_action_date;
         }
         history_last_action = result ? DateOnly(latest_change.ActionDate) : null;
         return result;
      }

      private string ExtractLeadingDate(string str) {
         string result = Regex.Replace(str, @"(\w{3} \d{1,2} \d{4}).*", "$1").ToString();
         return result;
      }

      private string DateOnly(string date_time_string) {
         if (DateTime.TryParse(date_time_string, out DateTime dt)) return dt.ToString("dd MMMM yyyy");
         else throw new ApplicationException($"BillUpdates:DateOnly: Invalid date in {date_time_string}");
      }
   }
}