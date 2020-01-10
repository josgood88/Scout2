using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Library;
using Library.Database;
using Scout2.IndividualReport;

namespace Scout2.Sequence {
   public class RecordLastDate : BaseController {

      /// <summary>
      /// Date of last update needs to be acquired before bill reports are regenerated.  Regeneration includes
      /// updating the bill's history and that can update the bill's last action.  Last Action is the item
      /// that controls whether regeneration is needed.  Calling this method stops that left recursion
      /// </summary>
      /// <param name="form1"></param>
      /// <param name="whenLastUpdated">Fill this list with "Last Action Date" dates from bill reports</param>
      public void Run(Form1 form1, ref List<BillLastUpdate> whenLastUpdated) {
         string completion_message = string.Empty;
         var start_time = DateTime.Now;
         LogAndDisplay(form1.txtRecordReportDates, "Recording each bill report's last-updated date.");
         try {
            whenLastUpdated.Clear();
            whenLastUpdated = form1.IsRegenerateAll() ? SelectAllBills() : RecognizeChangedBills();
         } catch (Exception ex) {
            completion_message = $"Regenerate.AcquireLastUpdateDate: {ex.Message}.";
         }
         var elapsed = DateTime.Now - start_time;
         if (completion_message == string.Empty) completion_message = $"Last-Update dates acquired. {elapsed.ToString("c")} ";
         LogAndDisplay(form1.txtRecordReportDates, completion_message);
      }

      private List<BillLastUpdate> RecognizeChangedBills() {
         var result = new List<BillLastUpdate>();
         try {
            List<string> folder_contents = Directory.EnumerateFiles(Config.Instance.HtmlFolder, "*.html").ToList();
            foreach (var path in folder_contents) {
               if (!path.Contains("Weekly")) {
                  DateTime latest_report_date = DateFromReport(path);
                  DateTime latest_history_date = DateFromHistoryTable(path);
                  if (latest_history_date > latest_report_date) {
                     String bill = Path.GetFileNameWithoutExtension(path);
                     String last_updated = string.Empty;
                     result.Add(new BillLastUpdate(bill, StringDateFromReport(path)));
                     var message = $"{bill} ({last_updated}) has changed.";
                     LogThis(message);
                  }
               }
            }
         } catch (Exception ex) {
            LogThis($"Regenerate.RecognizeChangedBills: {ex.Message}.");
         }
         return result;
      }

      private List<BillLastUpdate> SelectAllBills() {
         var result = new List<BillLastUpdate>();
         try {
            List<string> folder_contents = Directory.EnumerateFiles(Config.Instance.HtmlFolder, "*.html").ToList();
            foreach (var path in folder_contents) {
               if (!path.Contains("Weekly")) {
                  var bill = Path.GetFileNameWithoutExtension(path);
                  var last_updated = string.Empty;
                  result.Add(new BillLastUpdate(bill, last_updated));
               }
            }
         } catch (Exception ex) {
            LogThis($"Regenerate.SelectAllBills: {ex.Message}.");
         }
         return result;
      }

      private string StringDateFromReport(string path) {
         string result = string.Empty;
         var history = PreviousReport.History(path);
         if (history.Count >= 1) {  // If this bill has history
            result = Regex.Replace(history[1].Trim(), @"<br /> (\w{3} \d{1,2} \d{4}).*", "$1");
         }
         return result;
      }

      private DateTime DateFromReport(string path) {
         string str_date = StringDateFromReport(path);
         DateTime.TryParse(str_date, out DateTime date_result);
         return date_result;
      }

      private DateTime DateFromHistoryTable(string path) {
         DateTime date_result = default(DateTime);
         try {
            String bill = Path.GetFileNameWithoutExtension(path);
            BillRow row = BillRow.Row(Ensure4DigitNumber(bill));
            string name_ext = Path.GetFileName(row.Lob);                   // BillVersionTable bill_xml is unique
            BillVersionRow bv_row = GlobalData.VersionTable.Scalar(name_ext);
            List<BillHistoryRow> history = GlobalData.HistoryTable.RowSet(bv_row.BillID);
            DateTime.TryParse(history.First().ActionDate, out date_result);
         } catch (Exception ex) {
            LogAndThrow($"Regenerate.DateFromHistoryTable: {ex.Message}.");
         }
         return date_result;
      }

      private string Ensure4DigitNumber(string bill) {
         CreateIndividualReport.ExtractHouseNumber(bill, out string house, out string number);
         while (number.Length < 4) number = $"0{number}";
         return $"{house}{number}";
      }
   }
}