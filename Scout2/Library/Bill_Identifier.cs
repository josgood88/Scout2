using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Library {
   /// The public properties of this class uniquely identify each version of each bill within a biennium.
   public class Bill_Identifier {
      public string Author { get; private set; }
      public string Title { get; private set; }
      public string LobPath { get; }
	   public string Contents { get; }
      public string Type { get; private set; }
      public string Number { get; private set; }
      public string BillID { get; private set; }
      public string Revision { get; private set; }

      public Bill_Identifier(string path) { 
         if (File.Exists(path)) {
            LobPath = path;
			   Contents = File.ReadAllText(path);
            if (Contents.Contains("&lt;")) {  // < isn't always done correctly
               var re_lt = "&lt;";
               Contents = Regex.Replace(Contents,re_lt,"<").ToString();
            }
            ExtractID(Contents);
            ExtractAuthor(Contents);
            ExtractTitle(Contents);
         } else {
            throw new ArgumentException($"{path} does not exist.");
         }
      }

      private string SubStringInterest(string contents, string prefix, string suffix) {
         var start = contents.IndexOf(prefix)+prefix.Length;
         var end = contents.IndexOf(suffix,start);
         if (start == -1 || end == -1) return string.Empty;
         var of_interest = contents.Substring(start,end-start);
         return of_interest;
      }

      private void ExtractID(string contents) {
         const string prefix = "<caml:Id>";           // Precedes substring of interest
         const string suffix = "</caml:Id>";          // Follows substring of interest
         const string num   = @"\d{4}";               // Bill number is 4 digits
         const string rev   = @"\d{2}";               // Revision is 2 digits
         const string type  = @"[^_]*";               // Bill type is terminated by underscore
         const string under = @"_*?";                 // Underscores
         const string year  = @"\d{4}";               // Year is 4 digits

         // Isolate the substring of interest, e.g. "20190AB__100099INT"
         var of_interest = SubStringInterest(contents,prefix,suffix);
         
         // Extract the three fields we want
         var re_type = $"{year}.({type}){under}{num}{rev}.*";
         var re_num  = $"{year}.{type}{under}({num}){rev}.*";
         var re_rev  = $"{year}.{type}{under}{num}({rev}).*";
         Type = Regex.Replace(of_interest,re_type,"$1").ToString();
         Number = Regex.Replace(of_interest,re_num,"$1").ToString();
         Revision = Regex.Replace(of_interest,re_rev,"$1").ToString();
         BillID = Type+Number;
      }

      private void ExtractAuthor(string contents) {
         const string prefix = "<caml:Name>";         // Precedes substring of interest
         const string suffix = "</caml:Name>";        // Follows substring of interest
         Author = SubStringInterest(contents,prefix,suffix);
         Author = Regex.Replace(Author,"'","''");     // SQL embedded apostrophe is two consecutive apostrophes
      }

      private void ExtractTitle(string contents) {
         const string prefix = "<caml:Subject>";      // Precedes substring of interest
         const string suffix = "</caml:Subject>";     // Follows substring of interest
         Title = SubStringInterest(contents,prefix,suffix);
         const string deletion = "xm-deletion_mark";
         if (Title.Contains(deletion)) {
            const string full_deletion = @"\<\?xm-deletion_mark data=.*?\?>";
            Title = Regex.Replace(Title,full_deletion,"").ToString();
         }
         const string insertion = "xm-insertion_mark";
         if (Title.Contains(insertion)) {
            const string full_insertion = @"\<\?xm-insertion_mark_start\?>(.*?)\<\?xm-insertion_mark_end\?>";
            Title = Regex.Replace(Title,full_insertion,"$1").ToString();
         }
      }
   }
}
