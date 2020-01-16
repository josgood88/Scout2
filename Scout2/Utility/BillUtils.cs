using System.Text.RegularExpressions;
namespace Scout2.Utility {
   public class BillUtils {
      /// <summary>
      /// Remove any dashes from the passed string.
      /// </summary>
      /// <param name="str"></param>
      /// <returns></returns>
      public static string NoDash(string str) {
         return Regex.Replace(str, "-", string.Empty);
      }
      /// <summary>
      /// Ensure the Measure/BillID has a 4-digt number, e.g. AB0123
      /// </summary>
      /// <param name="bill"></param>
      /// <returns></returns>
      public static string Ensure4DigitNumber(string bill) {
         ExtractHouseNumber(bill, out string house, out string number);
         while (number.Length < 4) number = $"0{number}";
         return $"{house}{number}";
      }
      // Extract house and number from bill id, returning house and number through argument references
      private static bool ExtractHouseNumber(string bill, out string house, out string number) {
         house = number = string.Empty;
         house  = Regex.Match(bill, @"\D*").Value;
         number = bill.Substring(house.Length);
         bool correct = house != string.Empty && house != bill && number != string.Empty && number != bill;
         return correct;
      }
   }
}