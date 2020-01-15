using System.Text.RegularExpressions;
using Scout2.IndividualReport;

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
            CreateIndividualReport.ExtractHouseNumber(MeasureNoDash, out string house, out string number);
            Measure = $"{house}-{number}";
         }
      }
   }
}
