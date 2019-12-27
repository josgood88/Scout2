using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Library;
using Library.Database;
using OpenQA.Selenium.Support.UI;

namespace Scout2.Sequence {
   public class ImportController : BaseController {
      public void Run(Form1 form1) {
         var start_time = DateTime.Now;
         try {
            LogAndDisplay(form1.txtImportProgress, "Writing Bill Version table.");
            BillVersionTable.ClearYourself(); 
            BillVersionRow.WriteRowset(GlobalData.VersionTable.Table);

            LogAndDisplay(form1.txtImportProgress, "Writing Bill History table.");
            BillHistoryTable.ClearYourself(); 
            BillHistoryRow.WriteRowset(GlobalData.HistoryTable.Table);

            LogAndDisplay(form1.txtImportProgress, "Writing Bill Location table.");
            LocationCodeTable.ClearYourself(); 
            LocationCodeRow.WriteRowset(GlobalData.LocationTable.Table);

            // Determine most recent version of each Asm/Sen bill.
            LogAndDisplay(form1.txtImportProgress, "Determining most recent version of each bill.");
            EnsureMostRecentEachBill();
            GlobalData.Profiles = Profiles(GlobalData.MostRecentEachBill);  // Prepare for ranking the bills
            UpdateBillRowsFromBillProfile(GlobalData.Profiles);
         } catch (Exception ex) {   
            LogAndThrow($"ZipController.Run: {ex.Message}.");
         }
         var elapsed = DateTime.Now - start_time;
         LogAndDisplay(form1.txtImportProgress, $"Imports and most-recent-bill processing complete. {elapsed.ToString("c")} ");
      }
      /// <summary>
      /// Wrap Bill_Identifiers into BillProfiles, in preparation for ranking the bills
      /// </summary>
      /// <param name="identities"></param>
      /// <returns></returns>
      private List<BillProfile> Profiles(List<Bill_Identifier> identities) {
         var negative_scores = PartialScore.ImportKeywordScores(Config.Instance.ScoutFile);
         var positive_scores = new List<PartialScore>();
         var result = new List<BillProfile>();
         foreach (var item in identities) {
            result.Add(new BillProfile(item,negative_scores,positive_scores));
         }
         return result;
      }
   }
}