using Library;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using Library.Database;
using OpenQA.Selenium.Support.UI;
using Scout2.Report;
using Scout2.Utility;

namespace Scout2.Sequence {
   public class InitializeBillRows : BaseController {
      public void Run(Form1 form1) {
         var start_time = DateTime.Now;
         try {
            LogAndDisplay(form1.txtImportProgress, "Determining most recent version of each bill.");
            // Need to know which lob file describes the most recent version of each bill
            GlobalData.MostRecentEachBill = MostRecentBills.Identify(Config.Instance.BillsFolder);

            // Need to copy GlobalData.Profiles to a temporary, update the temporary, then copy the temporary back.
            // BillRanker.Compute(ref GlobalData.Profiles) results in  
            //    A property or indexer may not be passed as an out or ref parameter.
            //    Property "Profiles" access returns temporary value.
            //    "ref" argument must be an assignable value, field or array element.
            LogAndDisplay(form1.txtImportProgress, "Computing positive and negative scores for all bills.");
            List<BillProfile> mr_profiles = Profiles(GlobalData.MostRecentEachBill);  // Prepare for ranking the bills
            BillRanker.Compute(ref mr_profiles);
            GlobalData.Profiles = mr_profiles;

            LogAndDisplay(form1.txtImportProgress, "Initializing BillRows from BILL_TBL.dat and elsewhere.");
            string path = $"{Path.Combine(Config.Instance.BillsFolder, "BILL_TBL.dat")}";
            BuildBillRowsTable(form1, path, GlobalData.Profiles);
            // Update the database BillRows table
            LogAndDisplay(form1.txtImportProgress, "Clearing and rewriting BillRows in the database.");
            BillRow.WriteRowset(GlobalData.BillRows);
         } catch (Exception ex) {
            LogAndThrow($"InitializeBillRows.Run: {ex.Message}.");
         }
         var elapsed = DateTime.Now - start_time;
         LogAndDisplay(form1.txtImportProgress, $"BillRows initialization complete. Elapsed Time: {elapsed.ToString("c")} ");
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
            result.Add(new BillProfile(item, negative_scores, positive_scores));
         }
         return result;
      }
      /// <summary>
      /// Build GlobalData.BillRows from scratch.
      /// The steps for this are
      ///   1. Import the BILL_TBL.dat data into GlobalData.BillRows.  This fills 9 of the 15 BillRow fields.
      /// </summary>
      /// <param name="form1">The main (Form1) display form. Display messages and progress here.</param>
      /// <param name="path_bill_tbl">Fully-qualified path to BILL_TBL.dat</param>
      /// <param name="mostRecentEachBill">For each bill, the Bill_Identifier identifying the most recent version</param>
      private void BuildBillRowsTable(Form1 form1, string path_bill_tbl, List<BillProfile> profiles) {
         // Import BILL_TBL.dat, which is the legislative site's information on bills before the legislature.
         // Trim all non-bill items during the import -- we want only type AB and SB (Assembly and Senate Bills)
         var bill_table_wrapper = new Bill_Tbl_Wrapper();
         bill_table_wrapper.ReadYourself();     // Import BILL_TBL.dat
         List<BillRow> rows_with_positions = BillRow.RowsetByQuery("Select * from Billrows Where Position <> ''");
         List<BillRow> all_rows = BillRow.RowSet();
         List<string> reports = BillUtils.HtmlFolderContents();

         // Re-create GlobalData.BillRows, using data from bill_table_wrapper and elsewhere.
         GlobalData.BillRows.Clear();
         List<string> SkipIf = new List<String>() 
            { "Chaptered", "Died", "Enrolled", "Failed", "Failed Passage in Committee", "Vetoed"};
         foreach (var item in bill_table_wrapper) {
            // Don't process bills that will not progress further in the legislature.
            //string result = SkipIf.FirstOrDefault(s => s == item.Current_status);
            //if (result != null) continue;

            // Use data from bill_table_wrapper.  Some fields are left blank.
            var bill_row = new BillRow();
            bill_row.MeasureType   = item.Measure_type; // e.g. AB
            bill_row.MeasureNum    = BillUtils.Ensure4DigitNumber(item.Measure_num);  // e.g. 0010
            bill_row.Bill          = $"{bill_row.MeasureType}{BillUtils.EnsureNoLeadingZerosBill(bill_row.MeasureNum)}"; // e.g. AB10
            //bill_row.Lob           = item.;
            //bill_row.NegativeScore = item;
            //bill_row.PositiveScore = item;
            //bill_row.Position      = item; // Hand Entered, e.g. Monitor
            bill_row.BillVersionID = VersionID(item); // e.g. 20190AB199INT
            //bill_row.Author        = item;
            //bill_row.Title         = item;
            bill_row.Location      = item.Current_location;      // e.g. CX08
            bill_row.Location2nd   = item.Current_secondary_loc; // e.g. Committee
            bill_row.MeasureState  = item.Measure_state;         // e.g. Amended Assembly
            bill_row.CurrentHouse  = item.Current_house;         // e.g. Assembly
            bill_row.CurrentStatus = item.Current_status;        // e.g. In Committee Process

            // Obtain the author, title, lob file path, and positive/negative scores from the profile for this bill
            var four_digit_billid = BillUtils.Ensure4DigitNumber(bill_row.Bill);
            var profile = (from x in profiles where x.Identifier.BillID == four_digit_billid select x).First();
            if (profile != null) {
               bill_row.Author        = profile.Identifier.Author;
               bill_row.Title         = profile.Identifier.Title;
               bill_row.Lob           = profile.Identifier.LobPath;
               bill_row.NegativeScore = profile.NegScore;
               bill_row.PositiveScore = profile.PosScore;
            } else {
               throw new ApplicationException($"InitializeBillRows.BuildBillRowsTable: Bill {bill_row.Bill} is present in bill_table_wrapper, but not in GlobalData.Profiles.");
            }

            // Fill in the Position data -- the position we are taking on this bill.  If we have a position, it is
            // in one of two places
            //    1.  The database BillRows table (not all bills have a report), or
            var pos = (from x in rows_with_positions where x.Bill == bill_row.Bill select x).FirstOrDefault();
            if (pos != null) bill_row.Position = pos.Position;
            //    2.  If that table hasn't been updated, in the actual report
            // If the two are in conflict, the bill report wins.
            var short_id = BillUtils.EnsureNoLeadingZerosBill(bill_row.Bill);
            var report_file = (from x in reports where x.Contains($"{short_id}.html") select x).FirstOrDefault();
            if (report_file != null) {
               var report = new BillReport(report_file);
               bill_row.Position = report.Position;
            }
            // Add this row to GlobalData.BillRows
            GlobalData.BillRows.Add(bill_row);
         }
         
         // Sort the table before returning it.  Ordered by bill ID, e.g. AB0001, communicates well
         GlobalData.BillRows = GlobalData.BillRows.OrderBy(a => a.Bill).ToList();
      }
      /// <summary>
      /// Extract the version ID (99, 98, 97...) from the Latest_bill_version_id given in BILL_TBL.dat.
      /// </summary>
      /// <param name="item">The BILL_TBL.dat data for a bill.</param>
      /// <returns></returns>
      private string VersionID(BillTableRow item) {
         string rx = "\\d{4}0" + item.Measure_type + item.Measure_num + "(\\d{2})" + ".*";
         string result = Regex.Replace(item.Latest_bill_version_id, rx, "$1");
         return result;
      }
   }
}