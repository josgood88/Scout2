using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Library;

namespace Scout2.Report {
   public class BillReport {
      public string Author     { get; private set; }
      public string Measure    { get; private set; }
      public string WIC        { get; private set; }
      public string FiveK      { get; private set; }
      public string Position   { get; private set; }
      public string OneLiner   { get; private set; } // One-line summation
      public string LastAction { get; private set; }
      public string Title      { get; private set; }

      private readonly Regex rx_measure = new Regex(@".B-\d+");
      private readonly Regex rx_author  = new Regex("\\(.*\\)");

      public BillReport(string file_path) {
         try {
            var lines = File.ReadLines(file_path).ToList();

            // Find Author, Measure, Title
            var line = lines.Find(x => x.Contains("Title</b>:"));
            if (line == null) throw new ApplicationException($"BillReport ctor: {file_path} contents has no title line.");
            Measure = rx_measure.Match(line).ToString();
            if (!Measure.Contains("B-")) throw new ApplicationException($"BillReport ctor: {file_path} contents has no measure.");
            Author  = rx_author.Match(line).ToString();
            if (!Author.Contains("(") || !Author.Contains(")")) throw new ApplicationException($"BillReport ctor: {file_path} contents has no author.");
            Author = Author.Trim(new[] { '(', ')' });
            var index = line.IndexOf(')');            // Title follows closing author parenthese
            if (index < 0) throw new ApplicationException($"BillReport ctor: {file_path} contents has no closing author parenthese.");
            Title = line.Substring(index+1).Trim();   // At least one space before title

            // Find Position
            line = lines.Find(x => x.Contains("Position</b>:"));
            if (line == null) throw new ApplicationException($"BillReport ctor: {file_path} contents has no position.");
            index = line.IndexOf(':');                   // Position follows colon
            Position = line.Substring(index+1).Trim();   // At least one space before position

            // Find summary statement
            line = lines.Find(x => x.Contains("ShortSummary</b>:"));
            if (line == null) throw new ApplicationException($"BillReport ctor: {file_path} contents has no summary statement.");
            index = line.IndexOf(':');                   // Summary follows colon
            OneLiner = line.Substring(index+1).Trim();   // At least one space before summary

            // Find Last Action
            line = lines.Find(x => x.Contains("Last Action:"));
            if (line == null) throw new ApplicationException($"BillReport ctor: {file_path} contents has no Last Action.");
            index = line.IndexOf(':');                   // Last Action follows colon
            LastAction = line.Substring(index+1).Trim(); // At least one space before Last Action
         } catch (Exception ex) {
            Console.WriteLine(ex.Message);
            throw;
         }
      }

      public int CompareByMeasure(BillReport rhs) {
         string m_lhs = this.Measure, m_rhs = rhs.Measure;
         string house_lhs = House(m_lhs), house_rhs = House(m_rhs);
         if (house_lhs != house_rhs) return house_lhs.CompareTo(house_rhs);
         string bill_lhs = BillNumber(m_lhs), bill_rhs = BillNumber(m_rhs);
         if (!int.TryParse(bill_lhs, out int i_lhs)) throw new ApplicationException($"BillReport.CompareByMeasure: Invalid measure {m_lhs}");
         if (!int.TryParse(bill_rhs, out int i_rhs)) throw new ApplicationException($"BillReport.CompareByMeasure: Invalid measure {m_rhs}");
         return i_lhs.CompareTo(i_rhs);
      }

      private string House(string measure) { return Regex.Match(measure, "^[A-Z]+").ToString();  }
      private string BillNumber(string measure) { return Regex.Replace(measure,@".*?(\d+)","$1").ToString(); }
      /// <summary>
      /// Answer whether our position on a bill is None -- we have no position.
      /// </summary>
      /// <returns></returns>
      public bool IsPositionNone() { return (Position == "None") ? true : false; }
      /// <summary>
      /// Answer whether our position on a bill is Oppose
      /// </summary>
      /// <returns></returns>
      public bool IsPositionOppose() {
         return (Position == "Oppose") ? true : false;
      }
      /// <summary>
      /// Answer whether our position on a bill is Modify or Monitor
      /// </summary>
      /// <returns></returns>
      public bool IsPositionModifyOrMonitor() {
         return (Position == "Modify" || Position == "Monitor") ? true : false;
      }
      /// <summary>
      /// Answer whether a bill is chaptered by examining the bill's history.
      /// Ignore all bills on which our position is "None".
      /// </summary>
      /// <returns></returns>
      public bool IsChaptered() {
         if (IsPositionNone()) return false;
         var measure = Regex.Replace(Measure, "(.*?)-(.*)", "$1$2");
         var location = Path.Combine(Config.Instance.HtmlFolder, $"{measure}.html");
         var lines = File.ReadLines(location).ToList();
         var line = lines.Find(x => x.Contains("Chaptered by Secretary of State"));
         return (line != null) ? true : false;
      }
      /// <summary>
      /// Answer whether a bill is chaptered by examining the bill's history.
      /// Ignore all bills on which our position is "None".
      /// </summary>
      /// <returns></returns>
      public bool IsDead() {
         if (IsPositionNone()) return false;
         var measure = Regex.Replace(Measure, "(.*?)-(.*)", "$1$2");
         var location = Path.Combine(Config.Instance.HtmlFolder, $"{measure}.html");
         var lines = File.ReadLines(location).ToList();
         var line = lines.Find(x => x.Contains("Chaptered by Secretary of State"));
         return (line != null) ? true : false;
      }
   }

   /// <summary>
   /// Provide an IEnumerable collection of BillReport, each of which is created from an actual bill report.
   /// This is used in generating the "Changes This Week" section of the report.
   /// </summary>
   public class BillReportCollection : IEnumerable<BillReport> {
      private string report_folder { get; set; }
      private readonly List<BillReport> reports;
      /// <summary>
      /// Create a collection of bill reports by iterating over the contents of the passed folder path.
      /// Ignore the file named WeeklyNewsMonitoredBills.html, as it is the weekly report.
      /// Each of the remaining files is a single bill report.  Construct a BillReport for each.
      /// </summary>
      /// <param name="_report_folder">Path to folder containing individual bill reports.</param>
      public BillReportCollection(string _report_folder) {
         report_folder = _report_folder;
         reports = new List<BillReport>();
         List<string> files = Directory.GetFiles(report_folder,"*.html").ToList();
         foreach (var file in files) {
            if (!file.Contains("WeeklyNewsMonitoredBills")) reports.Add(new BillReport(file));
         }
         reports.Sort((a, b) => a.CompareByMeasure(b));
      }
      /// <summary>
      /// IEnumerable of T requires an implementation of GetEnumerator().
      /// </summary>
      public IEnumerator<BillReport> GetEnumerator() {
         foreach (BillReport report in reports) {
            yield return report;
         }
      }
      IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
   }
}
