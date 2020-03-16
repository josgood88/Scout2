using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Library;
using Library.Database;
using Scout2.Utility;

namespace Scout2.IndividualReport {
   public class CreateIndividualReport : Scout2.IndividualReport.PreviousReport {
      // Create a Bill Report, given a bill identifier such as AB12
      public static List<string> ReportContents(BillRow row, string path) {
         return BaseReportContents(row, path);
      }
   }
}
