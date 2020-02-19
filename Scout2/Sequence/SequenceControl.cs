using System;

namespace Scout2.Sequence {
   /// <summary>
   /// The central design element of Scout2 is the sequence of events that it follows.
   /// This sequence is unchanging, but can be entered anywhere in the sequence.
   /// This class defines the sequence and its entry points.
   /// </summary>
   public class SequenceControl : BaseController {
      public static void ImportFromLegSite (Form1 form) { Run(form, SeqPoint.importFromLegSite); }
      public static void ExtractFromZip    (Form1 form) { Run(form, SeqPoint.extractFromZip); }
      public static void ImportToDB        (Form1 form) { Run(form, SeqPoint.importToDB); }
      public static void UpdateBillReports (Form1 form) { Run(form, SeqPoint.updateBillReports); }
      public static void CreateBillReports (Form1 form) { Run(form, SeqPoint.createBillReports); }
      public static void RegenBillReports  (Form1 form) { Run(form, SeqPoint.regenBillReports); }
      public static void WeeklyReport      (Form1 form) { Run(form, SeqPoint.weeklyReport); }
      /// <summary>
      /// Define the main sequence.  The definitions in this enum MUST be in the main sequence order
      /// because the Run method increments to find the next step.
      /// Both ways of writing this have a problem.  My way has a Run sequence dependency on SeqPoint and
      /// the only thing keeping the two in sync is a comment.  But the Run coding is easy to understand.
      /// Setting seq explicitly (e.g., seq = SeqPoint.extractFromZip) localizes the dependency, but the
      /// Run code isn't as obvious.
      /// At least, that's how I see it.  Your milage may vary.
      ///
      /// This sequence should also be in sync with the Form1 display.
      /// Not required, but looks better.
      /// </summary>
      private enum SeqPoint { importFromLegSite, extractFromZip, importToDB,
         regenBillReports, updateBillReports, createBillReports, weeklyReport,
         complete
      };
      /// <summary>
      /// Run the program through the main sequence.
      /// Note that each case simply increments the seq SeqPoint.
      /// It is debateable whether this is the more readable way to write this.
      /// I think it is.
      /// </summary>
      /// <param name="form">The form on which these controls are displayed</param>
      /// <param name="seq">Next step on the main sequence</param>
      private static void Run(Form1 form, SeqPoint seq) {
         bool are_table_files_present = BeforeEnteringMainSequence(form);  // Initialize before entering the main sequence.
         if (are_table_files_present || seq <= SeqPoint.extractFromZip) {
            var update_form = new UpdatedBillsForm();
            var unreported_form = new UnreportedBillsForm();
            while (seq != SeqPoint.complete) {        // While the main sequence is not complete
               switch (seq) {                         // Perform the current step in the sequence
                  case SeqPoint.importFromLegSite:
                     new LegSiteController().Run(form);  // Download the latest leginfo zip file, which is a zipped file.
                     seq++;
                     break;
                  case SeqPoint.extractFromZip:
                     new ZipController().Run(form);   // Extract the contents of the downloaded zip file.
                     if (!are_table_files_present) {  // If could not fill database from unzipped table files
                        if (!BeforeEnteringMainSequence(form)) {
                           throw new ApplicationException($"SequenceController.Run: Table files missing, cannot initialize database.");
                        }
                     }
                     seq++;
                     break;
                  case SeqPoint.importToDB:
                     new ImportController().Run(form);// Update the database with the latest data on the bill's text, status, committee location, etc.
                     seq++;
                     break;
                  case SeqPoint.regenBillReports:
                     new Regenerate().Run(form);      // Regenerate the individual bill reports.  In particular, update the bill's history
                     seq++;
                     break;
                  case SeqPoint.updateBillReports:
                     new UpdateExistingReports().Run(form, update_form);// User updates existing bill reports
                     seq++;
                     break;
                  case SeqPoint.createBillReports:
                     new CreateNewReports().Run(form, unreported_form);// User creates reports for newly found bills of interest
                     seq++;
                     break;
                  case SeqPoint.weeklyReport:
                     new WeeklyReport().Run(form);    // Generate the weekly report
                     seq++;
                     break;
                  default:
                     throw new ApplicationException($"SequenceControl.Run: Invalid sequence point {seq} encountered.");
               }
            }
         } else {
            string msg = "At least one of BILL_HISTORY_TBL.dat, BILL_VERSION_TBL.dat, LOCATION_CODE_TBL.dat are not present.";
            LogAndShow(msg);
            throw new ApplicationException($"SequenceControl.Run: {msg}");
         }
      }
   }
}