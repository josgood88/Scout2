using Library.Database;
using System.Collections.Generic;

namespace Library {
   public static class GlobalData {
      public static List<BillProfile> Profiles      { get; set; }
      public static List<BillRow>     BillRows      { get; set; }
      public static BillHistoryTable  HistoryTable  { get; set; }
      public static BillVersionTable  VersionTable  { get; set; }
      public static LocationCodeTable LocationTable { get; set; }
      public static List<Bill_Identifier> MostRecentEachBill { get; set; }
   }
}
