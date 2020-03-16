using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Library;
using Library.Database;
using Scout2.Sequence;
using Scout2.Utility;

namespace Scout2.IndividualReport {
   public class IndividualReportCommon : BaseController {
      /// <summary>
      /// Before a bill is chaptered, the last action is the .First() line in the history.
      /// When a bill is chaptered, its history may not end with the "Chaptered by Secretary of State" because
      /// usually multiple actions take place on the same day.  Therefore, for a chaptered bill, report
      /// the line containing "Chaptered by Secretary of State".  It may not be the first line in the history.
      /// </summary>
      /// <param name="row">BillRow describing the bill being processed</param>
      /// <returns></returns>
      protected static string FindLastAction(BillRow row) {
         string name_ext = Path.GetFileName(row.Lob);                   // BillVersionTable bill_xml is unique
         BillVersionRow bv_row = GlobalData.VersionTable.Scalar(name_ext);
         List<BillHistoryRow> history = GlobalData.HistoryTable.RowSet(bv_row.BillID);

         string result = "Could not find last action.";
         if (row.MeasureState != "Chaptered") {
            result = history.First().Action;
         } else {
            var want_this = history.Find(x => x.Action.Contains("Chaptered by Secretary of State"));
            result = want_this.Action;
         }
         return result;
      }
      /// <summary>
      /// Generate a new individual report or re-generate one that already exists.
      /// </summary>
      /// <param name="row">Bill description from the BillRows table</param>
      /// <param name="path">Path to the .lob file for the bill's current version</param>
      /// <returns></returns>
      protected static List<string> BaseReportContents(BillRow row, string path) {
         LogThis("Scout2.Sequence.CreateNewReports.ReportContents");
         string name_ext = Path.GetFileName(row.Lob);                   // BillVersionTable bill_xml is unique
         BillVersionRow bv_row = GlobalData.VersionTable.Scalar(name_ext);
         List<BillHistoryRow> history = GlobalData.HistoryTable.RowSet(bv_row.BillID);
         var location_code = history.First().TernaryLocation;
         var location_code_row = GlobalData.LocationTable.Scalar(location_code);

         string appropriation = bv_row.Appropriation;
         string author = row.Author;
         string bill_id = row.Bill;
         string fiscal = bv_row.FiscalCommittee;
         string house = history.First().PrimaryLocation;
         string last_action = FindLastAction(row);
         string location = location_code_row == null ? BillUtils.WhenNullLocationCode(history) : location_code_row.Description;
         string local_pgm = bv_row.LocalProgram;
         string number = row.MeasureNum.TrimStart('0');
         string title = row.Title;
         string type_house = $"{bill_id.First()}B";
         string vers_id = row.BillVersionID;
         string vote = bv_row.VoteRequired;

         // Position and Summary data come from the previous version of the bill report
         // If the passed path is null or empty, then this method was called when no previous report exists.
         // When regenerating a report, there is a previous report.
         var summary = new List<string>();
         var position = new List<string>();
         var shortsummary = string.Empty;
         var committees = string.Empty;
         var likelihood = string.Empty;
         if (CommonUtils.IsNullOrEmptyOrWhiteSpace(path)) {
            // do nothing
         } else {
            summary = PreviousReport.Summary(path);
            position = PreviousReport.Position(path);
            shortsummary = PreviousReport.ShortSummary(path);
            committees = PreviousReport.Committees(path);
            likelihood = PreviousReport.Likelihood(path);
         }

         // With all necessary data obtained, generate the report file template.  This sets things up for entering the report manually.
         List<string> result = BeginIndividualReport(type_house, number, author, title);
         // Review
         result.AddRange(ReportReview(summary));
         // Position
         result.AddRange(ReportPosition(position));
         // Short Summary, Committees Prediction and Passage Likelihood
         result.AddRange(ReportSummaryPredictLikelihood(shortsummary, committees, likelihood));
         // Status, Location, etc
         result.AddRange(ReportStatusLocationEtc(location, last_action, vote, appropriation, fiscal, local_pgm, history));
         // Bill History
         result.AddRange(ReportHistory(history));
         return result;
      }
      /// <summary>
      /// Write the beginning of an individual report.  This include the XML header and the report title.
      /// </summary>
      /// <param name="type_house">BillRow describing the bill being processed</param>
      /// <param name="number">BillRow describing the bill being processed</param>
      /// <param name="author">BillRow describing the bill being processed</param>
      /// <param name="title">BillRow describing the bill being processed</param>
      /// <returns>List<string> containing the initial lines of the reportg</string></returns>
      protected static List<string> BeginIndividualReport(string type_house, string number,string author, string title) {
         List<string> result = new List<string> {
            "<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">",
            "<html xmlns=\"http://www.w3.org/1999/xhtml\" > ",
            "<head>",
            $"   <title> {type_house}-{number} ({author}) {title}</title>",
            "</head>",
            "<body>",
            "<p>",
            $"<b>Title</b>: {type_house}-{number} ({author}) {title}",
            "</p>"
         };
         return result;
      }
      // Review
      protected static List<string> ReportReview(List<string> summary) {
         if (summary.Count > 0) {
            return summary;
         } else {
            List<string> result = new List<string> {
               "<p>",
               $"<b>Summary</b>: (Reviewed {DateTime.Now.ToShortDateString()})",
               "   <br /> (Quotations taken directly from the bill's language, or from current code)",
               "   <br />",
               "   <br /> This is my review",
               "</p>"
            };
            return result;
         }
      }
      // Position
      protected static List<string> ReportPosition(List<string> position) {
         if (position.Count > 0) return position;
         List<string> result = new List<string> {
            "<p>",
            "   <b>Position</b>: ",
            "   <br /> This is my reason.",
            "</p>"
         };
         return result;
      }
      // Short Summary, Committees Prediction and Passage Likelihood
      protected static List<string> ReportSummaryPredictLikelihood(string shortsummary, string committees, string likelihood) {
         List<string> result = new List<string> {
            $"<b>ShortSummary</b>: {shortsummary}",
            $"<br /><b>Committees</b>: {committees}",
            $"<br /><b>Likelihood</b>: {likelihood}"
         };
         return result;
      }
      // Status, Location Etc
      protected static List<string> ReportStatusLocationEtc(
         string location, string last_action, string vote, string appropriation, 
         string fiscal, string local_pgm, List<BillHistoryRow> history) {
         string str_date = "No history";
         if (history.Count > 0) str_date = history.First().ActionDate;
         List<string> result = new List<string> {
            "<p>",
            "<b>Status</b>:",
            $"<br /> Location: {location}",
            "<table cellspacing=\"0\" cellpadding=\"0\">",
            $"   <tr><td> Last Action: {FormatDate(str_date)} </td><td> &nbsp; &nbsp; {last_action}</td></tr>",
            $"   <tr><td> Vote: {vote}                        </td><td> &nbsp; &nbsp; Appropriation: {appropriation}</td></tr>",
            $"   <tr><td> Fiscal committee: {fiscal}          </td><td> &nbsp; &nbsp; State-mandated local program: {local_pgm} </td></tr>",
            "</table>",
            "</p>"
         };
         return result;
      }
      // Bill History
      protected static List<string> ReportHistory(List<BillHistoryRow> history) {
         List<string> part1 = new List<string> {
            "<p>",
            "   <b>Bill History</b>:"
         };
         List<string> part2 = new List<string>();
         foreach (var item in history) {
            part2.Add($"   <br /> {FormatDate(item.ActionDate)} {item.Action}");
         }
         List<string> part3 = new List<string> {
            "</p>",
            "</body>",
            "</html>"
         };
         List<string> result = new List<string>();
         result.AddRange(part1);
         result.AddRange(part2);
         result.AddRange(part3);
         return result;
      }


   public static string FormatDate(string str_date) {
         string result = String.Empty;
         result = DateTime.TryParse(str_date, out DateTime date_result) 
            ? $"{date_result.ToString("MMM d yyyy")}" : "(invalid date)";
         return result;
      }

      protected static void From(string path, List<string> summary, List<string> position) {
         if (File.Exists(path)) {
            try {
               using (var sr = new StreamReader(path)) {
                  while (!sr.EndOfStream) {
                     string current_line = sr.ReadLine();
                     // TODO Convert from .Net Framework to .Net Core (8.0+ allow ??=)
                     if (String.IsNullOrEmpty(current_line)) current_line = String.Empty;
                     if (current_line.Trim().StartsWith("<b>Summary")) {
                        summary.Add(current_line);
                        bool exit1 = false;
                        while (!sr.EndOfStream && !exit1) {
                           current_line = sr.ReadLine();
                           if (String.IsNullOrEmpty(current_line)) current_line = String.Empty;
                           if (current_line.Trim().StartsWith("<b>Position")) exit1 = true;
                           else summary.Add(current_line);
                        }
                        // Position immediately follows Summary
                        if (current_line.Trim().StartsWith("<b>Position")) {
                           position.Add(current_line);
                           bool exit2 = false;
                           while (!sr.EndOfStream && !exit2) {
                              current_line = sr.ReadLine();
                              if (String.IsNullOrEmpty(current_line)) current_line = String.Empty;
                              if (current_line.Trim().StartsWith("<b>Status")) exit2 = true;
                              else position.Add(current_line);
                           }
                        }
                     }
                  }
               }
            } catch (Exception ex) {
               throw new ApplicationException($"PreviousReport.From: {ex.Message}");
            }
         }
      }
      public static List<string> History(string path) {
         var result = new List<string>();
         if (File.Exists(path)) {
            using (var sr = new StreamReader(path)) {
               while (!sr.EndOfStream) {
                  string current_line = sr.ReadLine();
                  if (String.IsNullOrEmpty(current_line)) current_line = String.Empty;
                  if (current_line.Trim().StartsWith("<b>Bill History")) {
                     result.Add(current_line);
                     while (!sr.EndOfStream) {
                        current_line = sr.ReadLine();
                        result.Add(current_line);
                     }
                  }
               }
            }
         }
         return result;
      }

      public static string Position(string path) {
         const string strPosition = "<b>Position";
         var result = String.Empty;
         if (File.Exists(path)) {
            using (var sr = new StreamReader(path)) {
               while (!sr.EndOfStream) {
                  string current_line = sr.ReadLine();
                  if (String.IsNullOrEmpty(current_line)) current_line = String.Empty;
                  if (current_line.Trim().StartsWith(strPosition)) {
                     var format = $"{strPosition}</b>:(.*)";
                     result = Regex.Replace(current_line.Trim(), format, "$1");
                  }
               }
            }
         }
         return result;
      }
   }
}