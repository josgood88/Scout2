using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scout2.Utility {
   public class DateUtils {

      public static string Date(string date_time_string) {
         string result;
         if (DateTime.TryParse(date_time_string, out DateTime dt)) result = dt.ToString("dd MMMM yyyy");
         else throw new ApplicationException($"DateUtils:Date(string): Invalid date in {date_time_string}");
         return result;
      }
   }
}
