using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Library;
using Library.Database;
using Scout2.Controllers;

namespace Scout2.WeeklyReport {
   public class Report {
      //private string bill;
      private bool verbose;
      private bool update;

      public Report(bool _verbose, bool _update) { verbose = _verbose; update = _update; }

      public void Run(string _bill) {
         var path = FilePath(_bill);
         Create(_bill,path);
      }

      private string Ensure4DigitNumber(string bill) {
         string house = "", number = "";
         CreateReport.ExtractHouseNumber(bill,out house,out number);
         while (number.Length < 4) number = $"0{number}";
         return $"{house}{number}";
      }

      private string FilePath(string bill) { return $"{Path.Combine(Config.Instance.HtmlFolder,bill)}.html"; }

      private void Create(string bill, string path) {
         BillRow row = BillRow.Row(Ensure4DigitNumber(bill));
         List<string> contents = CreateReport.ReportContents(row,path);
         WriteTextFile(contents,path);
         var message = $"{row.Bill} has negative score {row.NegativeScore}";
         Console.WriteLine(message);
         BaseController.LogAndShow(message);
      }

      private void WriteTextFile(List<string> contents, string path) {
         using (System.IO.StreamWriter file = new System.IO.StreamWriter(path)) {
            foreach (string line in contents) { file.WriteLine(line); }
         }
      }
   }
}
