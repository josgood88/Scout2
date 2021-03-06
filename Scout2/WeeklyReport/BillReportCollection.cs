﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Library;
using Library.Database;
using Scout2.Sequence;
using Scout2.Utility;

namespace Scout2.WeeklyReport {
   public class BillReport {
      public string Author { get; private set; }
      public string Measure { get; private set; }
      public string WIC { get; private set; }
      public string LPS { get; private set; }
      public string Position { get; private set; }
      public string OneLiner { get; private set; } // One-line summation
      public string LastAction { get; private set; }
      public string Title { get; private set; }

      private readonly Regex rx_measure = new Regex(@".B-\d+");
      private readonly Regex rx_author = new Regex("\\(.*\\)");

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

            // Find Last Action.  The correct source for Last Action is the history data imported from the 
            // legislature site.  That data is considered original, data emitted by this program is derivative.
            BillUtils.ExtractHouseNumber(Measure, out string house, out string number);
            var bill_id = $"{house}{number}";
            BillHistoryRow mostRecent = GlobalData.HistoryTable.LatestFromHouseNumber(bill_id);
            LastAction = $"{DateUtils.Date(mostRecent.ActionDate)} {mostRecent.Action}";
            LastAction = LastAction.Replace("''", "'"); // Governor''s becomes Governor's

            // Find WIC (Welfare and Institutions Code) and LPS (Lanterman-Petris-Short Act)
            var most_recent = GlobalData.MostRecentEachBill
               .Where(row => row.BillID == BillUtils.Ensure4DigitNumber(Measure)).FirstOrDefault();
            if (most_recent != null) {
               if (File.Exists(most_recent.LobPath)) {
                  var contents = FileUtils.FileContents(most_recent.LobPath);  // Need text without CRLF
                  var match_string = @"Section\s+\d+.*?Welfare\s+and\s+Institutions\s+Code";
                  MatchCollection matches = Regex.Matches(contents, match_string);
                  WIC = matches.Count > 0 ? "Yes" : "No";   // Is WIC if WIC referenced
                  LPS = "No";                               // LPS defaults to "No"
                  if (WIC == "Yes") {
                     foreach (var match in matches) {
                        var str = match.ToString();
                        var section_number = Regex.Match(str, @"\d+").ToString();
                        if (Int64.TryParse(section_number, out long numberResult)) {
                           // If section is between 5000 and 6000, Lanterman-Petris_Short is referenced
                           if (numberResult >= 5000 && numberResult < 6000) LPS = "Yes";
                        }
                     }
                  }
               }
            } else {
               //throw new ApplicationException($"BillReport.ctor({file_path}): {Measure} not in GlobalData.MostRecentEachBill");
            }

         } catch (Exception ex) {
            BaseController.LogAndShow(ex.Message);
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

      private string House(string measure) { return Regex.Match(measure, "^[A-Z]+").ToString(); }
      private string BillNumber(string measure) { return Regex.Replace(measure, @".*?(\d+)", "$1").ToString(); }
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
         return Position.Contains("Oppose") ? true : false;
      }
      /// <summary>
      /// Answer whether our position on a bill is Modify or Monitor
      /// </summary>
      /// <returns></returns>
      public bool IsPositionModifyOrMonitor() {
         return Position.Contains("Modify") || Position.Contains("Monitor") ? true : false;
      }
      /// <summary>
      /// Answer whether a bill is chaptered by examining the bill's history.
      /// Ignore all bills on which our position is "None".
      /// </summary>
      /// <returns></returns>
      public bool IsChaptered() {
         if (Position.Contains("Chaptered")) return true;
         var measure = Regex.Replace(Measure, "(.*?)-(.*)", "$1$2");
         var location = Path.Combine(Config.Instance.HtmlFolder, $"{measure}.html");
         var lines = File.ReadLines(location).ToList();
         var line = lines.Find(x => x.Contains("Chaptered by Secretary of State"));
         return (line != null) ? true : false;
      }
      public bool IsNotChaptered() { return !IsChaptered(); }
      /// <summary>
      /// Answer whether a bill is dead. Reviewer notes this by writing "This bill is dead" somewhere in the review.
      /// Notation is usually made as the first line in the Summary block of lines.
      /// </summary>
      /// <returns>Whether any line in the report contains an IsDead signature</returns>
      public bool IsDead() {
         if (Position.Contains("Dead")) return true;
         var measure = Regex.Replace(Measure, "(.*?)-(.*)", "$1$2");
         var location = Path.Combine(Config.Instance.HtmlFolder, $"{measure}.html");
         var lines = File.ReadLines(location).ToList();
         return Whether.Instance.IsDeadSignature(lines);
      }

      public bool IsNotDead() { return !IsDead(); }
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
         if (GlobalData.MostRecentEachBill.Count == 0) { // True if starting later in sequence than "Import Into Database"
            GlobalData.MostRecentEachBill = MostRecentBills.Identify(Config.Instance.BillsFolder);
         }
         List<string> files = Directory.GetFiles(report_folder, "*.html").ToList();
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
