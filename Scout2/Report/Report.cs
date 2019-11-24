using System;
using System.Collections.Generic;
using System.IO;

namespace Scout2.Report {
   public class Report {
      private readonly string output_folder;
      private readonly string path_log_file;
      private readonly string report_folder;

      public Report(string output_folder, string path_log_file, string report_folder) {
         this.output_folder = output_folder; 
         this.path_log_file = path_log_file;
         this.report_folder = report_folder;
      }

      public void Generate() {
         string weekly_report_path = ReportPath();
         var reports = new BillReportCollection(report_folder);
         using (var sw = new StreamWriter(weekly_report_path)) {
            Header(sw);
            NewThisWeek(sw);
            HighestPriority(sw);
            ChangesThisWeek(sw);
            UpcomingCommitteeHearingsOfInterest(sw);
            Oppose(sw);
            Modify_Monitor(sw);
            Chaptered(sw);
            RemainingLegislativeSchedule(sw);
            Definitions(sw);
            End(sw);
         }
      }

      private string ReportPath() { return Path.Combine(output_folder,"WeeklyNewsMonitoredBills.html"); }

      private void Header(StreamWriter sw) {
         string date = ReportDate(); // e.g., "16 Sep 2019";
         sw.WriteLine("<!DOCTYPE html PUBLIC \" // -//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">");
         sw.WriteLine("<html xmlns=\"http://www.w3.org/1999/xhtml\">");
         sw.WriteLine("<head>");
         sw.WriteLine($"   <title> News, Monitored Bills, {date}. </title>");
         sw.WriteLine("</head>");
         sw.WriteLine("<body>");
         sw.WriteLine($"   <b> News, Monitored Bills, {date}.</b>");
         sw.WriteLine("   <br />");
         sw.WriteLine("   <br />");
      }

      private void NewThisWeek(StreamWriter sw) {
         StartTable(sw,"New This Week");
         EndTable(sw);
      }

      private void HighestPriority(StreamWriter sw) {
         StartTable(sw, "Highest Priority Bills");
         EndTable(sw);
      }

      private void ChangesThisWeek(StreamWriter sw) {
         StartTable(sw, "Changes This Week");
         EndTable(sw);
      }

      private void UpcomingCommitteeHearingsOfInterest(StreamWriter sw) {
         sw.WriteLine("");
         sw.WriteLine("   <table border=\"1\">");
         sw.WriteLine("      <caption><b>Upcoming Committee Hearings Of Interest</b></caption>");
         sw.WriteLine("      <tr>");
         sw.WriteLine("         <th>Measure</th>");
         sw.WriteLine("         <th>Author</th>");
         sw.WriteLine("         <th>Topic</th>");
         sw.WriteLine("         <th>Date</th>");
         sw.WriteLine("         <th>Committe</th>");
         sw.WriteLine("         <th>Room</th>");
         sw.WriteLine("         <th>Day</th>");
         sw.WriteLine("         <th>Time</th>");
         sw.WriteLine("      </tr>");
         sw.WriteLine("   </table>");
         sw.WriteLine("   <br />");
         sw.WriteLine("   <br />");
      }

      private void Oppose(StreamWriter sw) {
         StartTable(sw, "Oppose");
         EndTable(sw);
      }

      private void Modify_Monitor(StreamWriter sw) {
         StartTable(sw, "Modify/Monitor");
         EndTable(sw);
      }

      private void Chaptered(StreamWriter sw) {
         StartTable(sw, "Chaptered");
         EndTable(sw);
      }

      private void RemainingLegislativeSchedule(StreamWriter sw) {
         sw.WriteLine(RemainingSchedule.AsString());
      }

      private void Definitions(StreamWriter sw) {
         sw.WriteLine(Scout2.Report.Definitions.AsString());
      }

      private void End(StreamWriter sw) {
         sw.WriteLine("</body>");
         sw.WriteLine("</html>");
      }

      /// <summary>
      /// Report date is always the Monday following today
      /// </summary>
      /// <returns></returns>
      private string ReportDate() {
         Dictionary<int, int> incrementTodayToReportDate = new Dictionary<int, int>() {
         // Sun    Mon    Tue    Wed    Thu    Fri    Sat
            {0,1}, {1,0}, {2,6}, {3,5}, {4,4}, {5,3}, {6,2} };
         var now = DateTime.Now;
         int dayOfWeek = (int)now.DayOfWeek; // 0 = Sunday
         int increment_by = incrementTodayToReportDate[dayOfWeek];
         var report_date = now.AddDays(increment_by);
         var result = report_date.ToString("dd MMM yyyy");
         return result;
      }

      private void StartTable(StreamWriter sw, string title) {
         sw.WriteLine("");
         sw.WriteLine("   <table border=\"1\">");
         sw.WriteLine($"      <caption><b>{title}</b></caption>");
         sw.WriteLine("      <tr>");
         sw.WriteLine("         <th>Measure</th>");
         sw.WriteLine("         <th>WIC</th>");
         sw.WriteLine("         <th>5000</th>");
         sw.WriteLine("         <th>Position</th>");
         sw.WriteLine("         <th>Summary</th>");
         sw.WriteLine("         <th>Last Change</th>");
         sw.WriteLine("      </tr>");
      }

      private void EndTable(StreamWriter sw) {
         sw.WriteLine("   </table>");
         sw.WriteLine("   <br />");
         sw.WriteLine("   <br />");
      }
   }
}
