using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using Library;
using Scout2.Controllers;

namespace Scout2 {
   public partial class Form1 : Form {

      public Form1() {
         InitializeComponent();
      }

      /// <summary>
      /// Start downloading the most recent leginfo file, which is a zipped file.
      /// Downloading proceeds asychronously -- control is returned to the UI while downloading proceeds.
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private async void btnLegSite_Click(object sender, EventArgs e) {
         try {
            await new LegSiteController().Run(this);   // Returns once the download is started
         } catch (Exception ex) {
            MessageBox.Show(ex.Message, "Unable to start downloading the most recent leginfo zip file.");
         }
      }

      private void btnZipFile_Click(object sender, EventArgs e) {
         Log.Instance.Info("Start At Zip File clicked");
      }

      private void btnImport_Click(object sender, EventArgs e) {
         Log.Instance.Info("Start At Import clicked");
      }

      private void btnReport_Click(object sender, EventArgs e) {
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
