using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Library;
using Library.Database;

namespace Scout2.Sequence {
   public class BaseController {
      /// <summary>
      /// Initialize before entering the main sequence.
      /// Prepares text areas on the main form.
      /// Sets contents of some database tables, using data from table files contained in the zipped download.
      /// </summary>
      /// <param name="form"></param>
      /// <returns>True if all is well, False if unable to access table files expected to be present</returns>
      protected static bool BeforeEnteringMainSequence(Form1 form) {
         form.txtLegSiteCompletion.Text = string.Empty;
         form.txtZipProgress.Text = string.Empty;
         form.txtImportProgress.Text = string.Empty;
         form.txtRegenProgress.Text = string.Empty;
         form.txtBillUpdatesProgress.Text = string.Empty;
         form.txtCreatesProgress.Text = string.Empty;
         form.txtReportProgress.Text = string.Empty;
         form.progressLegSite.Value = 0;
         form.Update();

         return EnsureGlobalData();  // Ensure that database tables have been read into memory
      }
      /// <summary>
      /// Sets contents of some database tables, using data from table files contained in the zipped download.
      /// </summary>
      /// <returns>True if all is well, False if unable to access table files expected to be present</returns>
      public static bool EnsureGlobalData() {   // public so that XUnit TestNewOrChangePrefix can ensure table contents are available
         bool result = true;
         Config.Instance.ReadYourself();  // Start of configuration data lifetime
         GlobalData.Profiles = new List<BillProfile>();
         if (GlobalData.BillRows == null)
            GlobalData.BillRows = BillRow.RowSet();
         if (GlobalData.HistoryTable == null) {
            string path = Path.Combine(Config.Instance.BillsFolder, "BILL_HISTORY_TBL.dat");
            if (File.Exists(path)) GlobalData.HistoryTable  = new BillHistoryTable(path);
            else result = false;
         }
         if (GlobalData.VersionTable == null) {
            string path = Path.Combine(Config.Instance.BillsFolder, "BILL_VERSION_TBL.dat");
            if (File.Exists(path)) GlobalData.VersionTable  = new BillVersionTable(path);
            else result = false;
         }
         if (GlobalData.LocationTable == null) {
            string path = Path.Combine(Config.Instance.BillsFolder, "LOCATION_CODE_TBL.dat");
            if (File.Exists(path)) GlobalData.LocationTable = new LocationCodeTable(path);
            else result = false;
         }
         GlobalData.MostRecentEachBill = new List<Bill_Identifier>();
         return result;
      }

      public static void LogThis(string message) {
         string output = $"{DateTime.Now.ToLocalTime()} {message}";
         Log.Instance.Info(message);
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
