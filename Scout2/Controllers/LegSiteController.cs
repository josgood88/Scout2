﻿using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Library;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Scout2.Controllers {
   public partial class LegSiteController : BaseController {
      private IWebDriver Driver { get; set; }

      public async Task<int> Run(Form1 form1) {
         LogThis($"Connecting to {Config.Instance.LegSite}.");
         try {
            InitializeChrome();
            FindLastModified().Click();         // Sort zip files in ascending date order
            FindLastModified().Click();         // Sort zip files in descending date order (stale fields, hence call again)
            var rows = ViewHrefs().ToList();    // Collect hrefs of the available zip files
            using (var etr = rows.GetEnumerator()) {
               bool found_zip = false;
               while (!found_zip) {
                  if (!etr.MoveNext()) {
                     var message = "LegsiteController.Index failed to find any pubinfo zip files.";
                     LogThis(message);
                     throw new ApplicationException(message);
                  } else if (etr.Current != null) {
                     if (etr.Current.Text.Contains("pubinfo_daily")) {
                        found_zip = true;
                        Driver.Manage().Window.Minimize();  // Hide the chrome window, making the application visable again
                        var interval = await DownloadLegSiteFileAsync(etr.Current.Text, form1);
                        var message = $"Download complete,  elapsed time = {interval}.";
                        LogThis(message);
                        MessageBox.Show(message);
                     }
                  }
               }
            }
         } catch (Exception ex) {
            var message = $"LegSiteController.Run: {ex.Message}";
            LogThis(message);
            MessageBox.Show(message);
         } finally {
            CloseChrome();
         }
         return 0;
      }

      private void InitializeChrome() {
         TimeSpan page_load_timeout = new TimeSpan(0, 0, 60);
         Driver = new ChromeDriver();
         Driver.Manage().Timeouts().PageLoad = page_load_timeout;
         Driver.Manage().Window.Maximize();
         Driver.Navigate().GoToUrl(Config.Instance.LegSite);
      }

      private void CloseChrome() {
         Driver.Close();
         Driver.Quit();
      }

      private IWebElement FindLastModified() {
         var last_modified = ItemsByText("Last modified");
         if (last_modified.Count != 1) {
            var message = $"LegsiteController.FindLastModified found {last_modified.Count} instances of \"Last Modified\" field.";
            LogThis(message);
            throw new Exception(message);
         }
         return last_modified.First();
      }
      private ReadOnlyCollection<IWebElement> ItemsByText(string text) {
         var descriptionText = $"//*[contains(text(), '{text}')]";
         return Driver.FindElements(By.XPath(descriptionText));
      }
      // Get all hrefs in current View
      private ReadOnlyCollection<IWebElement> ViewHrefs() {
         var result = Driver.FindElements(By.TagName("a"));
         return result;
      }

      /// <summary>
      /// Start downloading a file from the leg site.  Returns once the download is started.
      /// </summary>
      /// <param name="filename">Name of the file to be downloaded from the leg site.</param>
      /// <param name="form1">Need to update progressLegSite, so need the form.</param>
      private async Task<TimeSpan> DownloadLegSiteFileAsync(string filename, Form1 form1) {
         LogThis($"Downloading {filename}, which is the most recent zip file on the leg site.");
         var leg_site_path = Path.Combine(Config.Instance.LegSite, filename);
         var output_path = OutputPath(filename);   // Throw if output file cannot be created.
         ProgressBar progress = form1.progressLegSite;
         var start_time = DateTime.Now;
         using (var client = new HttpClientDownloadWithProgress(leg_site_path, output_path)) {
            client.ProgressChanged += (totalFileSize, totalBytesDownloaded, progressPercentage) => {
               //LogThis($"{progressPercentage} % ({totalBytesDownloaded}/{totalFileSize})");
               progress.Invoke(new Action(() => progress.Value = Convert.ToInt32(progressPercentage)));
            };
            await client.StartDownload();
         }
         var interval = DateTime.Now-start_time;
         return interval;
      }

      /// <summary>
      /// Create the full output path, given the file name.  Throw if that file cannot be created.
      /// If the output file is already open in some other program, then the output cannot be created.
      /// Throw if the output file cannot be created.
      /// Side Effect: This method deletes the current contents of the output file.
      /// </summary>
      /// <param name="filename"></param>
      private string OutputPath(string filename) {
         var full_path = Path.Combine(Config.Instance.DownloadsFolder, filename);
         var test_file_creation = new FileStream(full_path, FileMode.Create);
         test_file_creation.Close();
         return full_path;
      }
   }
}