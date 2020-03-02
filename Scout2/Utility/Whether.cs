using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms.VisualStyles;
using OpenQA.Selenium.Support.UI;

namespace Scout2.Utility {
   public class Whether {
      private static readonly List<string> dead_signatures = new List<string>(); // The strings that signify a dead bill
      private static Whether instance = null;
      private static readonly object access = new object();    // Lock this to prevent concurrent access
      /// <summary>
      /// // Populate signature list on initial construction
      /// </summary>
      private Whether() {
         dead_signatures.Add("This bill is dead");
         dead_signatures.Add("Died at Desk");
         dead_signatures.Add("Vetoed by Governor");
         dead_signatures.Add("pursuant to Joint Rule 56");
         dead_signatures.Add("Stricken from file");
      }

      // Call this to get access to the object
      public static Whether Instance {
         get {
            lock (access) {
               instance = instance ?? new Whether();
               return instance;
            }
         }
      }
      /// <summary>
      /// Determine if a string contains an IsDead signature
      /// </summary>
      /// <param name="str">string to be examind</param>
      /// <returns>bool whether string contains an IsDead signature</returns>
      // Call this to determine if a string is an IsDead signature
      private static bool IsDeadSignature(string str) {
         foreach (var signature in dead_signatures) { if (str.Contains(signature)) return true; }
         return false;
      }
      /// <summary>
      /// Determine if a List of string contains an IsDead signature
      /// </summary>
      /// <param name="input">List of strings which are expected to be a bill report.  They can actually be any strings.</param>
      /// <returns>bool whether any list member contains an IsDead signature</returns>
      public bool IsDeadSignature(List<string> input) {
         foreach (string line in input) { if (IsDeadSignature(line)) return true; }
         return false;
      }
   }
}