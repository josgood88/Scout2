﻿using System;
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

      /// <summary>
      /// On entry, GlobalData.BillRows contains the current contents of the database BillRows table.
      /// Update the author, title, and lob file path.
      /// GlobalData.BillRows will be written back to the database once ranking has been performed.
      /// Until then, Scout will continue using GlobalData.BillRows.
      /// </summary>
      private void UpdateGlobalBillRows(out List<BillRow> new_bills) {
         new_bills = new List<BillRow>();
         foreach (var profile in GlobalData.Profiles) {
            BillRow bill_row = new BillRow();
            if (profile.Identifier.BillID != "AB0001") {
               bill_row = (from x in GlobalData.BillRows where x.Bill == profile.Identifier.BillID select x).FirstOrDefault();
            }
            if (bill_row != null && bill_row.CurrentHouse != null) {   // Could test multiple fields, picked this one
               bill_row.Author = profile.Identifier.Author;
               bill_row.Lob = profile.Identifier.LobPath;
               bill_row.Title = profile.Identifier.Title;
            } else {
               new_bills.Add(new BillRow(profile));
            }
         }
      }
      /// <summary>
      /// When bill data was re-imported from the .lob files, the current positions were lost from the BillRows table.
      /// Before bill data was re-imported, a copy of the BillRows table was made.
      /// That data is passed here as preserved_positions.
      /// This method restores position to the BillRows table.
      /// </summary>
      /// <param name="preserved_positions"></param>
      private void RestorePositionData(List<BillRow> preserved_positions) {
         // Not all BillRows entries have positions.  Select those that do.
         var rows_with_position_data = 
            from row in preserved_positions where row.Position.Length > 0 select row;
         
         // Update a local copy of the BillRows table
         var bill_rows = BillRow.RowSet();   // Get a copy of the current BillRows table
         foreach (var row in rows_with_position_data) {
            var copy_row = bill_rows.Find(x => x.Bill == row.Bill);
            if (copy_row == null) throw new ApplicationException($"ImportController.RestorePositionData: Bill {row.Bill} has disappeared.");
            copy_row.Position = row.Position;
         }

         // Update BillRows in the database
         BillRow.WriteRowset(bill_rows);
      }
   }
}