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
   public partial class UnreportedBillsForm : Form {
      private static List<UnreportedBillForDisplay> contents_DataGridView = new List<UnreportedBillForDisplay>();
      public UnreportedBillsForm() { InitializeComponent(); }
      /// <summary>
      /// Update this view's DataGridView to have a grid appropriate to displaying which bills have been updated.
      /// An updated bill is whose review is now out of date because the legislature has updated the bill in some way.
      /// </summary>
      public void PrepareDataGridView() {
         DataGridViewTextBoxColumn dgv_measure = new DataGridViewTextBoxColumn();
         dgv_measure.HeaderText = "Measure";
         DataGridViewTextBoxColumn dgv_score = new DataGridViewTextBoxColumn();
         dgv_score.HeaderText = "Score";
         DataGridViewTextBoxColumn dgv_title = new DataGridViewTextBoxColumn();
         dgv_title.HeaderText = "Title";
         DataGridViewTextBoxColumn dgv_author = new DataGridViewTextBoxColumn();
         dgv_author.HeaderText = "Author";

         DataGridViewCheckBoxColumn dgv_checkbox = new DataGridViewCheckBoxColumn();
         dgv_checkbox.HeaderText = "Select";

         this.ViewUnreportedBills.Columns.Add(dgv_checkbox);
         this.ViewUnreportedBills.Columns.Add(dgv_measure);
         this.ViewUnreportedBills.Columns.Add(dgv_score);
         this.ViewUnreportedBills.Columns.Add(dgv_title);
         this.ViewUnreportedBills.Columns.Add(dgv_author);
      }
      /// <summary>
      /// This view's DataGridView is filled manually from the passed collection.
      /// </summary>
      /// <param name="collection">A list of UpdateNeeded which provides the information displayed in the DataGridView.</param>
      public void AddRows(List<UnreportedBillForDisplay> collection) {
         contents_DataGridView = collection;
         Display();
      }
      /// <summary>
      /// Display the DataGridView, which has already been loaded with its contents
      /// </summary>
      private void Display() {
         ViewUnreportedBills.Rows.Clear();
         foreach (var row in contents_DataGridView) {
            DisplayBillSummary(row);
         }
         ViewUnreportedBills.Refresh();
      }
      private void DisplayBillSummary(UnreportedBillForDisplay row) {
         this.ViewUnreportedBills.Rows.Add(false, row.Measure, row.NegativeScore, row.Title, row.Author);
      }
   }
}
