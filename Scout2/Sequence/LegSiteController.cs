using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using Library;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using Scout2.Utility;
using Keys=OpenQA.Selenium.Keys;

namespace Scout2.Sequence {
   public partial class LegSiteController : BaseController {
      private IWebDriver Driver { get; set; }
      public void Run(Form1 form1) {
         var start_time = DateTime.Now;
         try {
            InitializeChrome();
            //var abc = DownloadCommitteeAgendas(form1);
            DownloadLatestZip(form1);
         } finally {
            CloseChrome();
         }
         var elapsed = DateTime.Now - start_time;
         LogAndDisplay(form1.txtLegSiteCompletion, $"Downloads completed. Elapsed Time: {elapsed.ToString("c")} ");
      }
      /// <summary>
      /// Download the latest zip file from the legislature download site.  This zip contains all versions of all bills
      /// so far this biennium.  It also contains tables for the database and other files.  The next step in the sequence
      /// is responsible for unzipping the zip file.
      /// </summary>
      /// <param name="form1">The main Scout2 progress display</param>
      private void DownloadLatestZip(Form1 form1) {
         form1.TopMost = true;             // Don't let Selenium hide this program's form
         LogAndDisplay(form1.txtLegSiteCompletion, "Finding and then downloading latest zip file.  This can take 10-20 minutes.");
         Driver.Navigate().GoToUrl(Config.Instance.LegSite);

         Task t = Task.Run(async () => {
            try {
               FindLastModified().Click();         // Sort zip files in ascending date order
               FindLastModified().Click();         // Sort zip files in descending date order (stale fields, hence call again)
               var rows = ViewHrefs().ToList();    // Collect hrefs of the available zip files
               using (var etr = rows.GetEnumerator()) {
                  bool found_zip = false;
                  while (!found_zip) {
                     if (!etr.MoveNext()) {
                        MessageBox.Show("LegsiteController.Run failed to find any pubinfo zip files.");
                     } else if (etr.Current != null) {
                        if (etr.Current.Text.Contains("pubinfo_daily")) {
                           found_zip = true;
                           Driver.Manage().Window.Minimize();  // Hide the chrome window, making the application visable again
                           await DownloadLegSiteFileAsync(etr.Current.Text, form1);
                        }
                     }
                  }
               }
            } catch (Exception ex) {
               MessageBox.Show($"LegSiteController.Run: {ex.Message}");
            }
         });

         t.Wait();
         form1.TopMost = false;           // Don't let Selenium hide this program's form
      }
      /// <summary>
      /// Download the available agendas for those legislature committees whose agenda URLs are defined in
      /// D:/CCHR/Projects/Scout2/ConfigurationData/Committees.json.
      /// </summary>
      /// <param name="form1">The main Scout2 progress display</param>
      private List<CommitteeAgenda> DownloadCommitteeAgendas(Form1 form1) {
         var result = new List<CommitteeAgenda>();
         //form1.TopMost = true;            // Don't let Selenium hide this program's form
         var work_list = Config.Instance.Agendas;
         foreach (var committee_spec in work_list) {
            result.Add(DownloadAgendasForOneCommittee(form1, committee_spec));
         }
         //form1.TopMost = false;           // Don't let Selenium hide this program's form
         return result;
      }
      /// <summary>
      /// Download the agenda items for a single committee
      /// </summary>
      /// <param name="form1">The main Scout2 progress display</param>
      /// <param name="spec">Download agenda items for this committee</param>
      /// <returns></returns>
      private CommitteeAgenda DownloadAgendasForOneCommittee(Form1 form1, KeyValuePair<string,string> spec) {
         var result = new CommitteeAgenda();
         var committee_name = spec.Key;
         var url_and_type = spec.Value.Split('|');
         if (url_and_type.Count() != 2) throw new ApplicationException($"DownloadAgendasForOneCommittee: Wrong number of Value fields - {url_and_type}");
         var agendas_url = url_and_type.First();
         var style = url_and_type.Last();
         //LogAndDisplay(form1.txtLegSiteCompletion, $"Downloading agendas for {committee_name}");
         Driver.Navigate().GoToUrl(agendas_url);
         if (style == "PDF") result.hearings.AddRange(FetchFromListOfPDFs(agendas_url, form1));

         Task t = Task.Run(async () => {
            try {
               InitializeChrome();
               FindLastModified().Click();         // Sort zip files in ascending date order
               FindLastModified().Click();         // Sort zip files in descending date order (stale fields, hence call again)
               var rows = ViewHrefs().ToList();    // Collect hrefs of the available zip files
               using (var etr = rows.GetEnumerator()) {
                  bool found_zip = false;
                  while (!found_zip) {
                     if (!etr.MoveNext()) {
                        MessageBox.Show("LegsiteController.Run failed to find any pubinfo zip files.");
                     } else if (etr.Current != null) {
                        if (etr.Current.Text.Contains("pubinfo_daily")) {
                           found_zip = true;
                           Driver.Manage().Window.Minimize();  // Hide the chrome window, making the application visable again
                           await DownloadLegSiteFileAsync(etr.Current.Text, form1);
                        }
                     }
                  }
               }
            } catch (Exception ex) {
               MessageBox.Show($"LegSiteController.Run: {ex.Message}");
            }
         });

         t.Wait();
         return result;
      }
      /// <summary>
      /// Committees like Assembly Health show their agenda items as a list of hot links to pdf documents.
      /// This method handles that case
      /// </summary>
      /// <param name="agendas_url"></param>
      /// <returns></returns>
      private List<Hearing> FetchFromListOfPDFs(string agendas_url, Form1 form1) {
         var result = new List<Hearing>();
         var pdf_documents = ItemsByText("{pdf}");
         foreach (var item in pdf_documents) {
            item.Click();
            // https://ahea.assembly.ca.gov/sites/ahea.assembly.ca.gov/files/March%2019%202019%20Agenda.pdf
            var path = "https://ahea.assembly.ca.gov/sites/ahea.assembly.ca.gov/files/March%2019%202019%20Agenda.pdf";
            SimpleDownload(path, form1);
            ConvertPdfToText(Path.Combine(Config.Instance.DownloadsFolder, "March 19 2019 Agenda.pdf"));

            //// https://ahea.assembly.ca.gov/sites/ahea.assembly.ca.gov/files/March%2019%202019%20Agenda.pdf
            //string filename = item.Text.Replace("{pdf}", string.Empty);
            //string item_month = Regex.Replace(filename, @"(\w+) .*", "$1").ToString();
            //string item_day = Regex.Replace(filename, @"\w+\s+(\d+).*", "$1").ToString();
            //string item_year = Regex.Replace(filename, @".*?,\s+(\d+).*", "$1").ToString();
            //if (!int.TryParse(item_year, out int int_year)) 
            //   throw new ApplicationException($"FetchFromListOfPDFs: Unable to convert parse year from {filename}");
            //DateUtils.Biennium(out int first_year, out int second_year);
            //string xx = $"{item_month}%{first_year}%{second_year}{item_day}%{int_year%100}Agenda.pdf";

            //var new_documents = ItemsByText("{pdf}");
            //Driver.Navigate().GoToUrl("https://google.com/");
            Actions a = new Actions(Driver);
            //a.SendKeys(Keys.End);
            //a.KeyDown(OpenQA.Selenium.Keys.Shift).SendKeys("ThisWillBePrintedCAPS")
            // .KeyUp(OpenQA.Selenium.Keys.Shift).SendKeys("ThisWillBePrintedRegular").Build().Perform();
            a.KeyDown(OpenQA.Selenium.Keys.Control).SendKeys("S")    // "Ctrl + s" Open options to save the current page
             .KeyUp(OpenQA.Selenium.Keys.Control).Build().Perform();
            Actions b = new Actions(Driver);
            b.KeyDown(Keys.Alt).SendKeys("S").Build().Perform();     // "Alt + s"  Press the Save button
            b.KeyUp(Keys.Alt);

            Driver.Navigate().Back();  // Back to the list of pdf documents
         }
         return result;
      }

      private void InitializeChrome() {
         TimeSpan page_load_timeout = new TimeSpan(0, 0, 60);
         Driver = new ChromeDriver();
         Driver.Manage().Timeouts().PageLoad = page_load_timeout;
         Driver.Manage().Window.Maximize();
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
               LogThis($"{progressPercentage} % ({totalBytesDownloaded}/{totalFileSize})");
               //progress.Invoke(new Action(() => progress.Value = Convert.ToInt32(progressPercentage)));
            };
            await client.StartDownload();
         }
         var interval = DateTime.Now-start_time;
         return interval;
      }

      private void SimpleDownload(string url, Form1 form1) {
         try {
            using (var client = new WebClient()) {
               var contents = client.DownloadString(url);
               var output_path = OutputPath(Path.GetFileName(url));   // Throw if output file cannot be created.
               FileUtils.WriteTextFile(contents, output_path);
            }
         } catch (Exception ex) {
            var msg = $"LegSiteController.SimpleDownload: Exception \"{ex.Message}\" downloading {url}.";
            LogAndDisplay(form1.txtLegSiteCompletion, msg);
            throw new ApplicationException(msg);
         }
      }

      /// <summary>
      /// Create the full output path, given the file name.  Throw if that file cannot be created.
      /// If the output file is already open in some other program, then the output cannot be created.
      /// Throw if the output file cannot be created.
      /// Side Effect: This method deletes the current contents of the output file.
      /// </summary>
      /// <param name="_filename"></param>
      private string OutputPath(string _filename) {
         var filename = _filename.Replace("%20", " ");
         var full_path = Path.Combine(Config.Instance.DownloadsFolder, filename);
         var test_file_creation = new FileStream(full_path, FileMode.Create);
         test_file_creation.Close();
         return full_path;
      }
      /// <summary>
      /// Convert a PDF file to text.  Return the text.
      /// </summary>
      /// <param name="input_path">Path to the PDF file</param>
      /// <returns></returns>
      private void ConvertPdfToText(string input_path) {
         var result = new List<string>();
         try {
            var pdfParser = new PdfToText.PDFParser();
            var filename = Path.GetFileNameWithoutExtension(input_path) + ".txt";
            if (string.IsNullOrEmpty(filename)) throw new ApplicationException($"ConvertPdfToText: Null filename in {input_path}");
            filename += ".txt";
            var folder = Path.GetDirectoryName(input_path);
            if (string.IsNullOrEmpty(folder)) throw new ApplicationException($"ConvertPdfToText: Null folder in {input_path}");
            var output_path = Path.Combine(folder, filename);
            pdfParser.ExtractText(input_path, output_path);
         } catch (Exception ex) {
            string msg = $"ConvertPdfToText: Error \"{ex.Message}\" processing {input_path}.";
            LogAndThrow(msg);
         }
      }
   }
}