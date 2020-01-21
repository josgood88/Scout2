using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text.RegularExpressions;
namespace Library {
   public class BillRanker {
      /// <summary>
      /// Compute the positive and negative scores for the passed bill.
      /// </summary>
      /// <param name="profiles"></param>
      public static void Compute(ref List<BillProfile> profiles) {
         foreach (var profile in profiles) {
            IEnumerable<string> words = ReduceContentsToWords(profile.Identifier.Contents);
            profile.NegScore = Compute(words, profile.Negative);
            profile.PosScore = Compute(words, profile.Positive);
         }
      }

      private static IEnumerable<string> ReduceContentsToWords(string contents) {
         string free_of_html = RemoveHTML(contents);
         string[] words = free_of_html.Split(' ');
         return words;
      }


      // Remove HTML from a string containing the text of a bill
      private static string RemoveHTML(string contents) {
         // Remove standard bill prefix, including Legislative Counsel's digest
         const string exp = "The people of the State of California do enact as follows:";
         var split_here = contents.IndexOf(exp);
         string s0 = contents.Substring(split_here);

         // Remove HTML
         string s1 = Regex.Replace(s0, "</?caml(.*?)>",          string.Empty);
         string s2 = Regex.Replace(s1, "<em>(.*?)</em>",         " $1 ");
         string s3 = Regex.Replace(s2, "<strike>(.*?)</strike>", string.Empty);
         string s4 = Regex.Replace(s3, "<p>(.*?)</p>",           " $1 ");
         string s5 = Regex.Replace(s4, "<span class=.EnSpace./>",string.Empty);        // . instead of ""

         // Remove everything less than space.
         var allowedChars = Enumerable.Range(' ', 0x5f);   // Allow any ASCII greater equal space
         var goodChars = s5.Where(c => allowedChars.Contains(c));
         return new string(goodChars.ToArray());
      }

      private static int Compute(IEnumerable<string> words, List<PartialScore> keyword_scores) {
         int result = 0;
         foreach (var term in keyword_scores) {
            int count = words.Where(x => x == term.Keyword).Count();
            result += term.Worth * count;
         }
         return result;
      }
   }
}