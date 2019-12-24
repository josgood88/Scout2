using System;

namespace Scout2.Sequence {
   public class CreateNewReports : BaseController {
      ///
      /// Give the user an opportunity to create new reports
      /// for those bills having the highest scores.
      public void Run(Form1 form1, UpdatedBillsForm update_form) {
         var start_time = DateTime.Now;
         LogAndDisplay(form1.txtCreatesProgress, "Showing bills that may need new reports.");
         try {
            ShowNewBills(form1,update_form);
         } catch (Exception ex) {
            LogAndThrow($"CreateNewReports.Run: {ex.Message}.");
         }
         var elapsed = DateTime.Now - start_time;
         LogAndDisplay(form1.txtCreatesProgress, $"Through with bill report creation. {elapsed.ToString("c")} ");
      }
	  
	  private void ShowNewBills(Form1 form1, UpdatedBillsForm update_form) {
	  }
   }
}