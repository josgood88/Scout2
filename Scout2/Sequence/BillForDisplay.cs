
namespace Scout2.Sequence {
   public class BillForDisplay {

      public string Measure { private set; get; }
      public string Position { private set; get; }
      public string BillLastAction { private set; get; }
      public string HistoryLastAction { private set; get; }
      public string Score { private set; get; }

      public BillForDisplay(string a, string b, string c, string d) {
         Measure = a; Position = b; BillLastAction = c; HistoryLastAction = d; Score = "0";
      }
      public BillForDisplay(string a, string b, string c, string d, int score_) {
         Measure = a; Position = b; BillLastAction = c; HistoryLastAction = d; Score = score_.ToString();
      }
   }
   public class NewBillForDisplay {

      public string Measure { private set; get; }
      public string Score { private set; get; }
      public string Title { private set; get; }

      public NewBillForDisplay(string a, string b, string c) {
         Measure = a; Score = b; Title = c;
      }
   }
}