using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Library;
using Scout2.Sequence;

namespace Scout2.Report {
   public class Report : BaseController {
      private readonly string output_folder;
      private readonly string path_log_file;
      private readonly string report_folder;

      private class DateRange {
         public DateTime start { get; set; }
         public DateTime end { get; set; }
      }

      public Report(string output_folder, string path_log_file, string report_folder) {
         this.output_folder = output_folder; 
         this.path_log_file = path_log_file;
         this.report_folder = report_folder;
      }

      public void Generate() {
         string weekly_report_path = ReportPath();
         var reports = new BillReportCollection(report_folder);
         var past_week = PastWeek();
         using (var sw = new StreamWriter(weekly_report_path)) {
            Header(sw);
            NewThisWeek(reports, past_week,sw);
            HighestPriority(sw);
            ChangesThisWeek(reports, past_week, sw);
            UpcomingCommitteeHearingsOfInterest(sw);
            Oppose(sw, reports);
            Modify_Monitor(sw, reports);
            Chaptered(sw, reports);
            RemainingLegislativeSchedule(sw);
            Definitions(sw);
            End(sw);
         }
      }

      private string ReportPath() { return Path.Combine(Config.Instance.HtmlFolder,"WeeklyNewsMonitoredBills.html"); }

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
      /// <summary>
      /// Report any bill that is new this week.
      /// </summary>
      /// <param name="reports">Collected bill reports</param>
      /// <param name="past_week">The dates bounding the past calendar week</param>
      /// <param name="sw">StreamWriter which will be written to the report file</param>
      private void NewThisWeek(BillReportCollection reports, DateRange past_week, StreamWriter sw) {
         StartTable(sw,"New This Week");
         //foreach (var report in reports) {
         //   var dt = DateFromLastAction(report);
         //   if (DateIsInPastWeek(dt, past_week)) { }
         //   ReportOneBill(sw, report);
         //}
         EndTable(sw);
      }

      private void HighestPriority(StreamWriter sw) {
         StartTable(sw, "Highest Priority Bills");
         EndTable(sw);
      }
      /// <summary>
      /// Report any bill that changed this week.
      /// </summary>
      /// <param name="reports">Collected bill reports</param>
      /// <param name="past_week">The dates bounding the past calendar week</param>
      /// <param name="sw">StreamWriter which will be written to the report file</param>
      private void ChangesThisWeek(BillReportCollection reports, DateRange past_week, StreamWriter sw) {
         StartTable(sw, "Changes This Week");
         foreach (var report in reports) {
            var dt = DateFromLastAction(report);
            if (DateIsInPastWeek(dt, past_week)) {
               ReportOneBill(sw, report);
            }
         }
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

      private void Oppose(StreamWriter sw, BillReportCollection reports) {
         StartTable(sw, "Oppose");
         foreach (var report in reports) {
            if (report.IsDead()) continue;   // Don't bother reporting dead bills
            if (report.IsPositionOppose()) ReportOneBill(sw, report);
         }
         EndTable(sw);
      }

      private void Modify_Monitor(StreamWriter sw, BillReportCollection reports) {
         StartTable(sw, "Modify/Monitor");
         foreach (var report in reports) {
            if (report.IsDead()) continue;      // Don't bother reporting dead bills
            if (report.IsChaptered()) continue; // Chaptered bills are reported elsewhere
            if (report.IsPositionModifyOrMonitor()) ReportOneBill(sw, report);
         }
         EndTable(sw);
      }

      private void Chaptered(StreamWriter sw, BillReportCollection reports) {
         StartTable(sw, "Chaptered");
         foreach (var report in reports) {
            if (report.IsPositionNone()) continue;    // Don't bother reporting bills on which we have no position
            if (report.IsChaptered()) ReportOneBill(sw, report);
         }
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

      private void ReportOneBill(StreamWriter sw, BillReport report) {
         sw.WriteLine("<tr>");
         sw.WriteLine($"<td>{report.Measure} {report.Title}</td>");
         sw.WriteLine($"  <td>{report.WIC??"No"}</td>");
         sw.WriteLine($"  <td>{report.FiveK??"No"}</td>");
         sw.WriteLine($"  <td>{report.Position??"Null"}</td>");
         sw.WriteLine($"  <td>{report.OneLiner}</td>");
         sw.WriteLine($"  <td>{report.LastAction}</td>");
         sw.WriteLine("</tr>");
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

      private DateRange PastWeek() {
         DateRange result = new DateRange();
         result.end = DateTime.Now;
         result.start = result.end - TimeSpan.FromDays(7);
         return result;
      }

      private DateTime DateFromLastAction(BillReport report) {
         DateTime result = DateTime.MinValue;
         var text_date = Regex.Match(report.LastAction, @"^\w+.\s+\w+.\s+\w+").ToString();
         DateTime.TryParse(text_date, out result);
         return result;
      }

      private bool DateIsInPastWeek(DateTime dt, DateRange range) {
         return dt >= range.start && dt <= range.end;
      }
   }
}
