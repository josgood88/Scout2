using System.Collections.Generic;
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
         var dgv_measure = new DataGridViewTextBoxColumn();
         var dgv_score = new DataGridViewTextBoxColumn();
         var dgv_title = new DataGridViewTextBoxColumn();
         var dgv_author = new DataGridViewTextBoxColumn();

         dgv_measure.HeaderText = "Measure";
         dgv_score.HeaderText = "Score";
         dgv_title.HeaderText = "Title";
         dgv_author.HeaderText = "Author";

         ViewUnreportedBills.Columns.Add(dgv_measure);
         ViewUnreportedBills.Columns.Add(dgv_score);
         ViewUnreportedBills.Columns.Add(dgv_title);
         ViewUnreportedBills.Columns.Add(dgv_author);
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
         this.ViewUnreportedBills.Rows.Add(row.Measure, row.NegativeScore, row.Title, row.Author);
      } 
      
      private void OnCellClick(object sender, DataGridViewCellEventArgs e) {
         if (e.RowIndex >= 0) {
            DataGridViewRow row = ViewUnreportedBills.Rows[e.RowIndex];
            int i = 0;
            var measure = row.Cells[i++].Value.ToString();
            var score = row.Cells[i++].Value.ToString();
            var title = row.Cells[i++].Value.ToString();
            var author = row.Cells[i++].Value.ToString();
            var result = MessageBox.Show($"Create Report for {measure}?","",MessageBoxButtons.YesNo,MessageBoxIcon.Question);
            if (result == DialogResult.Yes) {
               CreateNewReports.GenerateCanonicalReport(measure);
            }
         } else {
            // Mouse clicked on column title, so DataGridView will be sorted on that column
         }
      }
   }
}
