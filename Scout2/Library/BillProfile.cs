using System;
using System.Collections.Generic;
using System.Text;

namespace Library {
   public class BillProfile {
      public Bill_Identifier Identifier { get; }
      public List<PartialScore> Negative { get; set; }
      public List<PartialScore> Positive { get; set; }
      public int NegScore { get; set; }
      public int PosScore { get; set; }

      public BillProfile(string path, List<PartialScore> neg_keywords,List<PartialScore> pos_keywords) { 
         Identifier = new Bill_Identifier(path);
         Negative = neg_keywords;
         Positive = pos_keywords;
      }

      public BillProfile(Bill_Identifier bill_Identifier,List<PartialScore> neg_keywords,List<PartialScore> pos_keywords) {
         Identifier = bill_Identifier;
         Negative = neg_keywords;
         Positive = pos_keywords;
      }
   }
}
