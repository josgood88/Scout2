using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Library;
using Library.Database;
using Scout2.Report;

namespace Scout2.Sequence {
   public class UpdateNeeded {
      public string Measure { private set; get; }
      public string Position { private set; get; }
      public string BillLastAction { private set; get; }
      public string HistoryLastAction { private set; get; }
      public UpdateNeeded(string a, string b, string c, string d) {
         Measure = a; Position = b; BillLastAction = c; HistoryLastAction = d;
      }
   }

   public class UpdateExistingReports : BaseController {
      ///
      /// Give the user an opportunity to update existing reports
      /// due to changes that have ocurred.
      public void Run(Form1 form1, UpdatedBillsForm update_form) {
         var start_time = DateTime.Now;
         try {
            // Read global data into memory from database
            EnsureGlobalData();  // Ensure that database tables have been read into memory
            List<UpdateNeeded> updated_bills = CollectUpdatedBills(form1,update_form);
            // Display those bills that have changed, or else a MessageBox saying nothing has changed
            if (updated_bills.Count > 0) {
               update_form.PrepareDataGridView();
               update_form.AddRows(updated_bills);
               update_form.ShowDialog();
            } else {
               MessageBox.Show("No bills have changed.  There is nothing to update");
            }
         } catch (Exception ex) {
            LogAndThrow($"UpdateExistingReports.Run: {ex.Message}.");
         }
         var elapsed = DateTime.Now - start_time;
         var message = $"Through with updating bill reports. {elapsed.ToString("c")} ";
         LogThis(message);
         form1.txtBillUpdatesProgress.Text = message;
         form1.txtBillUpdatesProgress.Update();
      }

      private List<UpdateNeeded> CollectUpdatedBills(Form1 form1, UpdatedBillsForm update_form) {
         // Collect all bill history for the current biennium.
         // Collect all bill reports written for the current biennium.
         var history = BillHistoryRow.RowSet();
         var individual_bill_reports = new BillReportCollection(Config.Instance.HtmlFolder);

         // Collect those bills that have been updated since the last report written on that bill.
         var updated_bills = new List<UpdateNeeded>();
         foreach (var bill in individual_bill_reports) {
            if (IsUpdated(bill, history, out string history_latest_action)) {
               string last_action_date = ExtractLeadingDate(bill.LastAction);
               updated_bills.Add(new UpdateNeeded(bill.Measure, bill.Position, last_action_date, history_latest_action));
            }
         }
         return updated_bills;
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