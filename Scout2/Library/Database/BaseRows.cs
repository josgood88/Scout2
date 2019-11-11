using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Library.Database {
   public class BaseRows {

      protected string NQ(String item) { return item.Trim('"'); }
      protected string MQ(String item) { return item.Trim((char)0x60); }

      public static List<string> SplitIntoColumns(string row) {
         List<string> result = new List<string>();
         result = Regex.Split(row,"\t").ToList();
         return result;
      }
   }
}
