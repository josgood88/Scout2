using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Library;
using Library.Database;

namespace Scout2.Sequence {
   /// <summary>
   /// Ranking is done by
   ///   Obtaining a list of key words, each of which has been assigned a score.
   ///      Some are negative scores indicating a bad bill, others are positive indicating a good bill.
   ///   For each bill
   ///      Read the bill's text
   ///      For each key word, compute a score based on the number of occurrences and the word's score
   ///      Sum the positive and negative scores into a positive and negative score for the bill
   ///      Record those positive and negative scores 
   ///   End for
   ///   Update the database with the recorded scores for the bills.
   /// </summary>
   public class RescoreBills : BaseController {
      public void Run(Form1 form1) {
         var start_time = DateTime.Now;
         LogAndDisplay(form1.txtUpdateScores, "Scoring bills.");

         // Determine which bills to process
         List<BillRow> process_these = BillRow.RowSet();
         LogAndDisplay(form1.txtUpdateScores, $"There are {GlobalData.Profiles.Count} bills to score.");

         if (process_these.Count > 0) {
            List<BillProfile> ranked = Process(form1, process_these);
            UpdateBillRowsFromBillProfile(ranked);
         }
         try {
         } catch (Exception ex) {
            LogAndThrow($"CreateNewReports.Run: {ex.Message}.");
         }
         var elapsed = DateTime.Now - start_time;
         if (process_these.Count > 0) {
            LogAndDisplay(form1.txtUpdateScores, $"Through with bill scoring. {elapsed.ToString("c")} ");
         }
      }
      /// <summary>
      /// Process each bill in the passed collection, computing each bill's scores.
      /// The scores in the BillRow associated with each bill are updated so that the caller may use them.
      /// </summary>
      /// <param name="form1">The display form containing the progress bar and text block used to communicate progress.</param>
      /// <param name="process_these">The list of BillRows to the processed</param>
      private List<BillProfile> Process(Form1 form1, List<BillRow> process_these) {
         // Obtain the negative and postive key words
         List<PartialScore> negative_words = PartialScore.ImportKeywordScores(Config.Instance.NegativeFile);
         List<PartialScore> positive_words = PartialScore.ImportKeywordScores(Config.Instance.PositiveFile);
         List<BillProfile> ranked = new List<BillProfile>();

         int count_bills_processed = 0;
         double next_benchmark = .0;      // Update progress bar every 1%
         form1.ProgressScore.Maximum = 100;
         foreach (var bill in process_these) {
            ranked.Add(Rank(bill, negative_words, positive_words));
            HandleProgressBar(form1, ref count_bills_processed, ref next_benchmark, process_these.Count);
            form1.txtUpdateScores.Text = $"{bill.NegativeScore}, {bill.PositiveScore}, {bill.Bill}";
            form1.txtUpdateScores.Update();
         }
         count_bills_processed = process_these.Count;
         HandleProgressBar(form1, ref count_bills_processed, ref next_benchmark, process_these.Count);
         return ranked;
      }
      /// <summary>(0
      /// Score a single bill.
      /// This method reads the bill text and computes the positive and negative scores for terms defined
      /// in the configuration data.
      /// </summary>
      /// <param name="bill">The BillRow describing the bill to be processed</param>
      /// <param name="negative_words">List of key words with negative values</param>
      /// <param name="positive_words">List of key words with positive values</param>
      private BillProfile Rank(BillRow bill, List<PartialScore> negative_words, List<PartialScore> positive_words) {
         var bill_identifier = new Bill_Identifier(bill.Lob);
         var raw_profile = new BillProfile(bill_identifier, negative_words, positive_words);
         var ranked_profile = CountKeywords(raw_profile);
         return ranked_profile;
      }

      // BillProfile contains a 2 lists of PartialScore.  A PartialScore gives a bill's score for a single key word.
      // This method counts the words in the bill's text that match the keyword in each PartialScore;
      private BillProfile CountKeywords(BillProfile profile) {
         var result = profile;
         result.Negative = CountKeywords(profile, profile.Negative);
         result.Positive = CountKeywords(profile, profile.Positive);
         result.NegScore = Sum(result.Negative);
         result.PosScore = Sum(result.Positive);
         if (result.NegScore > 0)
            Log.Instance.Info($"{result.NegScore}");
         return result;
      }

      private List<PartialScore> CountKeywords(BillProfile profile, List<PartialScore> partials) {
         var result = partials;
         foreach (var item in partials) {
            var searchTerm = new Regex(item.RegexForm);
            var matches = searchTerm.Matches(profile.Identifier.Contents);
            item.Count = matches.Count;
            if (item.Count > 0)
               Log.Instance.Info($"{item.Count}");
         }
         return result;
      }

      // This method sums the individual List<PartialScore> scores.
      private int Sum(List<PartialScore> partials) {
         var result = 0;
         foreach (var item in partials) { result += item.Count * item.Worth; }
         return result;
      }
      void HandleProgressBar(Form1 form1, ref int count_bills_processed, ref double next_benchmark, int collection_size) {
         var percent = Convert.ToDouble(++count_bills_processed) / collection_size;
         if (percent >= next_benchmark) {
            next_benchmark += 0.01;
            if (form1.ProgressScore.InvokeRequired) {
               throw new ApplicationException("RescoreBills.Process: Being run in background. Invoke required.");
            } else {
               form1.ProgressScore.Value = Convert.ToInt32(percent * 100);
               form1.ProgressScore.Update();
               form1.ProgressScore.Refresh();
               form1.ProgressScore.Invalidate();
            }
         }
      }
   }
}