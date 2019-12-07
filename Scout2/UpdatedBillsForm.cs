using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Scout2.Controllers;

namespace Scout2 {
   public partial class UpdatedBillsForm : Form {
      public UpdatedBillsForm() {
         InitializeComponent();
      }

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

      public void AddRows(List<BillUpdates.UpdateNeeded> c) {
         foreach (var row in c) {
            this.ViewBillsRequiringUpdate.Rows.Add(false, row.Measure, row.Position, row.BillLastAction, row.HistoryLastAction);
         }
      }
   }
}
