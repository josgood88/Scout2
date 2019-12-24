using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Library;
using Library.Database;
using Scout2.Report;

namespace Scout2.Sequence {
   //public class UpdateNeeded {
   //   public string Measure { private set; get; }
   //   public string Position { private set; get; }
   //   public string BillLastAction { private set; get; }
   //   public string HistoryLastAction { private set; get; }
   //   public UpdateNeeded(string a, string b, string c, string d) {
   //      Measure = a; Position = b; BillLastAction = c; HistoryLastAction = d;
   //   }
   //}

   public class Changes : BaseController {
      ///
      /// Collect all bills that have been updated since the last report written on that bill.
      /// Display a report listing those bills.
      public void Run(Form1 form1, UpdatedBillsForm update_form) {
         var start_time = DateTime.Now;
         try {
            // Read global data into memory from database
            EnsureGlobalData();  // Ensure that database tables have been read into memory

            // Collect all bill history for the current biennium.
            // Collect all bill reports written for the current biennium.
            var history = BillHistoryRow.RowSet();
            var individual_bill_reports = new BillReportCollection(Config.Instance.HtmlFolder);

            // Collect those bills that are new.
            //var new_bills = new List<UpdateNeeded>();
            //var line = GlobalData.BillRows.Find(x => x.CurrentHouse=="AB" && x.MeasureNum=="8");
            //foreach (var bill in individual_bill_reports) {
            //   if (IsUpdated(bill, history, out string history_latest_action)) {
            //      string last_action_date = ExtractLeadingDate(bill.LastAction);
            //      var is_new_bill = $"NEW Bill {history_latest_action}";
            //      new_bills.Add(new UpdateNeeded(bill.Measure, bill.Position, last_action_date, is_new_bill));
            //   }
            //}

            // Collect those bills that have been updated since the last report written on that bill.
            var updated_bills = new List<UpdateNeeded>();
            foreach (var bill in individual_bill_reports) {
               if (IsUpdated(bill, history, out string history_latest_action)) {
                  string last_action_date = ExtractLeadingDate(bill.LastAction);
                  updated_bills.Add(new UpdateNeeded(bill.Measure, bill.Position, last_action_date, history_latest_action));
               }
            }
            update_form.PrepareDataGridView();
            update_form.AddRows(updated_bills);
            //update_form.AddRows(new_bills);
            update_form.ShowDialog();
         } catch (Exception ex) {
            LogAndThrow($"BillUpdates.Run: {ex.Message}.");
         }
         var elapsed = DateTime.Now - start_time;
         var message = $"Bill Updates report complete. {elapsed.ToString("c")} ";
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
            if (!DateTime.TryParse(latest_change.ActionDate,out DateTime history_action_date)) {
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
         string result = string.Empty;
         if (DateTime.TryParse(date_time_string, out DateTime dt)) result = dt.ToString("dd MMMM yyyy");
         else throw new ApplicationException($"BillUpdates:DateOnly: Invalid date in {date_time_string}");
         return result;
      }
   }
}
