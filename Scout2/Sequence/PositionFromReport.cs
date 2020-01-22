using System.Text.RegularExpressions;
using Scout2.Utility;

namespace Scout2.Sequence {
   public struct PositionFromReport {
      public string Measure { get; set; }
      public string MeasureNoDash { get; set; }
      public string Position { get; set; }
      public PositionFromReport(string a, string b) {
         Position=b;
         if (a.Contains("-")) {
            Measure=a;
            MeasureNoDash = Regex.Replace(Measure, "-", string.Empty);
         } else {
            MeasureNoDash = a;
            BillUtils.ExtractHouseNumber(MeasureNoDash, out string house, out string number);
            Measure = $"{house}-{number}";
         }
      }
   }
}
