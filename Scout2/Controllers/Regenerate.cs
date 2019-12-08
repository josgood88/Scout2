using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Library;
using Library.Database;
using Scout2.IndividualReport;

namespace Scout2.Controllers {
   public class Regenerate : BaseController {
      public void Run(Form1 form1) {
         const bool verbose = false, update = false;
         var start_time = DateTime.Now;
         EnsureGlobalData();  // Ensure that database tables have been read into memory
         try {
            string bill = string.Empty;
            List<string> bills = new List<string>();
            if (bill == string.Empty) bills = RecognizeChangedBills();
            else bills.Add(bill);
            if (bills.Count > 0) {
               foreach (var item in bills) (new IndividualReport.IndividualReport(verbose, update)).Run(item);
            } else {
               LogAndShow("BR found no bills to process.");
            }
         } catch (Exception ex) {
            LogAndThrow($"Regenerate.Run: {ex.Message}.");
         }
         var elapsed = DateTime.Now - start_time;
         var message = $"Bill reports re-generation complete. {elapsed.ToString("c")} ";
         LogThis(message);
         form1.txtImportProgress.Text = message;
         form1.txtImportProgress.Update();
      }

      private List<string> RecognizeChangedBills() {
         var result = new List<string>();
         List<string> folder_contents = Directory.EnumerateFiles(Config.Instance.HtmlFolder, "*.html").ToList();
         foreach (var path in folder_contents) {
            if (!path.Contains("Weekly")) {
               DateTime latest_report_date = DateFromReport(path);
               DateTime latest_history_date = DateFromHistoryTable(path);
               if (latest_history_date > latest_report_date) {
                  result.Add(Path.GetFileNameWithoutExtension(path));
                  String bill = Path.GetFileNameWithoutExtension(path);
                  String position = PreviousReport.Position(path);
                  var message = $"{bill} ({position.Trim()}) has changed.";
                  Console.WriteLine(message);
                  LogThis(message);
               }
            }
         }
         return result;
      }

      private DateTime DateFromReport(string path) {
         var history = PreviousReport.History(path);
         if (history.Count < 1) return default(DateTime);     // No history makes this a new bill
         DateTime date_result = default(DateTime);
         string str_date = Regex.Replace(history[1].Trim(), @"<br /> (\w{3} \d{1,2} \d{4}).*", "$1");
         DateTime.TryParse(str_date, out date_result);
         return date_result;
      }

      private DateTime DateFromHistoryTable(string path) {
         DateTime date_result = default(DateTime);
         String bill = Path.GetFileNameWithoutExtension(path);
         BillRow row = BillRow.Row(Ensure4DigitNumber(bill));
         string name_ext = Path.GetFileName(row.Lob);                   // BillVersionTable bill_xml is unique
         BillVersionRow bv_row = GlobalData.VersionTable.Scalar(name_ext);
         List<BillHistoryRow> history = GlobalData.HistoryTable.RowSet(bv_row.BillID);
         DateTime.TryParse(history.First().ActionDate, out date_result);
         return date_result;
      }

      private string Ensure4DigitNumber(string bill) {
         string house = "", number = "";
         CreateIndividualReport.ExtractHouseNumber(bill, out house, out number);
         while (number.Length < 4) number = $"0{number}";
         return $"{house}{number}";
      }
   }
}