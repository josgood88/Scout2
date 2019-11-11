using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml;

namespace Library {
   public class PartialScore {
      public string Keyword { get; private set; }
      public string RegexForm { get; private set; }
      public int Worth { get; private set; }
      public int Count { get; set; }

      public static List<PartialScore> ImportKeywordScores(string filepath) {
         var result = new List<PartialScore>();
         const string not_semicolon = "[^;]*";
         string re_keyword    = $"({not_semicolon}).*";                    // Key word
         string re_regex_form = $"{not_semicolon};({not_semicolon}).*";    // Keyword in regex form
         string re_worth      = $"{not_semicolon};{not_semicolon};(.*)";   // How much the keyword is worth
         using (XmlReader reader = XmlReader.Create(filepath)) {
            while (reader.Read()) {
               if (reader.IsStartElement()) {
                  switch (reader.Name) {
                     case "Regex_Score": break;          // This name is not used
                     case "Pair":
                        if (reader.Read()) {
                           var terms = reader.Value.Replace("\n","").Trim();
                           var partial = new PartialScore();
                           partial.Keyword = Regex.Replace(terms,re_keyword,"$1");
                           partial.RegexForm   = Regex.Replace(terms,re_regex_form,"$1");
                           int worth;
                           if (Int32.TryParse(Regex.Replace(terms,re_worth,"$1"),out worth)) partial.Worth = worth;
                           result.Add(partial);
                        }
                        break;
                  }
               }
            }
         }
         return result;
      }
   }
}
