using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure.Design;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Scout2.Sequence;

namespace Scout2.IndividualReport {
   public class PreviousReport {
      const string signal_comm  = "<b>Committees";
      const string signal_like  = "<b>Likelihood";
      const string signal_posit = "<b>Position";
      const string signal_short = "<b>ShortSummary";
      const string signal_stat  = "<b>Status";
      const string signal_summ  = "<b>Summary";

      static readonly List<string> signal_end_of_section = new List<string> {
         signal_comm, signal_like, signal_posit, signal_short, signal_summ
      };
      /// <summary>
      /// Extract several fields from the file on the path.  That file is expected to be an individual bill report.
      /// </summary>
      /// <param name="path">Path to the individual report file being examined</param>
      /// <param name="summary">Receives the Summary section</param>
      /// <param name="position">Receives the position being taken on the bill</param>
      /// <param name="short_summary">Receives the one-line summary of the bill</param>
      /// <param name="committees">Receives the predicted path through legislature committees</param>
      /// <param name="likelihood">Receives our evaluation of the likelihood of bill passage</param>
      public static void From(string path, List<string> summary, List<string> position,
               out string short_summary, out string committees, out string likelihood) {
         // Fetch multi-line fields from the file on the path.
         // Clear unwanted <p> and </p> from the end of the lists
         summary = RemoveTrailingParagraphs(FetchListFields(path, signal_summ, signal_end_of_section));
         position = RemoveTrailingParagraphs(FetchListFields(path, signal_posit, signal_end_of_section));
         // Fetch single-line items from the file on the path.
         short_summary = FetchField(path, signal_short);
         committees = FetchField(path, signal_comm);
         likelihood = FetchField(path, signal_like);
      }

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
                        result.Add(current_line);
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

      private static List<string> RemoveTrailingParagraphs(List<string> items) {
         var result = items;
         while (result.Last().Contains("<p>") || result.Last().Contains("</p>")) {
            result.RemoveAt(result.Count - 1);
         }
         return result;
      }
      /// <summary>
      /// Obtains the predicted committee path described in an individual bill report
      /// </summary>
      /// <param name="path">Path to the individual report file being examined</param>
      /// <returns></returns>
      public static string Prediction(string path) { return FetchField(path, signal_comm); }
      /// <summary>
      /// Obtains the passage likelihood described in an individual bill report
      /// </summary>
      /// <param name="path">Path to the individual report file being examined</param>
      /// <returns></returns>
      public static string Likelihood(string path) { return FetchField(path, signal_like); }
   }
}
