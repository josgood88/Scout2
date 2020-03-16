using System.Collections.Generic;
using Library.Database;

namespace Scout2.IndividualReport {
   public class CreateIndividualReport : PreviousReport {
      // Create a Bill Report, given a bill identifier such as AB12
      public static List<string> ReportContents(BillRow row, string path) {
         return BaseReportContents(row, path);
      }
   }
}
