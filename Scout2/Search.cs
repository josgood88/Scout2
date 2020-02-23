using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Library;
using Library.Database;
using Scout2.Utility;

namespace Scout2 {
   public partial class Form1 : Form {
      struct bgwSearchArguments {
         public bgwSearchArguments(string a, string b, string c, string d) { word1=a; word2=b; text_min=c; text_max=d; }
         public string word1 { get; set; }
         public string word2 { get; set; }
         public string text_min { get; set; }
         public string text_max { get; set; }
      }

      /// <summary>
      /// This background worker performs the time-consuming task of searching all current bills for two words
      /// that are within a specified distance of each other.
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void bgw_SearchNearby(object sender, DoWorkEventArgs e) {
         BackgroundWorker worker = sender as BackgroundWorker;
         if (worker == null) throw new ApplicationException("bgw_SearchNearby failed to instantiate BackgroundWorker");

         // Ensure passed strings are valid.  If so take the minimum and maximum distance values.
         string param_problem = "";
         bgwSearchArguments args = (bgwSearchArguments)e.Argument;
         if (NEWs(args.word1)) BuildErrorMessage(ref param_problem, "First word must be specified");
         if (NEWs(args.word2)) BuildErrorMessage(ref param_problem, "Second word must be specified");
         if (NEWs(args.text_min)) BuildErrorMessage(ref param_problem, "Minimum size must be specified");
         if (NEWs(args.text_max)) BuildErrorMessage(ref param_problem, "Maximum must be specified");
         args.word1 = args.word1.Trim(); args.word2 = args.word2.Trim();
         args.text_min = args.text_min.Trim(); args.text_max = args.text_max.Trim();
         if (!Int16.TryParse(args.text_min, out short min_dist)) BuildErrorMessage(ref param_problem, "Unable to parse Minimum distance");
         if (min_dist < 0) BuildErrorMessage(ref param_problem, "Minimum distance must not be negative");
         if (!Int16.TryParse(args.text_max, out short max_dist)) BuildErrorMessage(ref param_problem, "Unable to parse Maximum distance");
         if (max_dist < 0) BuildErrorMessage(ref param_problem, "Maximum distance must not be negative");
         if (param_problem.Length > 0) throw new ApplicationException(param_problem);

         GlobalData.BillRows = BillRow.RowSet();
         var bills = GlobalData.BillRows.OrderBy(item => item.Bill).ToList();
         int one_percent = bills.Count / 100;

         Regex rx = CreateRegex(args.word1, args.word2, min_dist, max_dist);
         int progress_bar_value = 0;
         using (StreamWriter sw_matches = new StreamWriter("C:/Scratch/Scout2_Matches.txt")) {
            using (StreamWriter sw_bills = new StreamWriter("C:/Scratch/Scout2_Bills.txt")) {
               int count = 0;
               foreach (var bill in bills) {
                  MatchCollection found = SingleFile(bill, rx);
                  if (found.Count > 0) {
                     sw_bills  .WriteLine($"{bill.Bill}");
                     sw_matches.WriteLine($"{bill.Bill} {bill.Title} ({bill.Author})");
                     foreach (var match in found) sw_matches.WriteLine($"\t{Cleanup(match.ToString())}");
                  }
                  if (++count % one_percent == 0) worker.ReportProgress(++progress_bar_value);
               }
            }
         }
      }

      // This event handler updates the progress.
      private void bgw_TwoWordsNear_ProgressChanged(object sender, ProgressChangedEventArgs e) {
         progressTwoWordsNear.Invoke(new Action(() => progressTwoWordsNear.Value = e.ProgressPercentage));
      }

      // This event handler deals with the results of the background operation.
      private void bgw_TwoWordsNear_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
         // Prevent events from firing twice.
         bgw_TwoWordsNear.DoWork -= bgw_SearchNearby;
         bgw_TwoWordsNear.ProgressChanged -= bgw_TwoWordsNear_ProgressChanged;
         bgw_TwoWordsNear.RunWorkerCompleted -= bgw_TwoWordsNear_RunWorkerCompleted;

         var message = "Successful completion";
         if (e.Cancelled == true) message = "Canceled";
         else if (e.Error != null) message = $"Error: {e.Error.Message}";
         MessageBox.Show(message);
      }

      private static Regex CreateRegex(string word1, string word2, int min, int max) {
         //  \bword1\W+(?:\w+\W+){1,6}?word2\b 
         string wb = @"\b";               // Word_Boundary
         string w1 = $"{word1}";          // Word 1
         string w2 = $"{word2}";          // Word 1
         string mm = $"{min},{max}";      // Min, Max
         string exp = @wb + w1 + @"\W+(?:\w+\W+){" + $"{min},{max}" + "}?" + w2 + wb;
         return new Regex(exp);
      }

      private static MatchCollection SingleFile(BillRow row, Regex rx) {
         var result = string.Empty;
         var contents = ContentsWithHTML(row);
         var trimmed = RemoveHTML(contents);
         //File.WriteAllText("C:/Scratch/temp.txt", contents);
         return rx.Matches(trimmed);
      }

      /// <summary>
      /// This method reads a raw bill file, trims various introductory information, and returns the actual bill text.
      /// It does not remove HTML control information embedded in that text.
      /// </summary>
      /// <param name="row"></param>
      /// <returns></returns>
      private static string ContentsWithHTML(BillRow row) {
         var contents = string.Empty;
         if (File.Exists(row.Lob)) {
            contents = FileUtils.FileContents(row.Lob);
            var end_marker = "</caml:Preamble>";
            var offset = contents.IndexOf(end_marker) + end_marker.Length;
            contents = contents.Substring(offset);
         }
         return contents;
      }

      private static string RemoveHTML(string contents) {
         var no_angle_brackets = Regex.Replace(contents, "<.*?>", "");
         return no_angle_brackets;
      }

      private static string Cleanup(string text) {
         return Regex.Replace(text, "\t", "");
      }

      /// <summary>
      /// "NearBy" does some error checking and accumulates all error messages into a single string.
      /// This method appends a new error message to that string.
      /// </summary>
      /// <param name="accumulate_here"></param>
      /// <param name="new_message"></param>
      private static void BuildErrorMessage(ref string accumulate_here, string new_message) {
         if (accumulate_here.Length > 0) accumulate_here += ", ";
         accumulate_here += new_message;
      }

      private static bool NEWs(string s) {
         return string.IsNullOrEmpty(s) || string.IsNullOrWhiteSpace(s);
      }
   }
}
