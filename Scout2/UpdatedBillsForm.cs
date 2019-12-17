using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Scout2.Sequence;

namespace Scout2 {
   /// This view shows which bills have been updated by the legislature in some way -- the bill text may have been
   /// modified or there may merely have been some process-related change -- moved to a different committee or had
   /// a co-sponsor added.  Regardless the bill reviewer needs to take a look at the bill and update the bill's
   /// report if that is needed.
   public partial class UpdatedBillsForm : Form {
      private static List<UpdateNeeded> contents_DataGridView = new List<UpdateNeeded>();
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

         DataGridViewCheckBoxColumn dgv_checkbox = new DataGridViewCheckBoxColumn();
         dgv_checkbox.HeaderText = "Select";

         this.ViewBillsRequiringUpdate.Columns.Add(dgv_checkbox);
         this.ViewBillsRequiringUpdate.Columns.Add(dgv_measure);
         this.ViewBillsRequiringUpdate.Columns.Add(dgv_position);
         this.ViewBillsRequiringUpdate.Columns.Add(dgv_la_bill);
         this.ViewBillsRequiringUpdate.Columns.Add(dgv_la_hist);
      }
      /// <summary>
      /// This view's DataGridView is filled manually from the passed collection.
      /// </summary>
      /// <param name="collection">A list of UpdateNeeded which provides the information displayed in the DataGridView.</param>
      public void AddRows(List<UpdateNeeded> collection) {
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

      private bool IsNotPositionNone(UpdateNeeded row) { return row.Position != "None"; }
      private bool DisplayAll() { return this.chkNonNoneOnly.Checked == false; }
      private void DisplayBillSummary(UpdateNeeded row) {
         this.ViewBillsRequiringUpdate.Rows.Add(false, row.Measure, row.Position, row.BillLastAction, row.HistoryLastAction);
      }
   }
}
