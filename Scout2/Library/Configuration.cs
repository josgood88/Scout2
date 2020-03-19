using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Scout2.Utility;

namespace Library {
   /// <summary>
   /// Singleton ownership of configuration data.
   /// </summary>
   public sealed class Config {
      private Dictionary<string, string> own = new Dictionary<string, string>();     // This contains the configuration information
      private Dictionary<string, string> agendas = new Dictionary<string, string>(); // This contains the committee agenda URLs
      private List<string> highest_priority = new List<string>(); // Define the highest-priority bills
      private List<string> manualCommittees = new List<string>(); // Define the manual changes to committee sequences
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
      public List<string> HighestPriority { get { return highest_priority;  } }
      public List<string> ManualCommitteeChanges { get { return manualCommittees; } }
      public Dictionary<string, string> Agendas { get { return agendas; } }

      // Guarantee no exception when accessing configuration data
      private string ValueFor(string key) {
         string result = string.Empty;
         if (!string.IsNullOrEmpty(key)) {
            if (own.ContainsKey(key)) result = own[key];
            result = result ?? string.Empty;
         }
         return result;
      }

      // File locations are hardwired
      private const string committee_agendas  = "D:/CCHR/Projects/Scout2/ConfigurationData/Committees.json";
      private const string configuration_file = "D:/CCHR/Projects/Scout2/ConfigurationData/Config.json";
      private const string high_priority_file = "D:/CCHR/Projects/Scout2/ConfigurationData/HighestPriority.json";
      private const string manual_comm_file   = "D:/CCHR/Projects/Scout2/ConfigurationData/NewManualRouting.json";

      // Calling program given responsibilty for initializing dictionary contents.
      // See WriteYourself comments for discussion of this decision.
      public void ReadYourself() {
         ReadConfigFileItems();
         ReadHighestPriorityItems();
         manualCommittees = ManualCommitteesUpdate();
         agendas = ReadCommitteeAgendaURLs();
      }
      private void ReadConfigFileItems() {
         if (File.Exists(configuration_file)) {
            var contents = FileUtils.FileContents(configuration_file);
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
      
      private void ReadHighestPriorityItems() {
         ManualCommitteesUpdate();
         if (File.Exists(high_priority_file)) {
            var contents = FileUtils.FileContents(high_priority_file);
            highest_priority = JsonConvert.DeserializeObject<List<string>>(contents);
         } else {
            highest_priority = new List<string>();
         }
      }

      public List<string> ManualCommitteesUpdate() {
         var result = new List<string>();
         if (File.Exists(high_priority_file)) {
            var contents = File.ReadAllText(manual_comm_file);
            result = JsonConvert.DeserializeObject<List<string>>(contents);
         }
         return result;
      }

      private Dictionary<string, string> ReadCommitteeAgendaURLs() {
         if (File.Exists(committee_agendas)) {
            var contents = FileUtils.FileContents(committee_agendas);
            agendas = JsonConvert.DeserializeObject<Dictionary<string, string>>(contents);
         }
         return agendas;
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
