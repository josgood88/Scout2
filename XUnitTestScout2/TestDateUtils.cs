using Scout2.Utility;
using System;
using Scout2.Report;
using Xunit;

namespace Scout.Tests {
   public class TestDateUtils {
      //public static IEnumerable<object[]> getNextMondayTest() {
      //   var tests = new List<Tuple<string, string>> {
      //      new Tuple<string,string>("2/16/2020","2/17/2020"),
      //      new Tuple<string,string>("2/17/2020","2/24/2020"),
      //      new Tuple<string,string>("2/18/2020","2/24/2020"),
      //      new Tuple<string,string>("2/19/2020","2/24/2020"),
      //      new Tuple<string,string>("2/20/2020","2/24/2020"),
      //      new Tuple<string,string>("2/21/2020","2/24/2020"),
      //      new Tuple<string,string>("2/22/2020","2/24/2020")
      //   };
      //   foreach (var test in tests) yield return test;
      //}


      [Theory]
      [InlineData(1, "garbage")]
      [InlineData(0, "2/16/2020")]
      [InlineData(0, "16 Feb 2020")]
      [InlineData(0, "16 February 2020")]
      public void TestDate(int bad_date, string this_date) {
         bool is_bad_date = bad_date != 0;
         try {
            string acceptable_string = DateUtils.Date(this_date);
            DateTime dt = DateTime.Parse(acceptable_string);
         } catch (ApplicationException ex) {
            if (ex.Message.Contains("Invalid date")) Assert.True(is_bad_date);
            else throw;
         }
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
         Report.DateRange range = new Report.DateRange();
         DateTime dt = DateTime.Parse(this_date);  // Test data assumed to have valid dates
         range.start = DateTime.Parse(start);
         range.end   = DateTime.Parse(end);
         bool result = DateUtils.DateIsInPastWeek(dt, range);
         Assert.True(result == (is_in_range != 0));
      }
   }
}
