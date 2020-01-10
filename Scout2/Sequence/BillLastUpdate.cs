namespace Scout2.Sequence {
   /// <summary>
   /// Used to capture the "Last Action Date" shown in a bill report
   /// </summary>
   public struct BillLastUpdate {
      public BillLastUpdate(string a, string b) { bill=a; last_updated=b; }
      public string bill { set; get; }
      public string last_updated { set; get; }
   }
}
