using System;
using System.Data.SQLite;
using System.IO;

namespace Library.Database {
   public class DB {
      // Don't worry about environment separator, but make it easy to change
      public static char Separator { get { return ',';} }
      public const int SQL_INSERT_ROW_LIMIT = 1000;

      public DB() {
         if (!File.Exists(DBPath())) {
            SQLiteConnection.CreateFile(DBPath());
         }
      }

      public static string DBPath() { return Path.Combine(ManifestConstants.DatabaseFolder,"Scout.db"); }

      public static SQLiteConnection Connect() {
         return new SQLiteConnection($"Data Source={DBPath()}");
      }

      /// All non-query commands pass through this method
      public static void NonQuery(string non_query,string caller) {
         SQLiteConnection con = null;
         try {
            using (con = DB.Connect()) {              // Obtain SQLiteConnection
               con.Open();                            // Open the connection to the database
               using (SQLiteCommand cmd = new SQLiteCommand(con)) {
                  cmd.CommandText = non_query;        // Set CommandText contains the non-query
                  cmd.ExecuteNonQuery();              // Execute the query
               }
            }
         } catch (Exception ex) {
            Log.Instance.Info($"{caller}: {ex.Message}");
            throw;
         }
      }

      /// Wrap a string in quotation marks if the string contains a comma
      public static string QuoteIfComma(string str) {
         var result = str;
         if (str.Contains(",")) result = $"\"{str}\"";
         return result;
      }
   }
}
