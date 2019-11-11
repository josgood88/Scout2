using System;

namespace Library {
   public static class ManifestConstants {
      public static string BillsFolder          { get { return bills_folder; } }
      public static string DatabaseFolder       { get { return database_folder; } }
      public static string DownloadsFolder      { get { return downloads_folder; } }
      public static string HtmlFolder           { get { return html_folder; } }
      public static string LegSite              { get { return leg_site; } }
      public static string NegativeFile         { get { return negative_file; } }
      public static string PositiveFile         { get { return positive_file; } }
      public static string ScoutFile            { get { return scout_file; } }

      private const string bills_folder           = "D:/CCHR/2019-2020/LatestDownload/Bills/";
      private const string database_folder        = "D:/CCHR/Projects/Scout/Data/";
      private const string downloads_folder       = "C:/Users/Joe/Downloads/";
      private const string html_folder            = "D:/CCHR/2019-2020/Html";
      private const string leg_site               = "https://downloads.leginfo.legislature.ca.gov/";
      private const string negative_file          = "D:/CCHR/Projects/Scout/ConfigurationData/RegexScore - Negative.xml";
      private const string positive_file          = "D:/CCHR/Projects/Scout/ConfigurationData/RegexScore - Positive.xml";
      private const string scout_file             = "D:/CCHR/Projects/Scout/ConfigurationData/RegexScore - Scout.xml";

   }
}
