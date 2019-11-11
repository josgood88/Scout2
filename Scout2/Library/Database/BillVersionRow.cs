using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Library.Database {
   public class BillVersionRow : BaseRows {
      public string BillVersionID         { get; private set; }
      public string BillID                { get; private set; }
      public string VersionNum            { get; private set; }
      public string BillVersionActionDate { get; private set; }
      public string BillVersionAction     { get; private set; }
      public string RequestNum            { get; private set; }
      public string Subject               { get; private set; }
      public string VoteRequired          { get; private set; }
      public string Appropriation         { get; private set; }
      public string FiscalCommittee       { get; private set; }
      public string LocalProgram          { get; private set; }
      public string SubstantiveChanges    { get; private set; }
      public string Urgency               { get; private set; }
      public string TaxLevy               { get; private set; }
      public string BillXml               { get; private set; }
      public string ActiveFlag            { get; private set; }
      public string TransUID              { get; private set; }
      public string TransUpdate           { get; private set; }

      public BillVersionRow(List<string> row) {
         using (var etr = row.GetEnumerator()) {
            try {
               if (etr.MoveNext()) BillVersionID         = MQ(etr.Current);
               if (etr.MoveNext()) BillID                = MQ(etr.Current);
               if (etr.MoveNext()) VersionNum            = MQ(etr.Current);
               if (etr.MoveNext()) BillVersionActionDate = MQ(etr.Current);
               if (etr.MoveNext()) BillVersionAction     = MQ(etr.Current);
               if (etr.MoveNext()) RequestNum            = MQ(etr.Current);
               if (etr.MoveNext()) Subject               = MQ(etr.Current);
               if (etr.MoveNext()) VoteRequired          = MQ(etr.Current);
               if (etr.MoveNext()) Appropriation         = MQ(etr.Current);
               if (etr.MoveNext()) FiscalCommittee       = MQ(etr.Current);
               if (etr.MoveNext()) LocalProgram          = MQ(etr.Current);
               if (etr.MoveNext()) SubstantiveChanges    = MQ(etr.Current);
               if (etr.MoveNext()) Urgency               = MQ(etr.Current);
               if (etr.MoveNext()) TaxLevy               = MQ(etr.Current);
               if (etr.MoveNext()) BillXml               = MQ(etr.Current);
               if (etr.MoveNext()) ActiveFlag            = MQ(etr.Current);
               if (etr.MoveNext()) TransUID              = MQ(etr.Current);
               if (etr.MoveNext()) TransUpdate           = MQ(etr.Current);
            } catch (Exception e) {
               Log.Instance.Info($"BillVersionRow(List<String> row) exception: {e.Message}, {etr.Current}");
               throw;
            }
         }
      }

      public static void WriteRowset(IEnumerable<BillVersionRow> collection) {
         DB.NonQuery("Delete from BillVersionTable;","BillVersionRow.WriteRowset");
         var accumulator = new StringBuilder();   // Accumulate SQL statements here, submit as single statement
         var line_count = 0;
         foreach (var item in collection) item.WriteYourself(accumulator,ref line_count);
         FlushAccumulator(accumulator);
      }

      public void WriteYourself(StringBuilder sql_accumulator,ref int line_count) {
         sql_accumulator.Append($" ('{BillVersionID}','{BillID}','{VersionNum}','{BillVersionActionDate}','{BillVersionAction}','{RequestNum}','{Subject}','{VoteRequired}','{Appropriation}','{FiscalCommittee}','{LocalProgram}','{SubstantiveChanges}','{Urgency}','{TaxLevy}','{BillXml}','{ActiveFlag}','{TransUID}','{TransUpdate}'),");
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
               var cmd0 = "Insert Into BillVersionTable (BillVersionID, BillID, VersionNum, BillVersionActionDate, BillVersionAction, RequestNum, Subject, VoteRequired, Appropriation, FiscalCommittee, LocalProgram, SubstantiveChanges, Urgency, TaxLevy, BillXml, ActiveFlag, TransUID, TransUpdate) ";
               string nonQuery = cmd0 + $" VALUES {inserts};";
               sql_accumulator.Clear();
               DB.NonQuery(nonQuery,"BillVersionRow:FlushAccumulator");
            }
         } catch (SqlException sql) {
            Log.Instance.Info($"BillVersionRow:FlushAccumulator: {sql.Message}");
            throw;
         } catch (Exception e) {
            Log.Instance.Info($"BillVersionRow:FlushAccumulator: {e.Message}");
            throw;
         }
         return true;
      }
   }
}
