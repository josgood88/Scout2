using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Library {
   /// This class obtains a list of the current versions of Senate and House bills.
   public class MostRecentBills {

      public static List<Bill_Identifier> Identify(string folder_path) {
         var result = new List<Bill_Identifier>();
         var folder = Path.GetDirectoryName(folder_path);      // Ensure path terminates at folder (no filename.ext)
         result = FindMostRecentBillVersions(folder).ToList<Bill_Identifier>();// All .lob files in the folder
         return result;
      }

      public static List<BillProfile> Profile(string folder_path) {
         var result = new List<BillProfile>();
         return result;
      }

      static IEnumerable<Bill_Identifier> FindMostRecentBillVersions(string folder) {
         List<Bill_Identifier> all_bills = new List<Bill_Identifier>();
         Log.Instance.Info("FindMostRecentBillVersions: Listing all files in {folder}.");
         string[] filePaths = Directory.GetFiles(folder,"*.lob",SearchOption.TopDirectoryOnly);
         Log.Instance.Info("...Creating List<BillIdentifier>, one entry for each file.");
         foreach (var path in filePaths) all_bills.Add(new Bill_Identifier(path));

         Log.Instance.Info("...Selecting assembly bill identifiers.");
         var assembly_all_bills = SelectType(all_bills,"AB");
         Log.Instance.Info("...Selecting senate bill identifiers.");
         var senate_all_bills = SelectType(all_bills,"SB");

         var assembly_recent_bills = MostRecent(assembly_all_bills,"AB");
         Log.Instance.Info($"...There are {assembly_recent_bills.Count} distinct Assembly bills.");
         var senate_recent_bills = MostRecent(senate_all_bills,"SB");
         Log.Instance.Info($"...There are {senate_recent_bills.Count} distinct Senate bills.");

         return assembly_recent_bills.Concat(senate_recent_bills);
      }

      /// Obtain all bills of a type (AB, SB)
      static List<Bill_Identifier> SelectType(List<Bill_Identifier> all_bills, string type) {
         List<Bill_Identifier> result = new List<Bill_Identifier>();
         foreach(var item in all_bills) if (item.Type == type) result.Add(item);
         return result;
      }

      /// Obtain the most recent version of every bill
      static List<Bill_Identifier> MostRecent(List<Bill_Identifier> all_bills, string type) {
         List<Bill_Identifier> result = new List<Bill_Identifier>();
         var highest = HighestBillNumber(all_bills);
         // Count from lowest bill number to highest bill number.  
         // The legislature is not constrained to utilize every number in the range.  And they don't.
         for (int i = 1; i <= highest; i++) {
            var str_num = i.ToString("D4");     // Bill number is always 4 digits
            var str_bill_id = type+str_num;
            // Every revision of the bill with this Bill ID, e.g. AB0001
            var selected = (from item_bill in all_bills where (item_bill.BillID == str_bill_id) select item_bill).ToList();
            if (selected.Count > 0) { 
               // Lowest number revision (which is the most recent) becomes .First()
               selected.Sort((x,y) => string.Compare(x.Revision,y.Revision));
               result.Add(selected.First());
            }
         }
         return result;
      }

      /// Find the highest bill number in a list of bills.
      /// This is needed because iteration in MostRecent above is from 1 to highest bill number.
      static int HighestBillNumber(List<Bill_Identifier> all_bills) {
         int result = 0;
         foreach (var item in all_bills) {
            var num = 0;
            if (Int32.TryParse(item.Number,out num)) {
               if (num > result) result = num;
            }
         }
         return result;
      }

      /// Insert the most recent version of every bill into the BillRows table
      //static void InsertMostRecent(List<Bill_Identifier> asm_bills, List<Bill_Identifier> sen_bills) {
      //   BillRowsTable.CreateYourself();              // Ensure BillRows table exists
      //   BillRowsTable.ClearYourself();               // Ensure BillRows table is empty
      //   BillRowsTable.InsertCollection(asm_bills);   // Insert the Assembly bills
      //   BillRowsTable.InsertCollection(sen_bills);   // Insert the Senate bills
      //}
   }
}
