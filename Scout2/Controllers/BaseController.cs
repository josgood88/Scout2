using System;
using Library;

namespace Scout2.Controllers {
   public class BaseController {

      protected void LogThis(string message) {
         string output = $"{DateTime.Now.ToLocalTime()} {message}";
         Console.WriteLine(message);
         Log.Instance.Info(message);
      }
   }
}
