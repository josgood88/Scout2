using System.Runtime.CompilerServices;
// Allows Testing to test ReplaceUnicodeSubString
[assembly: InternalsVisibleTo("Scout.Tests")]

namespace Scout2.Utility {
   public class MiscUtils {
      public static bool IsEven(int value) { return value % 2 == 0; }
      public static bool IsOdd (int value) { return !IsEven(value); }
   }
}