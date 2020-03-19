using System.Collections.Generic;

namespace Scout2 {
   /// <summary>
   /// When a committee holds a hearing on a bill, it does so on some scheduled date.
   /// </summary>
   public class Hearing {
      public string measure { get; set; }
      public string date { get; set; }
   }
   /// <summary>
   /// A committee has an agenda of bills that it intends to review in a public hearing.
   ///  </summary>
   public class CommitteeAgenda {
      public string committee { get; set; }
      public string url { get; set; }
      public List<Hearing> hearings { get; private set; }
   }
}