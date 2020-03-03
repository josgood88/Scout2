using Scout2.Utility;
using System;
using Scout2.Report;
using Xunit;

namespace Scout.Tests {
   public class TestDateUtils {
      /// <summary>
      /// 
      /// </summary>
      /// <param name="is_bad_date">1 if this_date is an invalid date, 0 otherwise</param>
      /// <param name="this_date">the date being tested</param>
      [Theory]
      [InlineData(1, "garbage")]
      [InlineData(0, "2/16/2020")]
      [InlineData(0, "16 Feb 2020")]
      [InlineData(0, "16 February 2020")]
      public void TestDate(int is_bad_date, string this_date) {
         bool whether_bad_date = is_bad_date != 0;
         try {
            string acceptable_string = DateUtils.Date(this_date);
            DateTime dt = DateTime.Parse(acceptable_string);
         } catch (ApplicationException ex) {
            if (ex.Message.Contains("Invalid date")) Assert.True(whether_bad_date);
            else throw;
         }
      }

      // NextMonday_private is not tested with garbage data because its only caller passes DateTime.Now.
      // Because NextMonday_private is only called with DateTime.Now, it is not expected to throw
      [Theory]
      [InlineData("2/26/2019", "3/4/2019")]
      [InlineData("2/27/2019", "3/4/2019")]
      [InlineData("2/28/2019", "3/4/2019")]
      [InlineData("3/01/2019", "3/4/2019")]
      [InlineData("3/02/2019", "3/4/2019")]
      [InlineData("3/03/2019", "3/4/2019")]
      [InlineData("3/04/2019", "3/4/2019")]
      //
      [InlineData("2/25/2020", "3/2/2020")]
      [InlineData("2/26/2020", "3/2/2020")]
      [InlineData("2/27/2020", "3/2/2020")]
      [InlineData("2/28/2020", "3/2/2020")]
      [InlineData("2/29/2020", "3/2/2020")]
      [InlineData("3/01/2020", "3/2/2020")]
      [InlineData("3/02/2020", "3/2/2020")]
      public void TestNextMonday(string date, string followingMonday) {
         DateTime test_data = DateTime.Parse(date);
         DateTime correct_answer = DateTime.Parse(followingMonday);
         DateTime result = DateUtils.NextMonday_private(test_data);
         Assert.True(result == correct_answer);
      }

      // LastMonday_private is like NextMonday_private in that it is not tested with garbage data
      // because its only caller passes DateTime.Now.
      // Because LastMonday_private is only called with DateTime.Now, it is not expected to throw
      [Theory]
      [InlineData("2/26/2019", "2/25/2019")]
      [InlineData("2/27/2019", "2/25/2019")]
      [InlineData("2/28/2019", "2/25/2019")]
      [InlineData("3/01/2019", "2/25/2019")]
      [InlineData("3/02/2019", "2/25/2019")]
      [InlineData("3/03/2019", "2/25/2019")]
      [InlineData("3/04/2019", "2/25/2019")]
      //
      [InlineData("2/25/2020", "2/24/2020")]
      [InlineData("2/26/2020", "2/24/2020")]
      [InlineData("2/27/2020", "2/24/2020")]
      [InlineData("2/28/2020", "2/24/2020")]
      [InlineData("2/29/2020", "2/24/2020")]
      [InlineData("3/01/2020", "2/24/2020")]
      [InlineData("3/02/2020", "2/24/2020")]
      public void TestLastMonday(string date, string followingMonday) {
         DateTime test_data = DateTime.Parse(date);
         DateTime correct_answer = DateTime.Parse(followingMonday);
         DateTime result = DateUtils.LastMonday_private(test_data);
         Assert.True(result == correct_answer);
      }

      [Theory]
      [InlineData(1, "2/10/2020", "2/10/2020", "2/16/2020")]
      [InlineData(1, "2/11/2020", "2/10/2020", "2/16/2020")]
      [InlineData(1, "2/12/2020", "2/10/2020", "2/16/2020")]
      [InlineData(1, "2/13/2020", "2/10/2020", "2/16/2020")]
      [InlineData(1, "2/14/2020", "2/10/2020", "2/16/2020")]
      [InlineData(1, "2/15/2020", "2/10/2020", "2/16/2020")]
      [InlineData(1, "2/16/2020", "2/10/2020", "2/16/2020")]
      public void TestDateIsInPastWeek(int is_in_range, string this_date, string start, string end) {
         DateTime dt = DateTime.Parse(this_date);  // Test data assumed to have valid dates
         var range = new Report.DateRange(start, end);
         bool result = DateUtils.DateIsInPastWeek(dt, range);
         Assert.True(result == (is_in_range != 0));
      }
   }
}
