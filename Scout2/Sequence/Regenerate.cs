using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Library;
using Library.Database;
using Scout2.IndividualReport;

namespace Scout2.Sequence {
   public class Regenerate : BaseController {

      /// <summary>
      /// This method regenerates bill reports, based on the contents of whenLastUpdated.
      /// </summary>
      /// <param name="form1"></param>
      /// <param name="whenLastUpdated">Fill this list with "Last Action Date" dates from bill reports</param>
      public void Run(Form1 form1, List<BillLastUpdate> whenLastUpdated) {
         string completion_message = string.Empty;
         const bool verbose = false, update = false;
         var start_time = DateTime.Now;
         LogAndDisplay(form1.txtRegenProgress, "Updating bill history on individual bill reports.");
         try {
            if (whenLastUpdated.Count > 0) {
               foreach (var item in whenLastUpdated) (new IndividualReport.IndividualReport(verbose, update)).Run(item.bill);
            } else {
               completion_message = "Regenerate found no bills to process.";
            }
         } catch (Exception ex) {
            completion_message = $"Regenerate.Run: {ex.Message}.";
         }
         var elapsed = DateTime.Now - start_time;
         if (completion_message == string.Empty) completion_message = $"Bill reports re-generation complete. {elapsed.ToString("c")} ";
         LogAndDisplay(form1.txtRegenProgress, completion_message);
      }
   }
}