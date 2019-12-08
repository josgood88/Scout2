using System;
using System.IO;
using System.Windows.Forms;
using Library;
using Library.Database;

namespace Scout2.Controllers {
   public class BaseController {

      protected static void LogThis(string message) {
         string output = $"{DateTime.Now.ToLocalTime()} {message}";
         Console.WriteLine(message);
         Log.Instance.Info(message);
      }
      /// <summary>
      /// The main sequence of processing contains multiple start points.
      /// Some need global data tables to be filled in.  This is the single point where that is done.
      /// It is called from each start point that needs one or more of these tables
      /// </summary>
      protected static void EnsureGlobalData() {
         if (GlobalData.BillRows == null)
            GlobalData.BillRows = BillRow.RowSet();
         if (GlobalData.VersionTable == null)
            GlobalData.VersionTable  = new BillVersionTable(Path.Combine(Config.Instance.BillsFolder, "BILL_VERSION_TBL.dat"));
         if (GlobalData.HistoryTable == null)
            GlobalData.HistoryTable  = new BillHistoryTable(Path.Combine(Config.Instance.BillsFolder, "BILL_HISTORY_TBL.dat"));
         if (GlobalData.LocationTable == null)
            GlobalData.LocationTable = new LocationCodeTable(Path.Combine(Config.Instance.BillsFolder, "LOCATION_CODE_TBL.dat"));
      }

      public static void LogAndShow(string message) {
         LogThis(message);
         MessageBox.Show(message);
      }

      public static void LogAndThrow(string message) {
         LogThis(message);
         throw new ApplicationException(message);
      }
   }
}
