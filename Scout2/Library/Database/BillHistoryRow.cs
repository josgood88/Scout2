using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Text.RegularExpressions;

namespace Library.Database {
   public class BillHistoryRow : BaseRows {
      public string BillID            { get; private set; }
      public string BillHistoryID     { get; private set; }
      public string ActionDate        { get; private set; }
      public string Action            { get; private set; }
      public string Trans_UID         { get; private set; }
      public string TransUpdateDate   { get; private set; }
      public string ActionSequence    { get; private set; }
      public string ActionCode        { get; private set; }
      public string ActionStatus      { get; private set; }
      public string PrimaryLocation   { get; private set; }
      public string SecondaryLocation { get; private set; }
      public string TernaryLocation   { get; private set; }
      public string EndStatus         { get; private set; }

      public BillHistoryRow(List<string> row) {
         using (var etr = row.GetEnumerator()) {
            try {
               if (etr.MoveNext()) BillID            = MQ(etr.Current);
               if (etr.MoveNext()) BillHistoryID     = MQ(etr.Current);
               if (etr.MoveNext()) ActionDate        = MQ(etr.Current);
               if (etr.MoveNext()) Action            = MQ(etr.Current);
               if (etr.MoveNext()) Trans_UID         = MQ(etr.Current);
               if (etr.MoveNext()) TransUpdateDate   = MQ(etr.Current);
               if (etr.MoveNext()) ActionSequence    = MQ(etr.Current);
               if (etr.MoveNext()) ActionCode        = MQ(etr.Current);
               if (etr.MoveNext()) ActionStatus      = MQ(etr.Current);
               if (etr.MoveNext()) PrimaryLocation   = MQ(etr.Current);
               if (etr.MoveNext()) SecondaryLocation = MQ(etr.Current);
               if (etr.MoveNext()) TernaryLocation   = MQ(etr.Current);
               if (etr.MoveNext()) EndStatus         = MQ(etr.Current);
            } catch (Exception e) {
               Log.Instance.Info($"BillHistoryRow(List<String> row) exception: {e.Message}, {etr.Current}");
               throw;
            }
         }
      }

      public static void WriteRowset(IEnumerable<BillHistoryRow> collection) {
         DB.NonQuery("Delete from BillHistoryTable;","BillHistoryRow.WriteRowset");
         var accumulator = new StringBuilder();   // Accumulate SQL statements here, submit as single statement
         var line_count = 0;
         foreach (var item in collection) item.WriteYourself(accumulator,ref line_count);
         FlushAccumulator(accumulator);
      }

      public void WriteYourself(StringBuilder sql_accumulator,ref int line_count) {
         char[] c = Action.ToCharArray();
         TransUpdateDate = TransUpdateDate;
         Action = Regex.Replace(Action,"'","''"); // SQL embedded apostrophe is two consecutive apostrophes
         string xx = $" ('{BillID}','{BillHistoryID}','{ActionDate}','{Action}','{Trans_UID}','{TransUpdateDate}', '{ActionSequence}', '{ActionCode}', '{ActionStatus}','{PrimaryLocation}','{SecondaryLocation}','{TernaryLocation}','{EndStatus}'),";
         sql_accumulator.Append(xx);
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
               var cmd0 = "Insert Into BillHistoryTable (BillID,BillHistoryID,ActionDate,Action,Trans_UID,TransUpdateDate,ActionSequence,ActionCode,ActionStatus,PrimaryLocation,SecondaryLocation,TernaryLocation,EndStatus)";
               string nonQuery = cmd0 + $" VALUES {inserts};";
               sql_accumulator.Clear();
               DB.NonQuery(nonQuery,"BillHistoryRow:FlushAccumulator");
            }
         } catch (SqlException sql) {
            Log.Instance.Info($"BillHistoryRow:FlushAccumulator: {sql.Message}");
            throw;
         } catch (Exception e) {
            Log.Instance.Info($"BillHistoryRow:FlushAccumulator: {e.Message}");
            throw;
         }
         return true;
      }
   }
}
