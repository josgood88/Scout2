using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using Library;
using Scout2.Sequence;

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
      private void btnLegSite_Click(object sender, EventArgs e) {
         try {
            SequenceControl.ImportFromLegSite(this);
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
            SequenceControl.ExtractFromZip(this);  // Extract the contents of the downloaded zip file.
         } catch (Exception ex) {
            MessageBox.Show(ex.Message, "Unable to extract the contents of the downloaded zip file.");
         }
      }
      /// <summary>
      /// Update the database with the latest data on the bill's text, status, committee location, etc.
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void btnImport_Click(object sender, EventArgs e) {
         try {
            SequenceControl.ImportToDB(this);
         } catch (Exception ex) {
            MessageBox.Show(ex.Message, "Unable to import the bill files.");
         }
      }
      /// <summary>
      /// Regenerate the individual bill reports.  In particular, update the bill's history
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void btnRegenerate_Click(object sender, EventArgs e) {
         try {
            SequenceControl.RegenBillReports(this);
         } catch (Exception ex) {
            MessageBox.Show(ex.Message, "Unable to regenerate at least one bill report.");
         }
      }
      /// <summary>
      /// Show those bills that have changed in some way from the last time the bill reports were regenerated.
      /// The most import change is a change in the bill's text.  Committee location is also important.
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void btnUpdateAndNew_Click(object sender, EventArgs e) {
         try {
            SequenceControl.UpdateBillReports(this);
         } catch (Exception ex) {
            MessageBox.Show(ex.Message, "Unable to update bill reports.");
         }
      }

      private void btnCreateReports_Click(object sender, EventArgs e) {
         try {
            SequenceControl.CreateBillReports(this);
         } catch (Exception ex) {
            MessageBox.Show(ex.Message, "Unable to create bill report.");
         }
      }
      /// <summary>
      /// Generate the weekly report.
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void btnReport_Click(object sender, EventArgs e) {
         try {
            SequenceControl.WeeklyReport(this);
         } catch (Exception ex) {
            MessageBox.Show(ex.Message, "Unable to generate the weekly report.");
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
      /// <summary>
      /// Regnerate needs a way to know whether the "Regenerate All" checkbox is checked.
      /// </summary>
      /// <returns></returns>
      public bool IsRegenerateAll() { return chkRegenerateAll.Checked;  }

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
