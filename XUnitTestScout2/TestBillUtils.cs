using System;
using System.Collections.Generic;
using System.IO;
using Library;
using Xunit;
using Scout2.WeeklyReport;
using Scout2.Sequence;
using Scout2.Utility;

namespace Scout.Tests {
   public class TestBillUtils {

      //=====================NoDash=====================
      [Theory]
      [InlineData(null, null)]
      [InlineData("", "")]
      [InlineData("-", "")]
      [InlineData("abc", "abc")]
      [InlineData("a-b-c", "abc")]
      public void TestNoDash(string input, string right_answer) {
         Assert.True(BillUtils.NoDash(input) == right_answer);
      }

      //=====================Ensure4DigitNumber (throw)=====================
      [Theory]
      [InlineData(null)]
      [InlineData("")]
      [InlineData("AB")]
      [InlineData("SB")]
      [InlineData("AB12345")]
      [InlineData("AB-12345")]
      public void TestEnsure4DigitNumberThrow(string input) {
         Assert.Throws<ApplicationException>(() => BillUtils.Ensure4DigitNumber(input));
      }

      //=====================Ensure4DigitNumber (no throw)=====================
      [Theory]
      [InlineData("AB1", "AB0001")]
      [InlineData("AB12", "AB0012")]
      [InlineData("AB123", "AB0123")]
      [InlineData("AB1234", "AB1234")]
      [InlineData("AB-1", "AB0001")]
      [InlineData("AB-12", "AB0012")]
      [InlineData("AB-123", "AB0123")]
      [InlineData("AB-1234", "AB1234")]
      public void TestEnsure4DigitNumberNoThrow(string input, string right_answer) {
         Assert.True(BillUtils.Ensure4DigitNumber(input) == right_answer);
      }

      //=====================EnsureNoLeadingZerosBill=====================
      [Theory]
      [InlineData(null, "")]
      [InlineData("", "")]
      [InlineData("AB0001", "AB1")]
      [InlineData("AB0012", "AB12")]
      [InlineData("AB0123", "AB123")]
      [InlineData("AB1234", "AB1234")]
      [InlineData("AB-1", "AB1")]
      [InlineData("AB-12", "AB12")]
      [InlineData("AB-123", "AB123")]
      [InlineData("AB-1234", "AB1234")]
      public void TestEnsureNoLeadingZerosBill(string input, string right_answer) {
         var result = BillUtils.EnsureNoLeadingZerosBill(input);
         Assert.True(result == right_answer);
      }

      //=====================EnsureDashAndNoLeadingZeros=====================
      [Theory]
      [InlineData(null, "")]
      [InlineData("", "")]
      [InlineData("AB0001", "AB-1")]
      [InlineData("AB0012", "AB-12")]
      [InlineData("AB0123", "AB-123")]
      [InlineData("AB1234", "AB-1234")]
      [InlineData("AB-1", "AB-1")]
      [InlineData("AB-12", "AB-12")]
      [InlineData("AB-123", "AB-123")]
      [InlineData("AB-1234", "AB-1234")]
      [InlineData("AB-01", "AB-1")]
      [InlineData("AB-012", "AB-12")]
      [InlineData("AB-0123", "AB-123")]
      [InlineData("AB-01234", "AB-1234")]
      public void TestEnsureDashAndNoLeadingZeros(string input, string right_answer) {
         var result = BillUtils.EnsureDashAndNoLeadingZeros(input);
         Assert.True(result == right_answer);
      }

      //=====================ExtractHouseNumber=====================
      [Theory]
      [InlineData(0, null, null, null)]
      [InlineData(0, "", "", "")]
      [InlineData(1, "AB0001", "AB", "0001")]
      [InlineData(1, "AB0012", "AB", "0012")]
      [InlineData(1, "AB0123", "AB", "0123")]
      [InlineData(1, "AB1234", "AB", "1234")]
      [InlineData(1, "AB-0001", "AB", "0001")]
      [InlineData(1, "AB-0012", "AB", "0012")]
      [InlineData(1, "AB-0123", "AB", "0123")]
      [InlineData(1, "AB-1234", "AB", "1234")]
      [InlineData(1, "AB-01", "AB", "01")]
      [InlineData(1, "AB-012", "AB", "012")]
      public void TestExtractHouseNumber(int right_bool, string input, string right_house, string right_number) {
         bool desired = right_bool != 0 ? true : false;
         var result = BillUtils.ExtractHouseNumber(input, out string house, out string number);
         Assert.True(result == desired);
         if (right_house != null) Assert.True(house == right_house);
         if (right_number != null) Assert.True(number == right_number);
      }

      //=====================IsNewThisWeek=====================
      [Theory]
      [InlineData(0, "2/23/2020", "2/24/2020", "3/1/2020")]
      [InlineData(0, "3/2/2020",  "2/24/2020", "3/1/2020")]
      [InlineData(1, "2/24/2020", "2/24/2020", "3/1/2020")]
      [InlineData(1, "2/25/2020", "2/24/2020", "3/1/2020")]
      [InlineData(1, "2/26/2020", "2/24/2020", "3/1/2020")]
      [InlineData(1, "2/27/2020", "2/24/2020", "3/1/2020")]
      [InlineData(1, "2/28/2020", "2/24/2020", "3/1/2020")]
      [InlineData(1, "2/29/2020", "2/24/2020", "3/1/2020")]
      [InlineData(1, "3/1/2020",  "2/24/2020", "3/1/2020")]
      public void TestIsNewThisWeek(int new_week, string report_date, string week_start, string week_end) {
         string report_contents = $"   <b>Summary</b>: (Reviewed {report_date})";
         var range = new Scout2.WeeklyReport.WeeklyReport.DateRange(week_start, week_end);
         bool is_new_week = new_week != 0;
         bool test = is_new_week == BillUtils.IsNewThisWeek(report_contents, range);
         Assert.True(is_new_week == BillUtils.IsNewThisWeek(report_contents, range));
      }

      //=====================DateOfInitialReview=====================
      [Theory]
      [InlineData(1, "12/15/2018", "   <b>Summary</b>: (Reviewed 12/15/2018)")]
      [InlineData(0, "01/01/0001", null)]
      [InlineData(0, "01/01/0001", "")]
      [InlineData(0, "01/01/0001", " ")]
      [InlineData(0, "01/01/0001", "abc")]
      public void TestDateOfInitialReview(int is_valid, string right_answer, string report_contents) {
         DateTime.TryParse(right_answer, out DateTime correct);
         try {
            var result = BillUtils.DateOfInitialReview(report_contents);
            if (is_valid == 0) Assert.True(false, $"Unexpected DateOfInitialReview success parsing {report_contents}");
            Assert.True(correct.Year==result.Year && correct.Month==result.Month&& correct.Day==result.Day);
         } catch (Exception ) {
            if (is_valid == 1) Assert.True(false, $"Unexpected DateOfInitialReview failure parsing {report_contents}");
         }
      }

      //=====================DateNewOrChangePrefix=====================
      // This test depends on actual bill reports being present in the TestData folder, which is a subfolder of
      // the folder which contains Scout.Tests.csproj.
      [Theory]
      [InlineData("New",    "02/24/2020", "03/01/2020", "../../../TestData/SB1200WeekEnding20200308.html")]
      [InlineData("Update", "03/16/2020", "03/22/2020", "../../../TestData/AB1976WeekEnding20200315.html")]
      [InlineData("None",   "03/16/2020", "03/22/2020", "../../../TestData/AB3285WeekEnding20200315.html")]
      public void TestNewOrChangePrefix(string right_answer, string week_start, string week_end, string report_file_path) {
         if (File.Exists(report_file_path)) {
            BaseController.EnsureGlobalData();  // Report constructor requires GlobalHistoryTable
            var range = new Scout2.WeeklyReport.WeeklyReport.DateRange(week_start, week_end);
            var report = new BillReport(report_file_path);
            var answer = BillUtils.NewOrChangePrefix(range, report);
            switch (right_answer) {
               case "New":
                  Assert.True(answer.Contains("NEW"), $"TestNewOrChangePrefix: {answer} is not correct, should contain NEW.");
                  break;
               case "Update":
                  Assert.True(answer.Contains("UPDATED"), $"TestNewOrChangePrefix: {answer} is not correct, should contain UPDATED.");
                  break;
               case "None":
                  Assert.True(answer.Length == 0, $"TestNewOrChangePrefix: {answer} is not correct, should be of 0 length.");
                  break;
               default:
                  Assert.True(false, $"TestNewOrChangePrefix: {answer} is not a valid NewOrChangePrefix response.");
                  break;
            }
         } else {
            Assert.True(false, $"TestNewOrChangePrefix: {report_file_path} does not exist.");
         }
      }

      //=====================CheckManualUpdate=====================
      [Theory]
      //                        
      [InlineData("", "AB1122", null)]
      [InlineData("", "AB1122", "")]
      [InlineData("", "AB1122", " ")]
      [InlineData("", "AB1122", "ABC")]
      [InlineData("",   "AB1122", "AB99")]
      [InlineData("Manual", "AB1122", "AB1122")]
      public void TestCheckManualUpdate(string right_answer, string measure, string testCommittees) {
         // Customize the list of manual updates
         var cache = Config.Instance.ManualCommitteeChanges;
         if (testCommittees is null) Config.Instance.ManualCommitteeChanges = null;
         else Config.Instance.ManualCommitteeChanges = new List<string>() { testCommittees };
         // Answer whether this bill contains a manual update
         var answer = BillUtils.CheckManualUpdate(measure);
         switch (right_answer) {
            case "Manual":
               Assert.True(answer.Contains("MANUAL"), $"TestCheckManualUpdate: {answer} is not correct, should contain MANUAL.");
               break;
            case "":
               Assert.True(answer.Length == 0, $"TestCheckManualUpdate: {answer} is not correct, should be of 0 length.");
               break;
            default:
               Assert.True(false, $"TestCheckManualUpdate: {answer} is not a valid CheckManualUpdate response.");
               break;
         }
         // Restore the production list of manual updates.
         Config.Instance.ManualCommitteeChanges = cache;
      }

      //=====================DateFromLastAction=====================
      // This test depends on actual bill reports being present in the TestData folder, which is a subfolder of
      // the folder which contains Scout.Tests.csproj.
      [Theory]
      [InlineData("03/05/2020", "../../../TestData/SB1200WeekEnding20200308.html")]
      [InlineData("03/17/2020", "../../../TestData/AB1976WeekEnding20200315.html")]
      [InlineData("03/09/2020", "../../../TestData/AB3285WeekEnding20200315.html")]
      public void TestDateFromLastAction(string right_answer, string report_file_path) {
         if (File.Exists(report_file_path)) {
            BaseController.EnsureGlobalData();  // Report constructor requires GlobalHistoryTable
            var report = new BillReport(report_file_path);
            var correct = DateTime.Parse(right_answer);
            var answer = BillUtils.DateFromLastAction(report);
            Assert.True(correct.Year == answer.Year && correct.Month == answer.Month && correct.Day == answer.Day, 
                        $"TestNewOrChangePrefix: {answer.ToShortDateString()} is not correct, should be {correct.ToShortDateString()}.");
         } else {
            Assert.True(false, $"TestNewOrChangePrefix: {report_file_path} does not exist.");
         }
      }

      //=====================WhenNullLocationCode=====================
      // WhenNullLocationCode isn't unit tested.  It is simple enough that inspection is deemed sufficient.
   }
}
