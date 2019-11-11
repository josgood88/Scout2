using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Library.Database {
   public class BaseTables {
      protected const int SQL_INSERT_ROW_LIMIT = 1000-1; // SQL Server bulk insert limit is 1000 rows

      protected List<string> ReadDatFile(string path) {
         var result = new List<string>();
         using (StreamReader sr = new StreamReader(path)) {
            while (!sr.EndOfStream) { result.Add(sr.ReadLine()); }
         }
         return result;
      }
   }
}
