using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Library.Database {
   public class LocationCodeTable : BaseTables {
      public static string table_Name = "LocationCodeTable";
      public List<LocationCodeRow> Table { get; set; }

      private static string createTableQuery =
         $"Create Table If Not Exists [{table_Name}] (" +
            "[SessionYear]         Text NULL," +
            "[LocationCode]        Text NULL," +
            "[LocationType]        Text NULL," +
            "[ConsentCalendarCode] Text NULL," +
            "[Description]         Text NULL," +
            "[LongDescription]     Text NULL," +
            "[ActiveFlg]           Text NULL," +
            "[TransUID]            Text NULL," +
            "[TransUpdate]         Text NULL," +
            "[InactiveFileFlg]     Text NULL " +
            ")";

      public LocationCodeTable(string path) {
         if (File.Exists(path)) {
            List<string> file_rows = ReadDatFile(path);
            Table = ParseLocationCodeDat(file_rows);
         } else {
            throw new ArgumentException($"LocationCodeTable(string path): {path} does not exist.");
         }
      }

      /// Create the LocationCodeTable table if the database doesn't already have such a table
      public static void CreateYourself() { DB.NonQuery(createTableQuery,"LocationCodeTable.CreateYourself"); }

      /// Delete the LocationCodeTable table
      public static void DeleteYourself() { DB.NonQuery($"Drop Table [{table_Name}]","LocationCodeTable.DeleteYourself"); }

      /// Clear the LocationCodeTable table
      public static void ClearYourself() { DB.NonQuery($"Delete From [{table_Name}]","LocationCodeTable.DeleteYourself"); }

      List<LocationCodeRow> ParseLocationCodeDat(List<string> file_rows) {
         var result = new List<LocationCodeRow>();
         foreach (string file_row in file_rows) {
            List<string> columns = LocationCodeRow.SplitIntoColumns(file_row);
            result.Add(new LocationCodeRow(columns));
         }
         return result;
      }

      public LocationCodeRow Scalar(string location_code) {
         var result = (from item in Table where (item.LocationCode == location_code) select item).FirstOrDefault();
         return result;
      }
   }
}
