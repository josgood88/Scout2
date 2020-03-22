using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Library;
using Scout2.Sequence;
using Scout2.Utility;

namespace Scout2.WeeklyReport {
   public class WeeklyReport : BaseController {
      private readonly string output_folder;
      private readonly string path_log_file;
      private readonly string report_folder;

      public class DateRange {
         public DateTime start { get; private set; }
         public DateTime end { get; private set; }
         public DateRange() { }
         public DateRange(DateTime _start, DateTime _end) { start = _start; end = _end;  }
         public DateRange(string _start, string _end) {
            bool start_success = (DateTime.TryParse(_start, out DateTime start_date));
            if (!start_success) throw new ApplicationException($"DateRange ctor: invalid start date {_start}");
            bool end_success = (DateTime.TryParse(_end, out DateTime end_date));
            if (!end_success) throw new ApplicationException($"DateRange ctor: invalid end date {_end}");
            start = start_date;
            end = end_date;
         }
      }

      public WeeklyReport(string output_folder, string path_log_file, string report_folder) {
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
            HighestPriority(sw, reports, past_week);  // Our highest priority bills
            NewThisWeek(sw, reports, past_week);      // New bills of interest this week
            ChangesThisWeek(sw, reports, past_week);  // Changes this week in bills of interest
            //UpcomingCommitteeHearingsOfInterest(sw);// Committee hearings for bills of interest
            Oppose(sw, reports, past_week);           // Bills for which our position is Oppose
            Modify_Monitor(sw, reports, past_week);   // Bills for which our position is Monitor or Modify
            Predictions(sw, reports, past_week);      // Per-bill expected path through committees
            Chaptered(sw, reports);                   // Bills of interest which chaptered this biennium
            Dead(sw, reports);                        // Bills of interest which died this biennium
            RemainingLegislativeSchedule(sw);         // What's left on this year's schedule
            Definitions(sw);                          // Definitions of terms
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
      /// A bill is new if it was initially reviewed during the past week.
      /// </summary>
      /// <param name="reports">Collected bill reports</param>
      /// <param name="past_week">The dates bounding the past calendar week</param>
      /// <param name="sw">StreamWriter which will be written to the report file</param>
      private void NewThisWeek(StreamWriter sw, BillReportCollection reports, DateRange past_week) {
         StartTable(sw,"New Of-Interest Bills This Week");
         foreach (var report in reports) {
            string path = $"{Path.Combine(Config.Instance.HtmlFolder, BillUtils.EnsureNoLeadingZerosBill(report.Measure))}.html";
            if (File.Exists(path)) { 
               string contents = FileUtils.FileContents(path);
               if (BillUtils.IsNewThisWeek( contents, past_week)) {
                  if (report.IsDead()) continue;            // Don't bother reporting dead bills
                  if (report.IsPositionNone()) continue;    // Don't bother reporting bills on which we have no position
                  if (report.IsChaptered()) continue;       // Don't bother reporting chaptered bills
                  ReportOneBill(sw, report);
               }
            }
         }
         EndTable(sw);
      }

      private void HighestPriority(StreamWriter sw, BillReportCollection reports, DateRange past_week) {
         StartTable(sw, "Highest Priority Bills");
         foreach (string bill in Config.Instance.HighestPriority) {
            var measure = BillUtils.EnsureDashAndNoLeadingZeros(bill);
            var report = (from item in reports where (item.Measure == measure) select item).FirstOrDefault();
            if (report != null) {
               string prefix = BillUtils.NewOrChangePrefix(past_week, report);
               ReportOneBillWithPrefix(sw, report, prefix);
            }
         }
         EndTable(sw); 
      }
      /// <summary>
      /// Report any bill that changed this week.
      /// </summary>
      /// <param name="reports">Collected bill reports</param>
      /// <param name="past_week">The dates bounding the past calendar week</param>
      /// <param name="sw">StreamWriter which will be written to the report file</param>
      private void ChangesThisWeek(StreamWriter sw, BillReportCollection reports, DateRange past_week) {
         StartTable(sw, "Changes This Week in Bills Of Interest");
         foreach (var report in reports) {
            if (report.IsPositionNone()) continue;    // Don't bother reporting bills on which we have no position
            if (report.IsDead()) continue;            // Don't bother reporting dead bills (e.g. Joint Rule 56)

            string path = $"{Path.Combine(Config.Instance.HtmlFolder, BillUtils.EnsureNoLeadingZerosBill(report.Measure))}.html";
            if (File.Exists(path)) {
               string contents = FileUtils.FileContents(path);
               if (BillUtils.IsNewThisWeek(contents, past_week)) continue; // Don't report new bills.

               var dt = DateFromLastAction(report);
               if (DateUtils.DateIsInPastWeek(dt, past_week)) {
                  ReportOneBill(sw, report);
               }
            }
         }
         EndTable(sw);
      }

      private void Predictions(StreamWriter sw, BillReportCollection reports, DateRange past_week) {
         StartPredictionTable(sw, "Predicted Committee Routing");
         foreach (var report in reports) {
            if (report.IsPositionNone()) continue;    // Don't report bills on which we have no position
            if (report.IsDead()) continue;            // Don't report dead bills (e.g. Joint Rule 56)
            if (report.IsChaptered()) continue;       // Don't report chaptered bills
            string path = $"{Path.Combine(Config.Instance.HtmlFolder, BillUtils.EnsureNoLeadingZerosBill(report.Measure))}.html";
            string committees = IndividualReport.PreviousReport.Committees(path);
            string likelihood = IndividualReport.PreviousReport.Likelihood(path);
            if (committees.Length > 0 || likelihood.Length > 0) {
               ReportPrediction(sw, past_week, report, committees, likelihood);
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
         EndTable(sw);
      }

      private void Oppose(StreamWriter sw, BillReportCollection reports, DateRange past_week) {
         StartTable(sw, "Oppose");
         foreach (var report in reports) {
            if (report.IsDead()) continue;      // Don't report dead bills
            if (report.IsChaptered()) continue; // Chaptered bills are reported elsewhere
            if (report.IsPositionOppose()) {
               string prefix = BillUtils.NewOrChangePrefix(past_week, report);
               ReportOneBillWithPrefix(sw, report, prefix);
            }
         }
         EndTable(sw);
      }

      private void Modify_Monitor(StreamWriter sw, BillReportCollection reports, DateRange past_week) {
         StartTable(sw, "Modify/Monitor");
         foreach (var report in reports) {
            if (report.IsDead()) continue;      // Don't bother reporting dead bills
            if (report.IsChaptered()) continue; // Chaptered bills are reported elsewhere
            if (report.IsPositionModifyOrMonitor()) {
               string prefix = BillUtils.NewOrChangePrefix(past_week, report);
               ReportOneBillWithPrefix(sw, report,prefix);
            }
         }
         EndTable(sw);
      }

      private void Chaptered(StreamWriter sw, BillReportCollection reports) {
         StartTable(sw, "Of-Interest Bills Chaptered This Biennium");
         foreach (var report in reports) {
            if (report.IsPositionNone()) continue;    // Don't bother reporting bills on which we have no position
            if (report.IsChaptered()) ReportOneBill(sw, report);
         }
         EndTable(sw);
      }

      private void Dead(StreamWriter sw, BillReportCollection reports) {
         StartTable(sw, "Of-Interest Bills Dead This Biennium");
         foreach (var report in reports) {
            if (report.IsPositionNone()) continue;    // Don't bother reporting bills on which we have no position
            if (report.IsDead()) ReportOneBill(sw, report);
         }
         EndTable(sw);
      }


      private void RemainingLegislativeSchedule(StreamWriter sw) {
         sw.WriteLine(RemainingSchedule.AsString());
      }

      private void Definitions(StreamWriter sw) {
         sw.WriteLine(Scout2.WeeklyReport.Definitions.AsString());
      }

      private void End(StreamWriter sw) {
         sw.WriteLine("</body>");
         sw.WriteLine("</html>");
      }

      private void ReportOneBill(StreamWriter sw, BillReport report) {
         sw.WriteLine("<tr>");
         sw.WriteLine($"<td>{report.Measure} {report.Title} ({report.Author})</td>");
         sw.WriteLine($"  <td>{report.WIC}</td>");
         sw.WriteLine($"  <td>{report.LPS}</td>");
         sw.WriteLine($"  <td>{report.Position??"Null"}</td>");
         sw.WriteLine($"  <td>{report.OneLiner}</td>");
         sw.WriteLine($"  <td>{report.LastAction}</td>");
         sw.WriteLine("</tr>");
      }

      private void ReportOneBillWithPrefix(StreamWriter sw, BillReport report, string prefix) {
         sw.WriteLine("<tr>");
         sw.WriteLine($"<td>{prefix}{report.Measure} {report.Title} ({report.Author})</td>");
         sw.WriteLine($"  <td>{report.WIC}</td>");
         sw.WriteLine($"  <td>{report.LPS}</td>");
         sw.WriteLine($"  <td>{report.Position??"Null"}</td>");
         sw.WriteLine($"  <td>{report.OneLiner}</td>");
         sw.WriteLine($"  <td>{report.LastAction}</td>");
         sw.WriteLine("</tr>");
      }

      private void ReportPrediction(StreamWriter sw, DateRange past_week, BillReport report, string committees, string likelihood) {
         string prefix = BillUtils.NewOrChangePrefix(past_week, report);
         sw.WriteLine("<tr>");
         sw.WriteLine($"<td>{prefix} {report.Measure} {report.Title} ({report.Author})</td>");
         sw.WriteLine($"<td>{committees}</td> <td>{likelihood}</td>");
         sw.WriteLine("</tr>");
      }
      /// <summary>
      /// Report date is always the Monday following today
      /// </summary>
      /// <returns></returns>
      private string ReportDate() {
         return DateUtils.NextMonday().ToString("dd MMM yyyy");
      }

      private void StartTable(StreamWriter sw, string title) {
         sw.WriteLine("");
         sw.WriteLine("   <table border=\"1\">");
         sw.WriteLine($"      <caption><b>{title}</b></caption>");
         sw.WriteLine("      <tr>");
         sw.WriteLine("         <th>Measure</th>");
         sw.WriteLine("         <th>WIC</th>");    // Welfare and Institutions Code
         sw.WriteLine("         <th>LPS</th>");    // Lanterman-Petris-Short Act
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

      private void StartPredictionTable(StreamWriter sw, string title) {
         sw.WriteLine("");
         sw.WriteLine("   <table border=\"1\">");
         sw.WriteLine($"      <caption><b>{title}</b></caption>");
         sw.WriteLine("      <tr>");
         sw.WriteLine("         <th>Measure</th>");
         sw.WriteLine("         <th>Prediction</th>");
         sw.WriteLine("         <th>Passage Likelihood</th>");
         sw.WriteLine("      </tr>");
      }

      private DateRange PastWeek() {
         var range_end = DateUtils.NextMonday();
         var end = new DateTime(range_end.Year, range_end.Month, range_end.Day);
         var start = range_end - TimeSpan.FromDays(7);
         return new DateRange(start,end);
      }

      private DateTime DateFromLastAction(BillReport report) {
         var text_date = Regex.Match(report.LastAction, @"^\w+\s+\w+\s+\w+").ToString();
         return (DateTime.TryParse(text_date, out DateTime result)) ? result : default(DateTime);
      }
   }
}
