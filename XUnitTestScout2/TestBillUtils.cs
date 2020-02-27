using System;
using Xunit;
using Scout2.Report;
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
      [InlineData("12")]
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
         var range = new Report.DateRange(week_start, week_end);
         bool is_new_week = new_week != 0;
         bool test = is_new_week == BillUtils.IsNewThisWeek(report_contents, range);
         Assert.True(is_new_week == BillUtils.IsNewThisWeek(report_contents, range));
      }
   }
}
