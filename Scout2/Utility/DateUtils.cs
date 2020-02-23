using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scout2.Utility {
   public class DateUtils {
      /// <summary>
      /// Parse an input string purporting to represent a date and time.
      /// If the input string is valid, return the data in string form, e.g. "04 July 1776"
      /// Otherwise throw ApplicationException 
      /// </summary>
      /// <param name="date_time_string">Convert this string</param>
      /// <returns>date in string form, e.g. "04 July 1776"</returns>
      public static string Date(string date_time_string) {
         string result;
         if (DateTime.TryParse(date_time_string, out DateTime dt)) result = dt.ToString("dd MMMM yyyy");
         else throw new ApplicationException($"DateUtils:Date(string): Invalid date in {date_time_string}");
         return result;
      }
      /// <summary>
      /// Report date is always the Monday following today
      /// </summary>
      /// <returns></returns>
      public static DateTime NextMonday() {
         Dictionary<int, int> incrementTodayToReportDate = new Dictionary<int, int>() {
         // Sun    Mon    Tue    Wed    Thu    Fri    Sat
            {0,1}, {1,0}, {2,6}, {3,5}, {4,4}, {5,3}, {6,2} };
         var now = DateTime.Now;
         int dayOfWeek = (int)now.DayOfWeek; // 0 = Sunday
         int increment_by = incrementTodayToReportDate[dayOfWeek];
         var next_monday = now.AddDays(increment_by);
         return next_monday;
      }
      /// <summary>
      /// Last Report date was the Monday preceeding today
      /// </summary>
      /// <returns></returns>
      /// <summary>
      /// Last Report date was the Monday preceeding today
      /// </summary>
      /// <returns></returns>
      public static DateTime LastMonday() {
         Dictionary<int, int> incrementTodayToReportDate = new Dictionary<int, int>() {
         // Sun    Mon    Tue    Wed    Thu    Fri    Sat
            {0,1}, {1,0}, {2,6}, {3,5}, {4,4}, {5,3}, {6,2} };
         var now = DateTime.Now;
         int dayOfWeek = (int)now.DayOfWeek; // 0 = Sunday
         int increment_by = incrementTodayToReportDate[dayOfWeek] - 7;
         var next_monday = now.AddDays(increment_by);
         return next_monday;
      }

      public static bool DateIsInPastWeek(DateTime dt, Report.Report.DateRange range) {
         return dt >= range.start && dt <= range.end;
      }
   }
}
