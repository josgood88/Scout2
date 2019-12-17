using System;
using Library;
using Scout2.Report;

namespace Scout2.Sequence {
   public class WeeklyReport : BaseController {
      public void Run(Form1 form1) {
         var start_time = DateTime.Now;
         try {
            var output_folder = "D:/Scratch";
            var path_log_file = "D:/Scratch/Scout.log";
            new Scout2.Report.Report(output_folder, path_log_file, Config.Instance.HtmlFolder).Generate();
         } catch (Exception ex) {
            LogAndThrow($"ReportController.Run: {ex.Message}.");
         }
         var elapsed = DateTime.Now - start_time;
         var message = $"Report generation complete. {elapsed.ToString("c")} ";
         LogThis(message);
         form1.txtReportProgress.Text = message;
         form1.txtReportProgress.Update();
      }
   }
}