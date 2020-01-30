using System;

namespace RequirementsTraceability {
   public class Log {
      
      public static void Instance(string msg) { Console.WriteLine($"{DateTime.Now.ToLocalTime()}:{msg}"); }
   }
}