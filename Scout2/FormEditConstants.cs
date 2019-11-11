using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Scout2 {
   public partial class FormEditConstants : Form {
      public FormEditConstants() {
         InitializeComponent();
      }

      private void btnBillsFolder_Click(object sender, EventArgs e) {
         MessageBox.Show("Clicked", "Bills Folder");
      }

      private void btnDataBaseFolder_Click(object sender, EventArgs e) {
         MessageBox.Show("Clicked", "Database Folder");
      }

      private void btnHtmlFolder_Click(object sender, EventArgs e) {
         MessageBox.Show("Clicked", "Html Folder");
      }

      private void btnNegativeFile_Click(object sender, EventArgs e) {
         MessageBox.Show("Clicked", "Negative File");
      }

      private void btnPositiveFile_Click(object sender, EventArgs e) {
         MessageBox.Show("Clicked", "Positive File");
      }

      private void btnScoutFile_Click(object sender, EventArgs e) {
         MessageBox.Show("Clicked", "Scout File");
      }
   }
}
