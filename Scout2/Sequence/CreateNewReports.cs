using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Linq;
using System.Text.RegularExpressions;
using Library;
using Library.Database;

namespace Scout2.Sequence {
   public class CreateNewReports : BaseController {
      /// <summary>
      /// Give the user an opportunity to create new reports for bills not previously reviewed.
      /// </summary>
      /// <param name="form1"></param>
      /// <param name="new_bill_form"></param>
      public void Run(Form1 form1, NewBillsForm new_bill_form) {
         string completion_message = string.Empty;
         var start_time = DateTime.Now;
         LogAndDisplay(form1.txtCreatesProgress, "Showing bills that may need new reports.");
         try {
            List<NewBillForDisplay> bills_without_position = Candidates();
            // Display those bills that have no position, or else a MessageBox saying all bills have positions
            if (bills_without_position.Any()) {
               new_bill_form.PrepareDataGridView();
               new_bill_form.AddRows(bills_without_position);
               new_bill_form.ShowDialog();
            } else {
               completion_message = "All bills have positions.  No reviews are needed";
            }
         } catch (Exception ex) {
            completion_message = $"CreateNewReports.Run: {ex.Message}.";
         }
         var elapsed = DateTime.Now - start_time;
         if (completion_message == string.Empty) completion_message = $"Through with bill report creation. {elapsed.ToString("c")} ";
         LogAndDisplay(form1.txtBillUpdatesProgress, completion_message);
      }

      private List<NewBillForDisplay> Candidates() {
         List<BillRow> rows = GlobalData.BillRows
               .Where(n => n.Position.Length == 0)
               .OrderByDescending(n => n.NegativeScore)
               .ToList();
         var bills_without_position = new List<NewBillForDisplay>();
         foreach (var bill in rows) {
            string last_action_date = bill.NegativeScore.ToString();
            bills_without_position.Add(new NewBillForDisplay(bill.Bill, bill.NegativeScore.ToString(), bill.Title));
         }
         return bills_without_position;
      }

      private string ExtractLeadingDate(string str) {
         string result = Regex.Replace(str, @"(\w{3} \d{1,2} \d{4}).*", "$1").ToString();
         return result;
      }
   }
}