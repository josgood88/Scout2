using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Scout2.Sequence;

namespace Scout2.IndividualReport {
   public class PreviousReport : Scout2.IndividualReport.IndividualReportCommon {
      const string signal_comm  = "<b>Committees";
      const string signal_like  = "<b>Likelihood";
      const string signal_posit = "<b>Position";
      const string signal_short = "<b>ShortSummary";
      const string signal_stat  = "<b>Status";
      const string signal_summ  = "<b>Summary";

      static readonly List<string> signal_end_of_section = new List<string> {
         signal_comm, signal_like, signal_posit, signal_short, signal_stat, signal_summ
      };
      /// <summary>
      /// Obtains the (potentially multi-line) Summary described in an individual bill report
      /// Summary is terminated by Position.  There may be intervening lines between the actual end
      /// of the Summary and the Position marker.  Delete those lines and ensure that the summary
      /// is contained by <p></p>
      /// </summary>
      /// <param name="path">Path to the individual report file being examined</param>
      /// <returns></returns>
      public static List<string> Summary(string path) {
         var result = FetchListFields(path, signal_summ, signal_end_of_section);
         while (result.Last().Contains("/html") || result.Last().Contains("<p>") || result.Last().Contains("</p")) {
            result.RemoveAt(result.Count-1);
         }
         result.Insert(0, "<p>");
         result.Add("</p>");
         return result;
      }
      /// <summary>
      /// Obtains the (potentially multi-line) Position described in an individual bill report
      /// Position is terminated by ShortSummary.  There may be intervening lines between the actual end
      /// of the Position and the ShortSummary marker.  Delete those lines and ensure that the position
      /// is contained by <p></p>
      /// </summary>
      /// <param name="path">Path to the individual report file being examined</param>
      /// <returns></returns>
      /// TODO Hides IndividualReportCommon.Position.  What is desired?
      public static List<string> Position(string path) {
         var result = FetchListFields(path, signal_posit, signal_end_of_section);
         while (result.Last().Contains("/html") || result.Last().Contains("<p>") || result.Last().Contains("</p")) {
            result.RemoveAt(result.Count-1);
         }
         result.Insert(0, "<p>");
         result.Add("</p>");
         return result;
      }
      /// <summary>
      /// Obtains the ShortSummary described in an individual bill report
      /// </summary>
      /// <param name="path">Path to the individual report file being examined</param>
      /// <returns></returns>
      public static string ShortSummary(string path) { return FetchField(path, signal_short); }
      /// <summary>
      /// Obtains the predicted committee path described in an individual bill report
      /// </summary>
      /// <param name="path">Path to the individual report file being examined</param>
      /// <returns></returns>
      public static string Committees(string path) { return FetchField(path, signal_comm); }
      /// <summary>
      /// Obtains the passage likelihood described in an individual bill report
      /// </summary>
      /// <param name="path">Path to the individual report file being examined</param>
      /// <returns></returns>
      public static string Likelihood(string path) { return FetchField(path, signal_like); }

      /// TODO Hides IndividualReportCommon.History.  What is desired?
      public static List<string> History(string path) {
         var result = new List<string>();
         if (File.Exists(path)) {
            using (var sr = new StreamReader(path)) {
               while (!sr.EndOfStream) {
                  string current_line = sr.ReadLine();
                  if (string.IsNullOrEmpty(current_line)) continue;
                  if (current_line.Trim().Contains("<b>Bill History")) {
                     result.Add(current_line);
                     while (!sr.EndOfStream) {
                        current_line = sr.ReadLine();
                        if (current_line != null) {
                           current_line = current_line.Replace("''", "'"); // author''s becomes author's
                           result.Add(current_line);
                        }
                     }
                  }
               }
            }
         }
         return result;
      }
      /// <summary>
      /// Report sections such as Summary and Status contain multiple lines.
      /// </summary>
      /// <param name="path">Path to the individual report file being examined</param>
      /// <param name="signature">Signature of the section of interest</param>
      /// <param name="terminators">Any of these strings terminate the section</param>
      /// <returns></returns>
      private static List<string> FetchListFields(string path, string signature, List<string> terminators) {
         List<string> result = new List<string>();
         var sr = new StreamReader(path);
         try {
            // Recognize the line containing the signature and, if non-null and not empty, add it to the result.
            var signature_line = FindSignature(ref sr, signature);
            if (!string.IsNullOrEmpty(signature_line)) {
               result.Add(signature_line);
               // Add the body of the section, ending when a section terminator is recognized.
               while (!sr.EndOfStream) {
                  var current_line = sr.ReadLine();
                  if (string.IsNullOrEmpty(current_line)) continue;
                  var end_of_section = terminators.FirstOrDefault(t => current_line.Contains(t));
                  if (end_of_section == null) result.Add(current_line);
                  else break; // Exit when section terminator found
               }
            }
         } catch (ApplicationException ex) {
            BaseController.LogAndShow(ex.Message);
         } finally {
            sr.Close();
         }
         return result;
      }
      /// <summary>
      /// Report fields such as ShortSummary, Committees and Likelihood each exist on a single line beginning with
      /// a signature (such as "ShortSummary:") followed by the field contents.
      /// </summary>
      /// <param name="path">Path to the individual report file being examined</param>
      /// <param name="signature">Signature of the section of interest</param>
      /// <returns></returns>
      private static string FetchField(string path, string signature) {
         var result = string.Empty;
         if (File.Exists(path)) {
            using (var sr = new StreamReader(path)) {
               while (!sr.EndOfStream) {
                  string current_line = sr.ReadLine();
                  if (string.IsNullOrEmpty(current_line)) continue;
                  if (current_line.Trim().Contains(signature)) {
                     var format = $"{signature}</b>:(.*)";
                     result = Regex.Replace(current_line.Trim(), format, "$1");
                     break;
                  }
               }
            }
         }
         // There can be leading space-separated "<br />" in the result;
         return result.Replace("<br />", string.Empty).Trim();
      }
      /// <summary>
      /// Position a StreamReader on a stream of lines to the line containing a signature.
      /// Throws if signature not found.
      /// Otherwise returns the line containing the signature.
      /// </summary>
      /// <param name="sr"></param>
      /// <param name="signature"></param>
      private static string FindSignature(ref StreamReader sr, string signature) {
         while (!sr.EndOfStream) {
            string current_line = sr.ReadLine();
            if (string.IsNullOrEmpty(current_line)) continue;
            if (current_line.Trim().Contains(signature)) {
               return current_line;
            }
         }
         throw new ApplicationException($"Stream does not contain signature {signature}.");
      }
   }
}
