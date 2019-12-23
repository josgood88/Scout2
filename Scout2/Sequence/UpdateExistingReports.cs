using System;

namespace Scout2.Sequence {
   public class UpdateExistingReports : BaseController {
      ///
      /// Give the user an opportunity to update existing reports
      /// due to changes that have ocurred.
      public void Run(Form1 form1, UpdatedBillsForm update_form) {
         var start_time = DateTime.Now;
         try {
            // Read global data into memory from database
            EnsureGlobalData();  // Ensure that database tables have been read into memory
         } catch (Exception ex) {
            LogAndThrow($"UpdateExistingReports.Run: {ex.Message}.");
         }
         var elapsed = DateTime.Now - start_time;
         var message = $"Through with updating bill reports. {elapsed.ToString("c")} ";
         LogThis(message);
         form1.txtBillUpdatesProgress.Text = message;
         form1.txtBillUpdatesProgress.Update();
      }
   }
}