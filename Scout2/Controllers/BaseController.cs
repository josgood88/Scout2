using System;
using System.Windows.Forms;
using Library;

namespace Scout2.Controllers {
   public class BaseController {

      protected void LogThis(string message) {
         string output = $"{DateTime.Now.ToLocalTime()} {message}";
         Console.WriteLine(message);
         Log.Instance.Info(message);
      }

      protected void LogAndShow(string message) {
         LogThis(message);
         MessageBox.Show(message);
      }

      protected void LogAndThrow(string message) {
         LogThis(message);
         throw new ApplicationException(message);
      }
   }
}
