using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Scout2.Sequence;

namespace Scout2 {
   /// This view shows bills for which no position has been determined.
   public partial class NewBillsForm : Form {
      private static List<NewBillForDisplay> contents_DataGridView = new List<NewBillForDisplay>();
      public NewBillsForm() { InitializeComponent(); }
      /// <summary>
      /// Update this view's DataGridView to have a grid appropriate to displaying which bills have been updated.
      /// An updated bill is whose review is now out of date because the legislature has updated the bill in some way.
      /// </summary>
      public void PrepareDataGridView() {
         DataGridViewTextBoxColumn dgv_measure = new DataGridViewTextBoxColumn();
         dgv_measure.HeaderText = "Measure";
         DataGridViewTextBoxColumn dgv_position = new DataGridViewTextBoxColumn();
         dgv_position.HeaderText = "Score";
         DataGridViewTextBoxColumn dgv_la_bill = new DataGridViewTextBoxColumn();
         dgv_la_bill.HeaderText = "Title";

         DataGridViewCheckBoxColumn dgv_checkbox = new DataGridViewCheckBoxColumn();
         dgv_checkbox.HeaderText = "Select";

         this.ViewBillsRequiringUpdate.Columns.Add(dgv_checkbox);
         this.ViewBillsRequiringUpdate.Columns.Add(dgv_measure);
         this.ViewBillsRequiringUpdate.Columns.Add(dgv_position);
         this.ViewBillsRequiringUpdate.Columns.Add(dgv_la_bill);

         this.ViewBillsRequiringUpdate.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
      }
      /// <summary>
      /// This view's DataGridView is filled manually from the passed collection.
      /// </summary>
      /// <param name="collection">A list of UpdateNeeded which provides the information displayed in the DataGridView.</param>
      public void AddRows(List<NewBillForDisplay> collection) {
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
            DisplayBillSummary(row);
         }
         ViewBillsRequiringUpdate.Refresh();
      }

      private void DisplayBillSummary(NewBillForDisplay row) {
         this.ViewBillsRequiringUpdate.Rows.Add(false, row.Measure, row.Score, row.Title);
      }
   }
}
