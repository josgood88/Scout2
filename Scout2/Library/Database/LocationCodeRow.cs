using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Text.RegularExpressions;

namespace Library.Database {
   public class LocationCodeRow : BaseRows {
      public string SessionYear         { get; private set; }
      public string LocationCode        { get; private set; }
      public string LocationType        { get; private set; }
      public string ConsentCalendarCode { get; private set; }
      public string Description         { get; private set; }
      public string LongDescription     { get; private set; }
      public string ActiveFlg           { get; private set; }
      public string TransUID            { get; private set; }
      public string TransUpdate         { get; private set; }
      public string InactiveFileFlg     { get; private set; }

      public LocationCodeRow(List<string> row) {
         using (var etr = row.GetEnumerator()) {
            try {
               if (etr.MoveNext()) SessionYear         = MQ(etr.Current);
               if (etr.MoveNext()) LocationCode        = MQ(etr.Current);
               if (etr.MoveNext()) LocationType        = MQ(etr.Current);
               if (etr.MoveNext()) ConsentCalendarCode = MQ(etr.Current);
               if (etr.MoveNext()) Description         = MQ(etr.Current);
               if (etr.MoveNext()) LongDescription     = MQ(etr.Current);
               if (etr.MoveNext()) ActiveFlg           = MQ(etr.Current);
               if (etr.MoveNext()) TransUID            = MQ(etr.Current);
               if (etr.MoveNext()) TransUpdate         = MQ(etr.Current);
               if (etr.MoveNext()) InactiveFileFlg     = MQ(etr.Current);
            } catch (Exception e) {
               Log.Instance.Info($"LocationCodeRow(List<String> row) exception: {e.Message}, {etr.Current}");
               throw;
            }
         }
      }

      public static void WriteRowset(IEnumerable<LocationCodeRow> collection) {
         DB.NonQuery("Delete from LocationCodeTable;","LocationCodeRow.WriteRowset");
         var accumulator = new StringBuilder();   // Accumulate SQL statements here, submit as single statement
         var line_count = 0;
         foreach (var item in collection) item.WriteYourself(accumulator,ref line_count);
         FlushAccumulator(accumulator);
      }

      public void WriteYourself(StringBuilder sql_accumulator,ref int line_count) {
         //l_accumulator.Append($" ('{SessionYear}','{LocationCode}','{LocationType}','{ConsentCalendarCode}','{Description}','{LongDescription}','{ActiveFlg}','{TransUID}','{TransUpdate}','{InactiveFileFlg}'),");
         // SQL embedded apostrophe is two consecutive apostrophes
         Description = Regex.Replace(Description,"'","''");
         LongDescription = Regex.Replace(LongDescription,"'","''");
         sql_accumulator.Append($" ('{SessionYear}','{LocationCode}','{LocationType}','{ConsentCalendarCode}','{Description}','{LongDescription}',' ActiveFlg ',' TransUID ',' TransUpdate ',' InactiveFileFlg '),");
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
               var cmd0 = "Insert Into LocationCodeTable (SessionYear,LocationCode,LocationType,ConsentCalendarCode,Description,LongDescription,ActiveFlg,TransUID,TransUpdate,InactiveFileFlg)";
               string nonQuery = cmd0 + $" VALUES {inserts};";
               sql_accumulator.Clear();
               DB.NonQuery(nonQuery,"LocationCodeRow:FlushAccumulator");
            }
         } catch (SqlException sql) {
            Log.Instance.Info($"LocationCodeRow:FlushAccumulator: {sql.Message}");
            throw;
         } catch (Exception e) {
            Log.Instance.Info($"LocationCodeRow:FlushAccumulator: {e.Message}");
            throw;
         }
         return true;
      }
   }
}
