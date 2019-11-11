using log4net;
using System;

namespace Library {
   public class Log {
      private static readonly log4net.ILog instance = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

      private Log() { }

      public static log4net.ILog Instance {
         get { return instance; }
      }
   }
}
