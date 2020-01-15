
using Scout2.IndividualReport;
namespace Scout2.Sequence {
   public class ChangedBillForDisplay {
      public string Measure { protected set; get; }
      public string Position { protected set; get; }
      public string BillLastAction { protected set; get; }
      public string HistoryLastAction { protected set; get; }
      public string Score { protected set; get; }
      public ChangedBillForDisplay(string a, string b, string c, string d) {
         Measure = a; Position = b; BillLastAction = c; HistoryLastAction = d; Score = "0";
      }
      public ChangedBillForDisplay(string a, string b, string c, string d, int score_) {
         Measure = a; Position = b; BillLastAction = c; HistoryLastAction = d; Score = score_.ToString();
      }
   }

   public class UnreportedBillForDisplay {
      public string Measure { protected set; get; }
      public string NegativeScore { protected set; get; }
      public string Title { protected set; get; }
      public string Author { protected set; get; }
      public UnreportedBillForDisplay(string a, string b, string c, string d) {
         Measure = a; NegativeScore = Ensure4DigitNumber(b); Title = c; Author = d;
      }

      private string Ensure4DigitNumber(string number) {
         while (number.Length < 4) number = $"0{number}";
         return number;
      }
   }
}