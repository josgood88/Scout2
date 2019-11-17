using System;
using System.Windows.Forms;
using Library;

namespace Scout2.Controllers {
   public class BaseController {

      protected static void LogThis(string message) {
         string output = $"{DateTime.Now.ToLocalTime()} {message}";
         Console.WriteLine(message);
         Log.Instance.Info(message);
      }

      public static void LogAndShow(string message) {
         LogThis(message);
         MessageBox.Show(message);
      }

      public static void LogAndThrow(string message) {
         LogThis(message);
         throw new ApplicationException(message);
      }
   }
}
