using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace Library {
   /// <summary>
   /// This class uses the Singleton pattern and owns the configuration data.
   /// It is responsible for deserializing the configuration data upon singleton construction
   /// and serializing the configuration data upon singleton destruction.
   /// </summary>
   public sealed class Config {
      private Dictionary<string, string> own = new Dictionary<string, string>();  // This contains the configuration information
      private static Config instance = null;
      private static readonly object padlock = new object();
      private Config() { }
      public static Config Instance {
         get {
            lock (padlock) {
               if (instance == null) {
                  instance = new Config();
               }
               return instance;
            }
         }
      }

      public string BillsFolder     { get { EnsureContents(); return own["bills_folder"]; } }
      public string DatabaseFolder  { get { EnsureContents(); return own["database_folder"]; } }
      public string DownloadsFolder { get { EnsureContents(); return own["downloads_folder"]; } }
      public string HtmlFolder      { get { EnsureContents(); return own["html_folder"]; } }
      public string LegSite         { get { EnsureContents(); return own["leg_site"]; } }
      public string NegativeFile    { get { EnsureContents(); return own["negative_file"]; } }
      public string PositiveFile    { get { EnsureContents(); return own["positive_file"]; } }
      public string ScoutFile       { get { EnsureContents(); return own["scout_file"]; } }
      private void EnsureContents() { if (own.Count == 0) ReadYourself();  }
      private const string manifest_file = "D:/CCHR/Projects/Scout2/Data/Config.json";

      private void ReadYourself() {
         if (File.Exists(manifest_file)) {
            var contents = File.ReadAllText(manifest_file);
            own = JsonConvert.DeserializeObject<Dictionary<string, string>>(contents);
         } else {
            own["bills_folder"]     = "D:/CCHR/2019-2020/LatestDownload/Bills/";
            own["database_folder"]  = "D:/CCHR/Projects/Scout/Data/";
            own["downloads_folder"] = "C:/Users/Joe/Downloads/";
            own["html_folder"]      = "D:/CCHR/2019-2020/Html/";
            own["leg_site"]         = "https://downloads.leginfo.legislature.ca.gov/";
            own["negative_file"]    = "D:/CCHR/Projects/Scout/ConfigurationData/RegexScore - Negative.xml";
            own["positive_file"]    = "D:/CCHR/Projects/Scout/ConfigurationData/RegexScore - Positive.xml";
            own["scout_file"]       = "D:/CCHR/Projects/Scout/ConfigurationData/RegexScore - Scout.xml";
         }
      }

      public void WriteYourself() {
         using (var file = File.CreateText(manifest_file)) {
            var serializer = new JsonSerializer();
            serializer.Serialize(file, own);
         }
      }
   };
}
