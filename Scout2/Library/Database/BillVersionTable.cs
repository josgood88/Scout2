using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Library.Database {
   public class BillVersionTable : BaseTables {
      public List<BillVersionRow> Table { get; set; }
      public static string table_Name = "BillVersionTable";

      private static string createTableQuery =
         $"Create Table If Not Exists [{table_Name}] (" +
            "[BillVersionID]         Text NOT NULL PRIMARY KEY," +
            "[BillID]                Text NULL," +
            "[VersionNum]            Text NULL," +
            "[BillVersionActionDate] Text NULL," +
            "[BillVersionAction]     Int  NULL, " +
            "[RequestNum]            Int  NULL, " +
            "[Subject]               Text NULL, " +
            "[VoteRequired]          Text NULL, " +
            "[Appropriation]         Text NULL, " +
            "[FiscalCommittee]       Text NULL, " +
            "[LocalProgram]          Text NULL, " +
            "[SubstantiveChanges]    Text NULL, " +
            "[Urgency]               Text NULL, " +
            "[TaxLevy]               Text NULL, " +
            "[BillXml]               Text NULL, " +
            "[ActiveFlag]            Text NULL, " +
            "[TransUID]              Text NULL, " +
            "[TransUpdate]           Text NULL"   +
            ")";

      public BillVersionTable(string path) {
         if (File.Exists(path)) {
            List<string> file_rows = ReadDatFile(path);
            Table = ParseBillVersionDat(file_rows);
         } else {
            throw new ArgumentException($"BillVersionTable(string path): {path} does not exist.");
         }
      }

      /// Create the BillVersionTable table if the database doesn't already have such a table
      public static void CreateYourself() { DB.NonQuery(createTableQuery,"BillVersionTable.CreateYourself"); }

      /// Delete the BillVersionTable table
      public static void DeleteYourself() { DB.NonQuery($"Drop Table [{table_Name}]","BillVersionTable.DeleteYourself"); }

      /// Clear the BillVersionTable table
      public static void ClearYourself() { DB.NonQuery($"Delete From [{table_Name}]","BillVersionTable.DeleteYourself"); }

      List<BillVersionRow> ParseBillVersionDat(List<string> file_rows) {
         var result = new List<BillVersionRow>();
         foreach (string file_row in file_rows) {
            List<string> columns = BillVersionRow.SplitIntoColumns(file_row);
            result.Add(new BillVersionRow(columns));
         }
         return result;
      }

      public BillVersionRow Scalar(string bill_xml) {
         var result = (from item in Table where (item.BillXml == bill_xml) select item).FirstOrDefault();
         return result;
      }
   }
}
