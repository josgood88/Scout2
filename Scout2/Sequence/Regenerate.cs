using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Library;
using Library.Database;
using Scout2.IndividualReport;
using Scout2.Report;

namespace Scout2.Sequence {
   public class Regenerate : BaseController {
      public void Run(Form1 form1) {
         const bool verbose = false, update = false;
         var start_time = DateTime.Now;
         LogAndDisplay(form1.txtRegenProgress, "Updating bill history on individual bill reports.");
         try {
            string bill = string.Empty;
            List<string> bills = new List<string>();
            if (bill == string.Empty) 
               bills = form1.IsRegenerateAll() ? SelectAllBills () : RecognizeChangedBills();
            else bills.Add(bill);
            if (bills.Count > 0) {
               foreach (var item in bills) {
                  (new IndividualReport.IndividualReport(verbose, update)).Run(item);
               }
            } else {
               LogAndDisplay(form1.txtRegenProgress, "No bill history updates were necessary.");
            }
         } catch (Exception ex) {
            LogAndThrow($"Regenerate.Run: {ex.Message}.");
         }
         var elapsed = DateTime.Now - start_time;
         LogAndDisplay(form1.txtRegenProgress, $"Bill reports re-generation complete. {elapsed.ToString("c")} ");
      }

      private List<string> RecognizeChangedBills() {
         LogThis("Regenerating reports for those bills that have changed.");
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
                  LogThis(message);
               }
            }
         }
         return result;
      }

      private List<string> SelectAllBills() {
         LogThis("Regenerating reports for all bills.");
         var result = new List<string>();
         List<string> folder_contents = Directory.EnumerateFiles(Config.Instance.HtmlFolder, "*.html").ToList();
         foreach (var path in folder_contents) {
            if (!path.Contains("Weekly")) {
               result.Add(Path.GetFileNameWithoutExtension(path));
               var bill = Path.GetFileNameWithoutExtension(path);
               var position = PreviousReport.Position(path);
            }
         }
         return result;
      }

      private DateTime DateFromReport(string path) {
         var history = PreviousReport.History(path);
         if (history.Count < 1) return default(DateTime);     // No history makes this a new bill
         string str_date = Regex.Replace(history[1].Trim(), @"<br /> (\w{3} \d{1,2} \d{4}).*", "$1");
         DateTime.TryParse(str_date, out DateTime date_result);
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

      //todo Condense multiple copies
      private string Ensure4DigitNumber(string bill) {
         CreateIndividualReport.ExtractHouseNumber(bill, out string house, out string number);
         while (number.Length < 4) number = $"0{number}";
         return $"{house}{number}";
      }
   }
}