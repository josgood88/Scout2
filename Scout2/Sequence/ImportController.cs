﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Library;
using Library.Database;

namespace Scout2.Sequence {
   public class ImportController : BaseController {
      public void Run(Form1 form1) {
         var start_time = DateTime.Now;
         try {
            form1.txtImportProgress.Text = "Re-creating database tables.";
            form1.txtImportProgress.Update();

            // Read global data into memory from database
            EnsureGlobalData();  // Ensure that in-memory copies of database tables are not null

            form1.txtImportProgress.Text = "Writing Bill Version table.";
            form1.txtImportProgress.Update();
            BillVersionTable.ClearYourself(); 
            BillVersionRow.WriteRowset(GlobalData.VersionTable.Table);
            form1.txtImportProgress.Text = "Writing Bill History table.";
            form1.txtImportProgress.Update();
            BillHistoryTable.ClearYourself(); 
            BillHistoryRow.WriteRowset(GlobalData.HistoryTable.Table);
            form1.txtImportProgress.Text = "Writing Bill Location table.";
            form1.txtImportProgress.Update();
            LocationCodeTable.ClearYourself(); 
            LocationCodeRow.WriteRowset(GlobalData.LocationTable.Table);

            // Determine most recent version of each Asm/Sen bill
            form1.txtImportProgress.Text = "Determining most recent version of each bill.";
            form1.txtImportProgress.Update();
            EnsureMostRecentEachBill();
            GlobalData.Profiles = Profiles(GlobalData.MostRecentEachBill);  // Prepare for ranking the bills
            UpdateGlobalBillRows(out List<BillRow> new_bills);              // Update with GlobalData.Profiles data
         } catch (Exception ex) {   
            LogAndThrow($"ZipController.Run: {ex.Message}.");
         }
         var elapsed = DateTime.Now - start_time;
         var message = $"Extraction complete. {elapsed.ToString("c")} ";
         LogThis(message);
         form1.txtImportProgress.Text = message;
         form1.txtImportProgress.Update();
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
   }
}