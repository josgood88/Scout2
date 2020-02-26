namespace Scout2.Utility {
   public class CommonUtils {
      public static bool IsNotNullOrEmpty(string str) {
         return !string.IsNullOrEmpty(str);
      }
      public static bool IsNotNullOrWhiteSpace(string str) {
         return !string.IsNullOrWhiteSpace(str);
      }
      public static bool IsNullOrEmptyOrWhiteSpace(string str) {
         return string.IsNullOrEmpty(str) || string.IsNullOrWhiteSpace(str);
      }
      public static bool IsNotNullOrEmptyOrWhiteSpace(string str) {
         return IsNotNullOrEmpty(str) && IsNotNullOrWhiteSpace(str);
      }
   }
}