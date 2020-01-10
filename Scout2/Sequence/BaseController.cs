using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Library;
using Library.Database;

namespace Scout2.Sequence {
   public class BaseController {

      public static void LogThis(string message) {
         string output = $"{DateTime.Now.ToLocalTime()} {message}";
         Console.WriteLine(message);
         Log.Instance.Info(message);
      }
      /// <summary>
      /// Initialize before entering the main sequence.
      /// </summary>
      protected static void BeforeEnteringMainSequence(Form1 form) {
         form.txtLegSiteCompletion.Text = string.Empty;
         form.txtZipProgress.Text = string.Empty;
         form.txtImportProgress.Text = string.Empty;
         form.txtUpdateScores.Text = string.Empty;
         form.txtRecordReportDates.Text = string.Empty;
         form.txtRegenProgress.Text = string.Empty;
         form.txtBillUpdatesProgress.Text = string.Empty;
         form.txtCreatesProgress.Text = string.Empty;
         form.txtReportProgress.Text = string.Empty;
         form.progressLegSite.Value = 0;
         form.Update();

         EnsureGlobalData();  // Ensure that database tables have been read into memory
      }
      /// <summary>
      /// The main sequence of processing contains multiple start points.
      /// Some need global data tables to be filled in.  This is the single point where that is done.
      /// It is called from each start point that needs one or more of these tables
      /// </summary>
      protected static void EnsureGlobalData() {
         if (GlobalData.BillRows == null)
            GlobalData.BillRows = BillRow.RowSet();
         if (GlobalData.HistoryTable == null)
            GlobalData.HistoryTable  = new BillHistoryTable(Path.Combine(Config.Instance.BillsFolder, "BILL_HISTORY_TBL.dat"));
         if (GlobalData.VersionTable == null)
            GlobalData.VersionTable  = new BillVersionTable(Path.Combine(Config.Instance.BillsFolder, "BILL_VERSION_TBL.dat"));
         if (GlobalData.LocationTable == null)
            GlobalData.LocationTable = new LocationCodeTable(Path.Combine(Config.Instance.BillsFolder, "LOCATION_CODE_TBL.dat"));
      }
      // Long running method, needed by ImportController
      protected static void EnsureMostRecentEachBill() {
         if (GlobalData.MostRecentEachBill == null)
            GlobalData.MostRecentEachBill = MostRecentBills.Identify(Config.Instance.BillsFolder);
      }
      /// <summary>
      /// Update the database BillRows table from a list of BillProfile
      /// Position is added by BillRows(profile) constructor.
      /// Obtains position by querying the current GlobalData.BillRows.
      /// </summary>
      /// <param name="ranked">BillRows updated by the "ScoreTheBill" method.</param>
      protected void UpdateBillRowsFromBillProfile(List<BillProfile> ranked) {
         var bill_rows = new List<BillRow>();
         foreach (var profile in ranked) bill_rows.Add(new BillRow(profile));
         BillRow.WriteRowset(bill_rows);
      }

      public static void LogAndShow(string message) {
         LogThis(message);
         MessageBox.Show(message);
      }

      public static void LogAndThrow(string message) {
         LogThis(message);
         throw new ApplicationException(message);
      }

      public static void LogAndDisplay(TextBox textbox, string message) {
         LogThis(message);
         textbox.Text = message;
         textbox.Update();
      }
   }
}
