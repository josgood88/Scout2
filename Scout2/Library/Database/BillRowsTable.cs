using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Text;

namespace ClassLibrary.Database {
   public class BillRowsTable : BaseTables {
      private static List<BillRow> table_contents = new List<BillRow>();
      public static string table_Name = "BillRows";

      private static string createTableQuery =
         $"Create Table If Not Exists [{table_Name}] (" +
            "[BillID]        Text NOT NULL PRIMARY KEY," +
            "[MeasureType]   Text NOT NULL," +
            "[MeasureNum]    Text NOT NULL," +
            "[Lob]           Text NOT NULL," +
            "[NegativeScore] Int NULL, "  +
            "[PositiveScore] Int NULL, "  +
            "[Position]      Text NULL, " +
            "[Bill]          Text NULL, " +
            "[BillVersionID] Text NULL, " +
            "[Author]        Text NULL, " +
            "[Title]         Text NULL, " +
            "[Location]      Text NULL, " +
            "[Location2nd]   Text NULL, " +
            "[MeasureState]  Text NULL, " +
            "[CurrentHouse]  Text NULL, " +
            "[CurrentStatus] Text NULL"   +
            ")";

      /// Create the BillRows table if the database doesn't already have such a table
      public static void CreateYourself() { DB.NonQuery(createTableQuery,"BillRowsTable.CreateYourself"); }

      /// Delete the BillRows table
      public static void DeleteYourself() { DB.NonQuery($"Drop Table [{table_Name}]","BillRowsTable.DeleteYourself"); }

      /// Clear the BillRows table
      public static void ClearYourself() { DB.NonQuery($"Delete From [{table_Name}]","BillRowsTable.DeleteYourself"); }

      /// Select one or more rows into a row set.
      public static List<BillRow> Select(string query) {
         var result = new List<BillRow>();
         SQLiteConnection con = null;
         try {
            using (con = DB.Connect()) {           // Obtain SQLiteConnection
               con.Open();                         // Open the connection to the database
               using (SQLiteCommand com = new SQLiteCommand(con)) {
                  com.CommandText = query;
                  using (SQLiteDataReader reader = com.ExecuteReader()) {
                     if (reader.HasRows) {
                        while (reader.Read()) {
                           var row = new BillRow(reader);
                           table_contents.Add(new BillRow(reader));
                        }
                     }
                  }
               }
            }
         } catch (Exception ex) {
            Log.Instance.Info($"BillRowsTable.ReadYourself: {ex.Message}");
            throw;
         }
         return result;
      }

      public static void InsertCollection(List<Bill_Identifier> identifers) {
         var sql_accumulator = new StringBuilder();   // Accumulate SQL statements here, submit as single statement
         var accumulated = 0;
         foreach (var identifier in identifers) {
            if (accumulated >= SQL_INSERT_ROW_LIMIT) { 
               FlushAccumulator(sql_accumulator);
               accumulated = 0;
            }
            sql_accumulator.Append($" ({BillRow.RowToColumns(new BillRow(identifier))}),");
            accumulated++;
         }
      }

      private static bool FlushAccumulator(StringBuilder sql_accumulator) {
         try {
            string raw = sql_accumulator.ToString();
            if (raw.Length > 0) {
               string inserts = raw.Substring(0,raw.Length-1);    // Trim trailing comma
               var prefix = $"Insert Into [{table_Name}] ({ BillRow.Columns()}) Values";
               string nonQuery = prefix + inserts + ";";
               sql_accumulator.Clear();
               DB.NonQuery(nonQuery,"BillRowsTable.FlushAccumulator");
               Log.Instance.Info($"BillRowsTable.FlushAccumulator wrote {SQL_INSERT_ROW_LIMIT} rows.");
            }
         } catch (SqlException sql) {
            Log.Instance.Info($"BillRowsTable.FlushAccumulator: {sql.Message}");
            throw;
         } catch (Exception e) {
            Log.Instance.Info($"BillRowsTable.FlushAccumulator: {e.Message}");
            throw;
         }
         return true;
      }

      public static void Insert(BillRow row, StringBuilder sql_accumulator) {
         var non_query = $"Insert Into [{table_Name}] ({BillRow.Columns()}) Values ({BillRow.RowToColumns(row)});";
         DB.NonQuery(non_query,"BillRowsTable.Insert");
      }
   }
}
