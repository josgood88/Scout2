using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Library;

namespace Scout2.Sequence {
   public class ZipController : BaseController {
      public void Run(Form1 form1) {
         var start_time = DateTime.Now;
         try {
            var latest = FindLatestZipFile(Config.Instance.DownloadsFolder);  // Zip downloaded from leg site
            var di = new DirectoryInfo(Config.Instance.BillsFolder);          // BillsFolder contains leg site files
            LogAndDisplay(form1.txtZipProgress, $"Clearing contents of {Config.Instance.BillsFolder}.");
            if (di.Exists) di.Delete(true);                             // Clear out the target folder
            var latest_zip_file = Path.Combine(Config.Instance.DownloadsFolder,latest);

            // LatestDownloadFolder contains BillsFolder, zip file contains Bills folder
            LogAndDisplay(form1.txtZipProgress, $"Extracting {latest_zip_file} contents to {Config.Instance.BillsFolder}.");
            ZipFile.ExtractToDirectory(latest_zip_file,Config.Instance.BillsFolder);  // Extract zip file to target folder

            LogAndDisplay(form1.txtZipProgress, "Removing analysis files");
            RemoveAnalysisFiles();
         } catch (Exception ex) {
            LogAndThrow($"ZipController.Run: {ex.Message}.");
         }
         var elapsed = DateTime.Now - start_time;
         LogAndDisplay(form1.txtZipProgress, $"Extraction complete. {elapsed.ToString("c")} ");
      }

      /// Remove bill analysis files from the download folder. 
      private void RemoveAnalysisFiles() {
         var di = new DirectoryInfo(Config.Instance.BillsFolder);            // BillsFolder contains leg site files
         var unwanted_files = di.GetFiles("BILL_ANALYSIS_TBL_*.lob");   // Don't want the analysis files
         foreach (var file in unwanted_files) System.IO.File.Delete(file.FullName);
      }

      /// Find the latest leg site zip file in the download folder. 
      private string FindLatestZipFile(string download_folder) {
         var di = new DirectoryInfo(download_folder);
         FileInfo[] fiArray = di.GetFiles("pubinfo*.zip");
         Array.Sort(fiArray,(x,y) => StringComparer.OrdinalIgnoreCase.Compare(x.LastWriteTime,y.LastWriteTime));
         return fiArray.Last().Name;
      }
   }
}