using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Scout2.Sequence;

namespace Library {
   /// <summary>
   /// A single row of bill data as presented in the legislative site's BILL_TBL.dat
   /// </summary>
   public struct BillTableRow {
      public string Bill_id                { get; set; }
      public string Session_year           { get; set; }
      public string Session_num            { get; set; }
      public string Measure_type           { get; set; }
      public string Measure_num            { get; set; }
      public string Measure_state          { get; set; }
      public string Chapter_year           { get; set; }
      public string Chapter_type           { get; set; }
      public string Chapter_session_num    { get; set; }
      public string Chapter_num            { get; set; }
      public string Latest_bill_version_id { get; set; }
      public string Active_flg             { get; set; }
      public string Trans_uid              { get; set; }
      public string Trans_update           { get; set; }
      public string Current_location       { get; set; }
      public string Current_secondary_loc  { get; set; }
      public string Current_house          { get; set; }
      public string Current_status         { get; set; }
      public string Days_31st_in_print     { get; set; }

      enum field_offsets {
         bill_id, session_year, session_num, measure_type, measure_num, measure_state,
         chapter_year, chapter_type, chapter_session_num, chapter_num, latest_bill_version_id,
         active_flg, trans_uid, trans_update, current_location, current_secondary_loc,
         current_house, current_status, days_31st_in_print, count
      };

      public BillTableRow(string bill_tbl_row) {
         var split_fields = Regex.Split(bill_tbl_row, @"\t");
         if (split_fields.Length != (int)field_offsets.count) {
            throw new ApplicationException($"Bill_Tbl_Wrapper.ctor: Argument expected to have {field_offsets.count} fields, but has {split_fields.Length} instead.");
         } else {
            var fields = new List<string>();
            char apostrophe = (char)0x60;
            foreach (var item in split_fields) fields.Add(item.Trim(new char[] { apostrophe }));
            Bill_id                = fields[(int)field_offsets.bill_id];
            Session_year           = fields[(int)field_offsets.session_year];
            Session_num            = fields[(int)field_offsets.session_num];
            Measure_type           = fields[(int)field_offsets.measure_type];
            Measure_num            = fields[(int)field_offsets.measure_num];
            Measure_state          = fields[(int)field_offsets.measure_state];
            Chapter_year           = fields[(int)field_offsets.chapter_year];
            Chapter_type           = fields[(int)field_offsets.chapter_type];
            Chapter_session_num    = fields[(int)field_offsets.chapter_session_num];
            Chapter_num            = fields[(int)field_offsets.chapter_num];
            Latest_bill_version_id = fields[(int)field_offsets.latest_bill_version_id];
            Active_flg             = fields[(int)field_offsets.active_flg];
            Trans_uid              = fields[(int)field_offsets.trans_uid];
            Trans_update           = fields[(int)field_offsets.trans_update];
            Current_location       = fields[(int)field_offsets.current_location];
            Current_secondary_loc  = fields[(int)field_offsets.current_secondary_loc];
            Current_house          = fields[(int)field_offsets.current_house];
            Current_status         = fields[(int)field_offsets.current_status];
            Days_31st_in_print     = fields[(int)field_offsets.days_31st_in_print];
         }
      }
      /// <summary>
      /// Scout only cares about actual bills.  Resolutions and other trivia are of no interest.
      /// </summary>
      /// <param name="bill_tbl_row"></param>
      /// <returns></returns>
      public static bool IsABill(string bill_tbl_row) {
         var fields = Regex.Split(bill_tbl_row, @"\t");
         if (fields.Length != (int)field_offsets.count) {
            throw new ApplicationException($"Bill_Tbl_Wrapper.IsABill: Argument expected to have {field_offsets.count} fields, but has {fields.Length} instead.");
         } else {
            var type = fields[(int)field_offsets.measure_type];
            var abc = type == "SB";
            return type == "SB";
         }
      }
   }
   /// <summary>
   /// Provide a wrapper for BILL_TBL, which is the leg site's table of bills.
   /// More than anything else, this class encapsulates the attributes of BILL_TBL into a single location.
   /// </summary>
   public class Bill_Tbl_Wrapper : IEnumerable<BillTableRow>  {
      private List<BillTableRow> contents = new List<BillTableRow>();
      /// <summary>
      /// Read the contents of BILL_TBL.dat.  Parses those lines into a List of BillTableRow
      /// </summary>
      /// <returns>true if BILL_TBL read successfully, false otherwise</returns>
      public bool ReadYourself() {
         bool result = false;
         try {
            string path = $"{Path.Combine(Config.Instance.BillsFolder, "BILL_TBL.dat")}";
            if (File.Exists(path)) {
               var file_contents = File.ReadLines(path);
               foreach (var string_row in file_contents) {
                  var bill_table_row = new BillTableRow(string_row);
                  var bill_type = bill_table_row.Measure_type;
                  if (bill_type == "AB" || bill_type == "SB") contents.Add(bill_table_row);
               }
               result = true;
            }
         } catch (Exception ex) {
            BaseController.LogAndShow($"Unable to import BILL_TBL.dat - {ex.Message}");
         }
         return result;
      }
      /// <summary>
      /// Add a BillTableRow to this collection
      /// </summary>
      /// <param name="row"></param>
      public void Add(BillTableRow row) { contents.Add(row);  }
      /// <summary>
      /// IEnumerable of T requires an implementation of GetEnumerator().
      /// </summary>
      public IEnumerator<BillTableRow> GetEnumerator() {
         foreach (BillTableRow row in contents) {
            yield return row;
         }
      }
      IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
   }
}