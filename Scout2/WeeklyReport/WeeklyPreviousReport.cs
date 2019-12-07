using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Scout2.WeeklyReport {
   public class PreviousReport {

      public static void From(string path, List<string> summary, List<string> position) {
         if (File.Exists(path)) {
            string current_line = string.Empty;
            using (var sr = new StreamReader(path)) {
               while (!sr.EndOfStream) {
                  current_line = sr.ReadLine();
                  if (current_line.Trim().StartsWith("<b>Summary")) {
                     summary.Add(current_line);
                     bool exit1 = false;
                     while (!sr.EndOfStream && !exit1) {
                        current_line = sr.ReadLine();
                        if (current_line.Trim().StartsWith("<b>Position")) exit1 = true;
                        else summary.Add(current_line);
                     }
                     // Position immediately follows Summary
                     if (current_line.Trim().StartsWith("<b>Position")) {
                        position.Add(current_line);
                        bool exit2 = false;
                        while (!sr.EndOfStream && !exit2) {
                           current_line = sr.ReadLine();
                           if (current_line.Trim().StartsWith("<b>Status")) exit2 = true;
                           else position.Add(current_line);
                        }
                     }
                  }
               }
            }
         }
      }

      public static List<string> History(string path) {
         var result = new List<string>();
         if (File.Exists(path)) {
            string current_line = string.Empty;
            using (var sr = new StreamReader(path)) {
               while (!sr.EndOfStream) {
                  current_line = sr.ReadLine();
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
         var result = string.Empty;
         if (File.Exists(path)) {
            string current_line = string.Empty;
            using (var sr = new StreamReader(path)) {
               while (!sr.EndOfStream) {
                  current_line = sr.ReadLine();
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
