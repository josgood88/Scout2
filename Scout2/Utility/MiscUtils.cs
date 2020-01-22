using System;
using System.Collections.Generic;

namespace Scout2.Utility {
   public class MiscUtils {
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
   }
}