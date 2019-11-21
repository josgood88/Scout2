using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using Library;
using Scout2.Controllers;

namespace Scout2 {
   public partial class Form1 : Form {

      public Form1() {
         InitializeComponent();
         this.TopMost = true;
         Config.Instance.ReadYourself();  // Start of configuration data lifetime
      }

      private void Form1_FormClosing(object sender, FormClosingEventArgs e) {
         Config.Instance.WriteYourself(); // End of configuration data lifetime
      }

      /// <summary>
      /// Download the most recent leginfo file, which is a zipped file.
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private async void btnLegSite_Click(object sender, EventArgs e) {
         try {
            await new LegSiteController().Run(this);   // Download the latest leginfo zip file
         } catch (Exception ex) {
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
            TopMost = false;                 // If download not performed then form is still topmost
            new ZipController().Run(this);   // Extract the contents of the downloaded zip file.
         } catch (Exception ex) {
            MessageBox.Show(ex.Message, "Unable to extract the contents of the downloaded zip file.");
         }
      }

      private void btnImport_Click(object sender, EventArgs e) {
         try {
            TopMost = false;                 // If download not performed then form is still topmost
            new ImportController().Run(this);
         } catch (Exception ex) {
            MessageBox.Show(ex.Message, "Unable to import the bill files.");
         }
      }

      private void btnReport_Click(object sender, EventArgs e) {
         TopMost = false;                 // If download not performed then form is still topmost
         Log.Instance.Info("Start At Report clicked");
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
