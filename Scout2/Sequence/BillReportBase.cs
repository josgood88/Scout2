using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Library.Database;
using Scout2.WeeklyReport;

namespace Scout2.Sequence {
   public class BillReportBase : BaseController {

      protected string ExtractLeadingDate(string str) {
         string result = Regex.Replace(str, @"(\w{3} \d{1,2} \d{4}).*", "$1").ToString();
         return result;
      }

      private string DateOnly(string date_time_string) {
         if (DateTime.TryParse(date_time_string, out DateTime dt)) return dt.ToString("dd MMMM yyyy");
         else throw new ApplicationException($"BillReportBase:DateOnly: Invalid date in {date_time_string}");
      }
      /// <summary>
      /// Bill has been updated if the Last Action date in the history file is later than the Last Action date
      /// in the latest bill report.
      /// </summary>
      /// <param name="bill">Chamber and bill number concatenated, as in "AB8"</param>
      /// <param name="history">The latest history file</param>
      /// <param name="history_last_action">The date of the last action</param>
      /// <returns>true if the bill has been updated, false otherwise</returns>
      protected bool IsUpdated(BillReport bill, List<BillHistoryRow> history, out string history_last_action) {
         bool result = false;
         history_last_action = null;

         if (bill.IsNotDead() && bill.IsNotChaptered()) {
            // Get the latest action as shown in the latest bill report
            string last_action_date_string = Regex.Replace(bill.LastAction, @"(\d{1,2} \w{3,} \d{4}).*", "$1").ToString();
            if (!DateTime.TryParse(last_action_date_string, out DateTime bill_last_action_date)) {
               throw new ApplicationException($"BillReportBase.IsUpdated: Invalid datetime in bill - {bill.LastAction}");
            }
            // Get the latest action as shown in the history file
            string bill_id = bill.Measure.Replace("-", string.Empty);   // change.BillID doesn't have the dash
            var selected= (from item in history where (item.BillID.EndsWith(bill_id)) select item).ToList();
            selected.Sort((a, b) => a.SequenceAsInt().CompareTo(b.SequenceAsInt()));
            var latest_change = selected.Last();
            if (latest_change != null) {
               if (!DateTime.TryParse(latest_change.ActionDate, out DateTime history_action_date)) {
                  throw new ApplicationException($"BillReportBase.IsUpdated: Invalid datetime in history - {latest_change.ActionDate}");
               }
               result = history_action_date > bill_last_action_date;
               history_last_action = DateOnly(latest_change.ActionDate);
            }
         }
         return result;
      }
   }
}