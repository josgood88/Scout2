using System;

namespace Scout2.Sequence {
   public class CreateNewReports : BaseController {
      ///
      /// Give the user an opportunity to create new reports
      /// for those bills having the highest scores.
      public void Run(Form1 form1, UpdatedBillsForm update_form) {
         var start_time = DateTime.Now;
         try {
            // Read global data into memory from database
            EnsureGlobalData();  // Ensure that database tables have been read into memory
      
         } catch (Exception ex) {
            LogAndThrow($"CreateNewReports.Run: {ex.Message}.");
         }
         var elapsed = DateTime.Now - start_time;
         var message = $"Through with bill report creation. {elapsed.ToString("c")} ";
         LogThis(message);
         form1.txtCreatesProgress.Text = message;
         form1.txtCreatesProgress.Update();
      }
   }
}