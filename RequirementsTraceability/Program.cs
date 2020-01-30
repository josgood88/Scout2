using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace RequirementsTraceability {
   class Program {

      static void Main(string[] args) {
         Log.Instance("Starting");
         if (args.Any()) {
            Node node_ip = new Node();
            Edge edge_ip = new Edge();
            try {
               ImportRequirements(args.First(), out List<Node> requirements);
               string output_file_path = Graph.Create(args.First(), requirements);
               DisplayTheGraph(output_file_path);
            } catch (Exception ex) {
               Log.Instance(ex.Message);
            }
         } else {
            Log.Instance("Missing the argument that specifies the input file.");
         }
         Log.Instance("Complete");
      }

      /// <summary>
      /// Read the .txt requirements document.  Return the nodes and edges that connect the requirements.
      /// Fragility alert:
      /// This method assumes that ID:, Name: and Extends: occur on one line and in that order.
      /// </summary>
      /// <param name="file_path">Path to the .txt version of the requirements document</param>
      /// <param name="requirements">List of the nodes describing the requirements</param>
      static void ImportRequirements(string file_path, out List<Node> requirements) {
         requirements = new List<Node>();
         if (File.Exists(file_path)) {
            var lines = File.ReadAllLines(file_path);
            string id, title, extends;
            for (int index = 0; index < lines.Length; index++) {
               // Node description consists of 5 lines.  We want three of them.
               // Throw if any of the 4 lines following ID: are missing or are out of order.
               var s1 = lines[index];
               if (lines[index].Contains("ID:")) {
                  id = Regex.Replace(lines[index], @"ID:\s*(.+)", "$1").ToString();
                  var s2 = lines[index+1];
                  if (lines[++index].Contains("Last Edit:")) {
                     var s3 = lines[index+1];
                     if (lines[++index].Contains("Status:")) {
                        var s4 = lines[index+1];
                        if (lines[++index].Contains("Name:")) {
                           title = Regex.Replace(lines[index], @"Name:\s*(.+)", "$1").ToString();
                           var s5 = lines[index+1];
                           if (lines[++index].Contains("Extends:")) {
                              extends = Regex.Replace(lines[index], @"Extend:\s*(.+)", "$1").ToString();
                              List<Edge> edges = DefineEdges(id, extends);
                              requirements.Add(new Node(id, title, edges));
                           } else throw new ApplicationException($"Missing \"Extends:\"");
                        } else throw new ApplicationException($"Missing \"Name:\"");
                     } else throw new ApplicationException($"Missing \"Status:\"");
                  } else throw new ApplicationException($"Missing \"Last Edit:\"");
               } // Skip any line that doesn't start the 5-line sequence
            }
         } else {
            throw new ApplicationException($"Read {file_path}: File does not exist.");
         }
      }
      /// <summary>
      /// The current node extends (traces back to) zero or more other nodes.
      /// </summary>
      /// <param name="id">ID of the current node</param>
      /// <param name="extends">ID of those the current node traces back to</param>
      /// <returns></returns>
      static List<Edge> DefineEdges(string id, string extends) {
         var result = new List<Edge>();
         string ids_only = Regex.Replace(extends, @"Extends:\s*?(.*)", "$1");
         if (ids_only.Any()) {
            var strings = Regex.Split(ids_only, ",");
            foreach (string s in strings) {
               result.Add(new Edge(s, id));
            }
         }
         return result;
      }

      static void DisplayTheGraph(string output_file_path) {
         var directory = Path.GetDirectoryName(output_file_path);
         var batch_file_path = Path.Combine(directory, "run.bat");
         //string args = $"rundot.bat Traceability.dot Traceability.pdf";
         //var process = Process.Start("dir.exe", "*.*");
         //if (process != null) process.WaitForExit();
         RunOneCommand(batch_file_path, "");
      }

      static void RunOneCommand(string filename, string arguments) {
         try {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = filename;
            startInfo.Arguments = arguments;
            process.StartInfo = startInfo;
            process.Start();
         } catch (Exception ex) {
            Log.Instance($"RunOneCommand: {filename} {arguments} {ex.Message}");
         }
      }
   }
}
