using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Library;
using Library.Database;
using Scout2.Sequence;
using Scout2.Utility;

namespace Scout2 {
   /// This view shows which bills have been updated by the legislature in some way -- the bill text may have been
   /// modified or there may merely have been some process-related change -- moved to a different committee or had
   /// a co-sponsor added.  Regardless the bill reviewer needs to take a look at the bill and update the bill's
   /// report if that is needed.
   public partial class UpdatedBillsForm : Form {
      private static List<ChangedBillForDisplay> contents_DataGridView = new List<ChangedBillForDisplay>();
      public UpdatedBillsForm() { InitializeComponent(); }
      /// <summary>
      /// Update this view's DataGridView to have a grid appropriate to displaying which bills have been updated.
      /// An updated bill is whose review is now out of date because the legislature has updated the bill in some way.
      /// </summary>
      public void PrepareDataGridView() {
         DataGridViewTextBoxColumn dgv_measure = new DataGridViewTextBoxColumn();
         dgv_measure.HeaderText = "Measure";
         DataGridViewTextBoxColumn dgv_position = new DataGridViewTextBoxColumn();
         dgv_position.HeaderText = "Position";
         DataGridViewTextBoxColumn dgv_la_bill = new DataGridViewTextBoxColumn();
         dgv_la_bill.HeaderText = "Bill Last Action";
         DataGridViewTextBoxColumn dgv_la_hist = new DataGridViewTextBoxColumn();
         dgv_la_hist.HeaderText = "History Last Action";

         this.ViewBillsRequiringUpdate.Columns.Add(dgv_measure);
         this.ViewBillsRequiringUpdate.Columns.Add(dgv_position);
         this.ViewBillsRequiringUpdate.Columns.Add(dgv_la_bill);
         this.ViewBillsRequiringUpdate.Columns.Add(dgv_la_hist);
      }
      /// <summary>
      /// This view's DataGridView is filled manually from the passed collection.
      /// </summary>
      /// <param name="collection">A list of UpdateNeeded which provides the information displayed in the DataGridView.</param>
      public void AddRows(List<ChangedBillForDisplay> collection) {
         contents_DataGridView = collection;
         Display();
      }
      /// <summary>
      /// Update the DataGridView when the controlling "whether to display all bills" checkbox is toggled.
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void OnToggleCheckbox(object sender, EventArgs e) { Display(); }

      private void Display() {
         ViewBillsRequiringUpdate.Rows.Clear();
         foreach (var row in contents_DataGridView) {
            if (IsNotPositionNone(row) || DisplayAll()) DisplayBillSummary(row);
         }
         ViewBillsRequiringUpdate.Refresh();
      }

      private bool IsNotPositionNone(ChangedBillForDisplay row) { return row.Position != "None"; }
      private bool DisplayAll() { return this.chkNonNoneOnly.Checked == false; }
      private void DisplayBillSummary(ChangedBillForDisplay row) {
         this.ViewBillsRequiringUpdate.Rows.Add(row.Measure, row.Position, row.BillLastAction, row.HistoryLastAction);
      }

      private void OnCellClick(object sender, DataGridViewCellEventArgs e) {
         if (e.RowIndex >= 0) {
            DataGridViewRow row = ViewBillsRequiringUpdate.Rows[e.RowIndex];
            int i = 0;
            var measure = row.Cells[i++].Value.ToString();
            var score = row.Cells[i++].Value.ToString();
            var title = row.Cells[i++].Value.ToString();
            var author = row.Cells[i++].Value.ToString();
            var result = MessageBox.Show($"Update Report for {measure}?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes) {
               UpdateReport(measure);
            }
         } else {
            // Mouse clicked on column title, so DataGridView will be sorted on that column
         }
      }
      /// <summary>
      /// Allow the user to update the current bill report.
      /// If the position is changed, the database BillRow table is updated with the new position.
      /// </summary>
      /// <param name="measure"></param>
      private void UpdateReport(string measure) {
         string path = $"{Path.Combine(Config.Instance.HtmlFolder, BillUtils.EnsureNoLeadingZerosBill(measure))}.html";
         var process = Process.Start("notepad.exe", path);
         if (process != null) process.WaitForExit();
         else BaseController.LogAndShow($"CreateNewReports.GenerateCanonicalReport: Failed to start Notepad for {path}.");
         // Update the database position
         BillRow.UpdatePosition(BillUtils.Ensure4DigitNumber(measure), "");
         //GetPositionAndSummary(path, out List<string> summary, out List<string> position_list);
         //string first_line = position_list.FirstOrDefault();
         //string position = first_line != null ? Regex.Replace(first_line, ".*?:(.*)", "$1") : "None Specified";
         //BillRow.UpdatePosition(measure, position.Trim());
         BaseController.LogAndShow($"Update for {path} is complete.");
      }
   }
}
