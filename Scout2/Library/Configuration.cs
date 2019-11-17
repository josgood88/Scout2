using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Library {
   /// <summary>
   /// Singleton ownership of configuration data.
   /// </summary>
   public sealed class Config {
      private Dictionary<string, string> own = new Dictionary<string, string>();  // This contains the configuration information
      private static Config instance = null;
      private static readonly object access = new object();    // Lock this to prevent concurrent access
      private Config() { }                                     // Private is part of singleton pattern

      // Call this to get access to configuration data
      public static Config Instance {                          
         get {
            lock (access) {
               instance = instance ?? new Config();
               return instance;
            }
         }
      }

      // Users call these methods to access individual dictionary entries
      public string BillsFolder     { get { return ValueFor("bills_folder"); } }
      public string DatabaseFolder  { get { return ValueFor("database_folder"); } }
      public string DownloadsFolder { get { return ValueFor("downloads_folder"); } }
      public string HtmlFolder      { get { return ValueFor("html_folder"); } }
      public string LegSite         { get { return ValueFor("leg_site"); } }
      public string NegativeFile    { get { return ValueFor("negative_file"); } }
      public string PositiveFile    { get { return ValueFor("positive_file"); } }
      public string ScoutFile       { get { return ValueFor("scout_file"); } }

      // Guarantee no exception when accessing configuration data
      private string ValueFor(string key) {
         string result = string.Empty;
         if (!string.IsNullOrEmpty(key)) {
            if (own.ContainsKey(key)) result = own[key];
            result = result ?? string.Empty;
         }
         return result;
      }

      // File location is hardwired
      private const string configuration_file = "D:/CCHR/Projects/Scout2/ConfigurationData/Config.json";

      // Calling program given responsibilty for initializing dictionary contents.
      // See WriteYourself comments for discussion of this decision.
      public void ReadYourself() {
         if (File.Exists(configuration_file)) {
            var contents = File.ReadAllText(configuration_file);
            own = JsonConvert.DeserializeObject<Dictionary<string, string>>(contents);
         } else { // If no configuration file, use hard-coded initial data
            own["bills_folder"]     = "D:/CCHR/2019-2020/LatestDownload/Bills/";
            own["database_folder"]  = "D:/CCHR/Projects/Scout2/Data/";
            own["downloads_folder"] = "C:/Users/Joe/Downloads/";
            own["html_folder"]      = "D:/CCHR/2019-2020/Html/";
            own["leg_site"]         = "https://downloads.leginfo.legislature.ca.gov/";
            own["negative_file"]    = "D:/CCHR/Projects/Scout2/ConfigurationData/RegexScore - Negative.xml";
            own["positive_file"]    = "D:/CCHR/Projects/Scout2/ConfigurationData/RegexScore - Positive.xml";
            own["scout_file"]       = "D:/CCHR/Projects/Scout2/ConfigurationData/RegexScore - Scout.xml";
         }
      }

      // Calling program given responsibilty for persisting configuration data.
      // The primary argument against giving the user this responsibility is "Make it easy to use correctly
      // and hard to use incorrectly", which would mean 
      //    1. Having each accessor (e.g. BillsFolder()) check configuration data for 0 contents and read
      //       configuration data if no data is present.  ValueFor() could do this.
      //    2. Attaching an event handler to program exit -- the handler given responsibility for 
      //       persisting configuration data.
      //
      // The problem with this approach is that this makes the lifetime of the configuration data dependent on the
      // lifetime of the class that first references the configuration data.  In this application, a sequence of
      // of class instances process the data from initial download to report generation.  Configuration data would
      // be created and destroyed with each of these classes.  Realizing this would surprise a future developer and
      // thus is bad design.
      //
      // It is cleaner to begin the configuration data's lifetime with the primary form and the end the
      // configuration data's lifetime when the primary form closes.
      //
      public void WriteYourself() {
         using (var file = File.CreateText(configuration_file)) {
            var serializer = new JsonSerializer();
            serializer.Serialize(file, own);
         }
      }
   };
}