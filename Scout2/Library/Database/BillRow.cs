using Library;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;

namespace Library.Database {
   public class BillRow {
      public string Bill          { get; set; } // e.g. AB10
      public string MeasureType   { get; set; } // e.g. AB
      public string MeasureNum    { get; set; } // e.g. 10
      public string Lob           { get; set; }
      public int    NegativeScore { get; set; }
      public int    PositiveScore { get; set; }
      public string Position      { get; set; } // Hand Entered, e.g. Monitor
      public string BillVersionID { get; set; } // e.g. 20190AB199INT
      public string Author        { get; set; }
      public string Title         { get; set; }
      public string Location      { get; set; } // e.g. CX08
      public string Location2nd   { get; set; } // e.g. Committee
      public string MeasureState  { get; set; } // e.g. Amended Assembly
      public string CurrentHouse  { get; set; } // e.g. Assembly
      public string CurrentStatus { get; set; } // e.g. In Committee Process

      public BillRow() {
         // Just a temporary used for debugging
      }

      public BillRow(SQLiteDataReader reader) {
         var offset = 0;
         Bill          = reader.GetString(offset++).Trim();
         MeasureType   = reader.GetString(offset++).Trim();
         MeasureNum    = reader.GetString(offset++).Trim();
         Lob           = reader.GetString(offset++).Trim();
         NegativeScore = reader.GetInt32 (offset++);
         PositiveScore = reader.GetInt32 (offset++);
         Position      = reader.GetString(offset++).Trim();
         BillVersionID = reader.GetString(offset++).Trim();
         Author        = reader.GetString(offset++).Trim();
         Title         = reader.GetString(offset++).Trim();
         Location      = reader.GetString(offset++).Trim();
         Location2nd   = reader.GetString(offset++).Trim();
         MeasureState  = reader.GetString(offset++).Trim();
         CurrentHouse  = reader.GetString(offset++).Trim();
         CurrentStatus = reader.GetString(offset++).Trim();
      }

      public BillRow(Bill_Identifier identifier) {
         MeasureType   = identifier.Type.Trim();
         MeasureNum    = identifier.Number.Trim();
         Bill          = $"{MeasureType}{MeasureNum}";
         Lob           = identifier.LobPath.Trim();
         NegativeScore = 0;
         PositiveScore = 0;
         Position      = string.Empty;
         BillVersionID = identifier.Revision;
         Author        = identifier.Author.Trim();
         Title         = identifier.Title.Trim();
         Location      = string.Empty;
         Location2nd   = string.Empty;
         MeasureState  = string.Empty;
         CurrentHouse  = string.Empty;
         CurrentStatus = string.Empty;
      }

      public BillRow(BillProfile profile) {
         MeasureType   = profile.Identifier.Type.Trim();
         MeasureNum    = profile.Identifier.Number.Trim();
         Bill          = profile.Identifier.BillID;
         Lob           = profile.Identifier.LobPath.Trim();
         NegativeScore = profile.NegScore;
         PositiveScore = profile.PosScore;
         Position      = (from item in GlobalData.BillRows where (item.Bill == Bill) select item.Position).FirstOrDefault();
         BillVersionID = profile.Identifier.Revision;
         Author        = profile.Identifier.Author.Trim();
         Title         = profile.Identifier.Title.Trim();
         Location      = string.Empty;
         Location2nd   = string.Empty;
         MeasureState  = string.Empty;
         CurrentHouse  = string.Empty;
         CurrentStatus = string.Empty;
      }

      public static string Columns() {
         string result =
            "BillID"        + $"{DB.Separator}" + "MeasureType"   + $"{DB.Separator}" +
            "MeasureNum"    + $"{DB.Separator}" + "Lob"           + $"{DB.Separator}" +
            "NegativeScore" + $"{DB.Separator}" + "PositiveScore" + $"{DB.Separator}" +
            "Position"      + $"{DB.Separator}" + "BillVersionID" + $"{DB.Separator}" +
            "Author"        + $"{DB.Separator}" + "Title"         + $"{DB.Separator}" +
            "Location"      + $"{DB.Separator}" + "Location2nd"   + $"{DB.Separator}" +
            "MeasureState"  + $"{DB.Separator}" + "CurrentHouse"  + $"{DB.Separator}" +
            "CurrentStatus";
         return result;
      }

      public static string RowToColumns(BillRow row) {
         var forward = row.Lob.Replace('\\','/').Trim();
         string result = 
            $"'{row.Bill          }'{DB.Separator}'{row.MeasureType   }'{DB.Separator}"+
            $"'{row.MeasureNum    }'{DB.Separator}'{forward           }'{DB.Separator}"+
            $"{ row.NegativeScore }{ DB.Separator}{ row.PositiveScore }{ DB.Separator}"+
            $"'{row.Position      }'{DB.Separator}'{row.BillVersionID }'{DB.Separator}"+
            $"'{row.Author        }'{DB.Separator}'{row.Title         }'{DB.Separator}"+
            $"'{row.Location      }'{DB.Separator}'{row.Location2nd   }'{DB.Separator}"+
            $"'{row.MeasureState  }'{DB.Separator}'{row.CurrentHouse  }'{DB.Separator}"+
            $"'{row.CurrentStatus }'";
         return result;
      }

      public static List<BillRow> UpdateGlobalBillRows(IEnumerable<BillProfile> input) {
         var result = GlobalData.BillRows;
         foreach (var item in input) {
            // Do we need to add a new row or update an existing one?
            var index_existing_row = result.FindIndex(x => x.Bill == item.Identifier.BillID);
            if (index_existing_row == -1) result.Add(new BillRow(item));
            else Update(result,index_existing_row);
         }
         return result;
      }

      private static void Update(List<BillRow> result, int index) {
         string name_ext = Path.GetFileName(result[index].Lob);           // BillVersionTable bill_xml is unique
         BillVersionRow bv_row = GlobalData.VersionTable.Scalar(name_ext);
         List<BillHistoryRow> history = GlobalData.HistoryTable.RowSet(bv_row.BillID);
         var location_code = history.First().TernaryLocation;
         var location_code_row = GlobalData.LocationTable.Scalar(location_code);
         
         result[index].Location = history.First().PrimaryLocation;
         result[index].Location2nd = location_code_row?.LongDescription;
         if (result[index].Location2nd == null) result[index].Location2nd = location_code;   // e.g. "Third Reading"
         result[index].MeasureState = "m state";
         result[index].CurrentHouse = history.First().PrimaryLocation;
         result[index].CurrentStatus = history.First().EndStatus;
      }

      public static List<BillRow> RowSet() {
         var result = new List<BillRow>();
         var query = "Select BillID,MeasureType,MeasureNum,Lob,NegativeScore,PositiveScore,Position,BillVersionID,Author,Title,Location,Location2nd,MeasureState,CurrentHouse,CurrentStatus from BillRows Order By BillID;";
         try {
            using (SQLiteConnection con = DB.Connect()) {      // Obtain SQLiteConnection
               con.Open();                                     // Open the connection to the database
               using (SQLiteCommand cmd = new SQLiteCommand(query,con)) {
                  using (SQLiteDataReader reader = cmd.ExecuteReader()) {
                     while (reader.Read()) {
                        result.Add(new BillRow(reader));
                     }
                  }
               }
            }
         } catch (Exception ex) {
            Log.Instance.Info($"BillRow.RowSet: {ex.Message}");
            throw;
         }
         return result;
      }

      public static BillRow Row(string bill) {
         BillRow result = null;
         var query = $"Select BillID,MeasureType,MeasureNum,Lob,NegativeScore,PositiveScore,Position,BillVersionID,Author,Title,Location,Location2nd,MeasureState,CurrentHouse,CurrentStatus from BillRows Where BillID='{bill}';";
         try {
            using (SQLiteConnection con = DB.Connect()) {   // Obtain SQLiteConnection
               con.Open();                                  // Open the connection to the database
               using (SQLiteCommand cmd = new SQLiteCommand(query,con)) {
                  using (SQLiteDataReader reader = cmd.ExecuteReader()) {
                     reader.Read();
                     result = new BillRow(reader);
                  }
               }
            }
         } catch (Exception ex) {
            Log.Instance.Info($"BillRow.Row({query}): {ex.Message}");
            throw;
         }
         return result;
      }

      public static void WriteRowset(IEnumerable<BillRow> collection) {
         DB.NonQuery("Delete from BillRows;","BillRow.WriteRowset");
         var accumulator = new StringBuilder();   // Accumulate SQL statements here, submit as single statement
         var line_count = 0;
         foreach (var item in collection) item.WriteYourself(accumulator, ref line_count);
         FlushAccumulator(accumulator);
      }

      public void WriteYourself(StringBuilder sql_accumulator,ref int line_count) {
         sql_accumulator.Append($" ('{Bill}','{MeasureType}','{MeasureNum}','{Lob}',{NegativeScore},{PositiveScore},'{Position}','{BillVersionID}','{Author}','{Title}','{Location}','{Location2nd}','{MeasureState}','{CurrentHouse}','{CurrentStatus}'),");
         var str = sql_accumulator.ToString();
         if (++line_count == DB.SQL_INSERT_ROW_LIMIT) {    // SQL Server limit is 1000 rows
            line_count = 0;
            FlushAccumulator(sql_accumulator);
         }
      }

      public static bool FlushAccumulator(StringBuilder sql_accumulator) {
         try {
            string raw = sql_accumulator.ToString();
            if (raw.Length > 0) {
               string inserts = raw.Substring(0,raw.Length-1);    // Trim trailing comma
               var cmd0 = "Insert Into BillRows (BillID,MeasureType,MeasureNum,Lob,NegativeScore,PositiveScore,Position,BillVersionID,Author,Title,Location,Location2nd,MeasureState,CurrentHouse,CurrentStatus) ";
               string nonQuery = cmd0 + $" VALUES {inserts};";
               sql_accumulator.Clear();
               DB.NonQuery(nonQuery,"BillRow:FlushAccumulator");
            }
         } catch (SqlException sql) {
            Log.Instance.Info($"BillRow:FlushAccumulator: {sql.Message}");
            throw;
         } catch (Exception e) {
            Log.Instance.Info($"BillRow:FlushAccumulator: {e.Message}");
            throw;
         }
         return true;
      }
   }
}
