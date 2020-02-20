using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Library.Database {
   public class BillHistoryTable : BaseTables {
      public static string table_Name = "BillHistoryTable";
      public List<BillHistoryRow> Table { get; set; }

      private static string createTableQuery =
         $"Create Table If Not Exists [{table_Name}] (" +
            "[BillID]            Text NOT NULL," +
            "[BillHistoryID]     Text NULL PRIMARY KEY,"  +
            "[ActionDate]        Text NULL,"  +
            "[Action]            Text NULL,"  +
            "[Trans_UID]         Int  NULL, " +
            "[TransUpdateDate]   Int  NULL, " +
            "[ActionSequence]    Text NULL, " +
            "[ActionCode]        Text NULL, " +
            "[ActionStatus]      Text NULL, " +
            "[PrimaryLocation]   Text NULL, " +
            "[SecondaryLocation] Text NULL, " +
            "[TernaryLocation]   Text NULL, " +
            "[EndStatus]         Text NULL  " +
            ")";

      public BillHistoryTable(string path) {
         if (File.Exists(path)) {
            List<string> file_rows = ReadDatFile(path);
            Table = ParseBillHistoryDat(file_rows);
         } else {
            throw new ArgumentException($"BillHistoryTable(string path): {path} does not exist.");
         }
      }

      /// Create the BillHistoryTable table if the database doesn't already have such a table
      public static void CreateYourself() { DB.NonQuery(createTableQuery,"BillHistoryTable.CreateYourself"); }

      /// Delete the BillHistoryTable table
      public static void DeleteYourself() { DB.NonQuery($"Drop Table [{table_Name}]","BillHistoryTable.DeleteYourself"); }

      /// Clear the BillHistoryTable table
      public static void ClearYourself() { DB.NonQuery($"Delete From [{table_Name}]","BillHistoryTable.DeleteYourself"); }

      List<BillHistoryRow> ParseBillHistoryDat(List<string> file_rows) {
         var result = new List<BillHistoryRow>();
         foreach (string file_row in file_rows) {
            List<string> columns = BillHistoryRow.SplitIntoColumns(file_row);
            result.Add(new BillHistoryRow(columns));
         }
         return result;
      }

      public List<BillHistoryRow> RowSet(string bill_id) {
         var result = (from item in Table where (item.BillID == bill_id) select item).ToList();
         result.Sort((a, b) => a.SequenceAsInt().CompareTo(b.SequenceAsInt()));
         return result;
      }

      public BillHistoryRow LatestFromHouseNumber(string bill_id) {
         var result = (from item in Table where (item.BillID.EndsWith(bill_id)) select item).ToList();
         result.Sort((a, b) => a.SequenceAsInt().CompareTo(b.SequenceAsInt()));
         return result.Last();
      }
   }
}
