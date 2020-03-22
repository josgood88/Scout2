using System;
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
      /// <summary>
      /// Create a file and write some contents to it.  Existing file is overwritten
      /// </summary>
      /// <param name="contents">The full contents of the file</param>
      /// <param name="path">Path to the file</param>
      public static void WriteTextFile(string contents, string path) {
         using (StreamWriter file = new StreamWriter(path)) {
            file.WriteLine(contents);
         }
      }
      /// <summary>
      /// Read the contents of a file into a single string
      /// </summary>
      /// <param name="path">Path to the file</param>
      /// <returns>The full contents of the file</returns>
      public static string ReadTextFile(string path) {
         try {
            using (FileStream fs = new FileStream(path, FileMode.Open)) {
               using (StreamReader reader = new StreamReader(fs)) {
                  return reader.ReadToEnd();
               }
            }
         } catch (Exception ex) {
            throw new ApplicationException($"ReadTextFile: \"{ex.Message}\" encountered reading {path}.");
         }
      }
   }
}