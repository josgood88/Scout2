using System;
using Library;
using Library.Database;

namespace Scout2.Sequence {
   /// <summary>
   /// This class manages the work of importing data from legislative lob files (bill text and tables) into the
   /// database.  It provides a simple overall view of the import sequence.
   /// The grunt work is done elsewhere.
   /// </summary>
   public class ImportController : BaseController {
      public void Run(Form1 form1) {
         var start_time = DateTime.Now;
         try {
            LogAndDisplay(form1.txtImportProgress, "Initializing BillRows.");
            new InitializeBillRows().Run(form1);

            LogAndDisplay(form1.txtImportProgress, "Writing Bill Version table.");
            BillVersionTable.ClearYourself(); 
            BillVersionRow.WriteRowset(GlobalData.VersionTable.Table);

            LogAndDisplay(form1.txtImportProgress, "Writing Bill History table.");
            BillHistoryTable.ClearYourself(); 
            BillHistoryRow.WriteRowset(GlobalData.HistoryTable.Table);

            LogAndDisplay(form1.txtImportProgress, "Writing Bill Location table.");
            LocationCodeTable.ClearYourself(); 
            LocationCodeRow.WriteRowset(GlobalData.LocationTable.Table);

         } catch (Exception ex) {   
            LogAndThrow($"ZipController.Run: {ex.Message}.");
         }
         var elapsed = DateTime.Now - start_time;
         LogAndDisplay(form1.txtImportProgress, $"Data has been imported from legislative files to the local database. Elapsed Time: {elapsed.ToString("c")} ");
      }
   }
}