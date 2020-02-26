using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Library;
using Scout2.Report;

namespace Scout2.Utility {
   public class BillUtils : CommonUtils {
      /// <summary>
      /// Remove any dashes from the passed string.
      /// </summary>
      /// <param name="str"></param>
      /// <returns></returns>
      public static string NoDash(string str) {
         return str == null ? null : Regex.Replace(str, "-", string.Empty);
      }

      /// <summary>
      /// Ensure the Measure/BillID has a 4-digt number, e.g. AB0123.
      /// In the database BillRow table, BillID uses a 4-digit number so that when sorted by BillID, the sorting
      /// comes out as expected -- AB0003 preceeds AB0010.
      /// </summary>
      /// <param name="bill">Bill house & measure, e.g. AB123</param>
      /// <returns>Bill house & measure, 4-digit measure ensured</returns>
      public static string Ensure4DigitNumber(string bill) {
         if (string.IsNullOrEmpty(bill))
            throw new ApplicationException("BillUtils.Ensure4DigitNumber: Bill is null or empty.");
         ExtractHouseNumber(NoDash(bill), out string house, out string number);
         if (string.IsNullOrEmpty(house))
            throw new ApplicationException("BillUtils.Ensure4DigitNumber: {bill} does not specify chamber.");
         if (string.IsNullOrEmpty(number))
            throw new ApplicationException("BillUtils.Ensure4DigitNumber: {bill} does not specify bill number.");
         if (number.Length > 4)
            throw new ApplicationException("BillUtils.Ensure4DigitNumber: {bill} bill number is too long {(number.Length)}.");
         while (number.Length < 4) number = $"0{number}";
         return $"{house}{number}";
      }

      /// <summary>
      /// Ensure the Measure/BillID has no leading zeroes, e.g. AB123 instead of AB0123.
      /// Report names are given as AB123.html, not AB0123.html
      /// </summary>
      /// <param name="bill">Bill house & measure, e.g. AB123</param>
      /// <returns>Bill house & measure, no-leading-zeros measure ensured</returns>
      public static string EnsureNoLeadingZerosBill(string bill) {
         return IsNullOrEmptyOrWhiteSpace(bill) ? string.Empty : Regex.Replace(NoDash(bill), "B0+", "B");
      }

      /// <summary>
      /// Ensure the Measure/BillID has no leading zeroes, e.g. AB-123 instead of AB-0123
      /// and that the Measure/BillID has includes a dash.
      /// </summary>
      /// <param name="bill">Bill house & measure, e.g. AB123</param>
      /// <returns>Bill house & measure, 4-digit measure ensured</returns>
      public static string EnsureDashAndNoLeadingZeros(string bill) {
         if (IsNullOrEmptyOrWhiteSpace(bill)) return string.Empty;
         var no_leading_zeros = EnsureNoLeadingZerosBill(bill);
         ExtractHouseNumber(NoDash(no_leading_zeros), out string house, out string number);
         return $"{house}-{number}";
      }

      /// <summary>
      /// Extract house and number from bill id, returning house and number through argument references
      /// </summary>
      /// <param name="_bill">Bill house & measure</param>
      /// <param name="house">just the house, ma'am</param>
      /// <param name="number">just the number, ma'am</param>
      /// <returns></returns>
      public static bool ExtractHouseNumber(string _bill, out string house, out string number) {
         house = number = string.Empty;
         string bill = string.Empty;
         if (IsNotNullOrEmptyOrWhiteSpace(_bill)) {
            bill = NoDash(_bill);      // Don't hoist, want Null/Empty protection
            house  = Regex.Match(bill, @"\D*").Value;
            number = bill.Substring(house.Length);
         }
         bool correct = house != string.Empty && house != bill && number != string.Empty && number != bill;
         return correct;
      }
      /// <summary>
      /// Returns an enumeration of the *.html files which are the individual bill reports and the weekly report.
      /// This is not unit testing.  It is too simple to require testing and the necessary mock isn't worth the effort.
      /// </summary>
      /// <returns></returns>
      public static List<string> HtmlFolderContents() {
         return Directory.EnumerateFiles(Config.Instance.HtmlFolder, "*.html").ToList();
      }
      /// <summary>
      /// Answers whether an individual bill report appears for the first time this week.
      /// The bill review date is given on the summary line, e.g. Summary: (Reviewed 1/19/2019) 
      /// </summary>
      /// <param name="report_contents">The contents of the individual bill report in question</param>
      /// <param name="past_week">The starting and ending dates that define last week.</param>
      /// <returns></returns>
      public static bool IsNewThisWeek(string report_contents, Report.Report.DateRange past_week) {
         DateTime dt = DateOfInitialReview(report_contents);
         return DateUtils.DateIsInPastWeek(dt, past_week);
      }
      /// <summary>
      /// Returns the date this biennium when an individual bill report was first written.
      /// </summary>
      /// <param name="report_contents"></param>
      /// <returns></returns>
      public static DateTime DateOfInitialReview(string report_contents) {
         string s1 = Regex.Match(report_contents, @"\(Reviewed.*\)").ToString();
         string text_date = Regex.Replace(s1, @".Reviewed\s+(.*)\)", "$1");
         return (DateTime.TryParse(text_date, out DateTime result)) ? result : default(DateTime);
      }
      /// <summary>
      /// Given a bill report, returns the contents of the report as a single string.
      /// </summary>
      /// <param name="report">BillReport telling chamber and bill number, specifying bill, allowing bill report to be read</param>
      /// <returns></returns>
      public static string ContentsFromBillReport(BillReport report) {
         string path = $"{Path.Combine(Config.Instance.HtmlFolder, BillUtils.EnsureNoLeadingZerosBill(report.Measure))}.html";
         return FileUtils.FileContents(path);
      }
   }
}