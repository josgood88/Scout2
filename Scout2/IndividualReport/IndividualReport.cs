﻿using System;
using System.Collections.Generic;
using System.IO;
using Library;
using Library.Database;
using Scout2.Sequence;
using Scout2.Utility;

namespace Scout2.IndividualReport {
   public class IndividualReport : IndividualReportCommon {

      public void Run(string _bill) {
         var path = FilePath(_bill);
         Create(_bill,path);
      }

      private string FilePath(string bill) { return $"{Path.Combine(Config.Instance.HtmlFolder,bill)}.html"; }

      private void Create(string bill, string path) {
         try { 
         BillRow row = BillRow.Row(BillUtils.Ensure4DigitNumber(bill));
         List<string> contents = CreateIndividualReport.ReportContents(row, path);
         WriteTextFile(contents, path);
         var message = $"Regenerated {row.Bill} report.";
         BaseController.LogThis(message);
         } catch (Exception ex) {
            LogAndThrow($"IndividualReport.Create: {ex.Message}.");
         }
      }
   }
}
