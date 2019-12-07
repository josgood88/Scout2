using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using Library;
using Scout2.Controllers;

namespace Scout2 {
   public partial class Form1 : Form {

      public Form1() {
         InitializeComponent();
         Config.Instance.ReadYourself();  // Start of configuration data lifetime
      }

      private void Form1_FormClosing(object sender, FormClosingEventArgs e) {
         // End of configuration data lifetime
      }

      /// <summary>
      /// Download the most recent leginfo file, which is a zipped file.
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private async void btnLegSite_Click(object sender, EventArgs e) {
         try {
            this.TopMost = true;                      // Don't let Selemium hide this program's form
            await new LegSiteController().Run(this);  // Download the latest leginfo zip file
            this.TopMost = false;
            btnZipFile_Click(sender, e);              // Automatically go on to extracting zip file contents
         } catch (Exception ex) {
            this.TopMost = false;
            MessageBox.Show(ex.Message, "Unable to start downloading the most recent leginfo zip file.");
         }
      }
      /// <summary>
      /// Extract the contents of the downloaded zip file.
      /// Analysis files are not extracted because Scout does not use them.
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void btnZipFile_Click(object sender, EventArgs e) {
         try {
            new ZipController().Run(this);   // Extract the contents of the downloaded zip file.
            btnImport_Click(sender, e);      // Automatically go on to importing the bill files
         } catch (Exception ex) {
            MessageBox.Show(ex.Message, "Unable to extract the contents of the downloaded zip file.");
         }
      }

      private void btnImport_Click(object sender, EventArgs e) {
         try {
            new ImportController().Run(this);
            btnShowChanges_Click(sender, e);      // Automatically show which bills have changed
         } catch (Exception ex) {
            MessageBox.Show(ex.Message, "Unable to import the bill files.");
         }
      }

      private void btnShowChanges_Click(object sender, EventArgs e) {
         try {
            var form = new UpdatedBillsForm();
            new BillUpdates().Run(this, form);
         } catch (Exception ex) {
            MessageBox.Show(ex.Message, "Unable to show changes.");
         }
      }

      private void btnReport_Click(object sender, EventArgs e) {
         try {
            new ReportController().Run(this);
         } catch (Exception ex) {
            MessageBox.Show(ex.Message, "Unable to generate the report.");
         }
      }

      private async void button1_Click(object sender, EventArgs e) {
         Log.Instance.Info("Searching for two words near each other");
         txt1stWord.Text = "[Cc]hild"; txt2ndWord.Text = "[Aa]buse";
         txtMinDistance.Text = "0"; txtMaxDistance.Text = "4";
         bgw_TwoWordsNear.DoWork += bgw_SearchNearby;
         bgw_TwoWordsNear.ProgressChanged += bgw_TwoWordsNear_ProgressChanged;
         bgw_TwoWordsNear.RunWorkerCompleted += bgw_TwoWordsNear_RunWorkerCompleted;
         await Task.Run(() => bgw_TwoWordsNear.RunWorkerAsync(
            new bgwSearchArguments(txt1stWord.Text, txt2ndWord.Text, txtMinDistance.Text, txtMaxDistance.Text)));
      }

      private void Report(Exception ex) {
         Log.Instance.Info(ex.Message);
         MessageBox.Show(ex.Message, "Terminated: Exception");
      }

      /// <summary>
      /// LegSiteController needs a way to update the progress bar.
      /// </summary>
      /// <param name="value"></param>
      public void UpdateProgressLegSite(int value) { this.progressLegSite.Value = value; }

      private void helpToolStripMenuItem_Click(object sender, EventArgs e) {
         MessageBox.Show("Clicked", "Define Constants");
      }

      private void helpToolStripMenuItem1_Click(object sender, EventArgs e) {
         MessageBox.Show("Clicked", "Help");
      }

      private void exitToolStripMenuItem_Click(object sender, EventArgs e) {
         Application.Exit();
      }

      private void updateFoldersFileToolStripMenuItem_Click(object sender, EventArgs e) {
         var form = new FormEditConstants();
         form.ShowDialog();
      }
   }
}
