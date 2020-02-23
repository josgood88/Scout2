using System.IO;

namespace Scout2.Utility {
   public class FileUtils {
      /// <summary>
      /// Given a file path, returns the contents of the file as a single string.
      /// An empty string is returned if the file does not exist.
      /// </summary>
      /// <param name="path">The fully-qualified name to the file whose contents are desired.</param>
      /// <returns></returns>
      public static string FileContents(string path) {
         return File.Exists(path) ? File.ReadAllText(path) : string.Empty;
      }
   }
}