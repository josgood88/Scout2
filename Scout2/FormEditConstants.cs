using System;
using System.IO;
using System.Windows.Forms;
using Library;

namespace Scout2 {
   public partial class FormEditConstants : Form {
      public FormEditConstants() {
         InitializeComponent();
         txtBillsFolder.Text     = Config.Instance.BillsFolder;
         txtDatabaseFolder.Text  = Config.Instance.DatabaseFolder;
         txtDownloadsFolder.Text = Config.Instance.DownloadsFolder;
         txtHtmlFolder.Text      = Config.Instance.HtmlFolder;
         txtNegativeFile.Text    = Config.Instance.NegativeFile;
         txtPositiveFile.Text    = Config.Instance.PositiveFile;
         txtScoutFile.Text       = Config.Instance.ScoutFile;
         txtLegislatureSite.Text = Config.Instance.LegSite;
      }

      private void btnBillsFolder_Click(object sender, EventArgs e) {
         UpdateFolder(txtBillsFolder);
      }

      private void btnDataBaseFolder_Click(object sender, EventArgs e) {
         UpdateFolder(txtDatabaseFolder);
      }

      private void btnDownloadsFolder_Click(object sender, EventArgs e) {
         UpdateFolder(txtDownloadsFolder);
      }

      private void btnHtmlFolder_Click(object sender, EventArgs e) {
         UpdateFolder(txtHtmlFolder);
      }

      private void btnNegativeFile_Click(object sender, EventArgs e) {
         UpdateFile(txtNegativeFile);
      }

      private void btnPositiveFile_Click(object sender, EventArgs e) {
         UpdateFile(txtPositiveFile);
      }

      private void btnScoutFile_Click(object sender, EventArgs e) {
         UpdateFile(txtScoutFile);
      }

      private void ClearIfFolderNotExists(TextBox field) {
         if (field.Text.Length > 0) {
            if (!Directory.Exists(field.Text)) {
               field.Text = string.Empty;
               field.Update();
            }
         }
      }

      private void ClearIfFileNotExists(TextBox field) {
         if (field.Text.Length > 0) {
            if (!File.Exists(field.Text)) {
               field.Text = string.Empty;
               field.Update();
            }
         }
      }

      private void InitializeFolderDialog(TextBox field) {
         if (field.Text.Length > 0) {
            folderBrowserDialog1.SelectedPath = field.Text;
         }
      }

      private void InitializeFileDialog(TextBox field) {
         if (field.Text.Length > 0) {
            openFileDialog1.FileName = field.Text;
         }
      }

      private void UpdateFolder(TextBox field) {
         ClearIfFolderNotExists(field);
         InitializeFolderDialog(field);
         var result = folderBrowserDialog1.ShowDialog();
         if (result == DialogResult.OK) {
            field.Text = folderBrowserDialog1.SelectedPath;
            field.Update();
         }
      }

      private void UpdateFile(TextBox field) {
         ClearIfFileNotExists(field);
         InitializeFileDialog(field);
         var result = openFileDialog1.ShowDialog();
         if (result == DialogResult.OK) {
            field.Text = openFileDialog1.FileName;
            field.Update();
         }
      }
      /// <summary>
      /// Update contents of the highest-priority-bills file
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void btnHighPriorityFile_Click(object sender, EventArgs e) {
         MessageBox.Show("Highest Priority file update");
      }
      /// <summary>
      /// Update the configuration file when the form closes
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void FormEditConstants_FormClosing(object sender, FormClosingEventArgs e) {
         Config.Instance.WriteYourself(); // For now, always update.  Later perhaps can be a bit smarter.
      }
   }
}
