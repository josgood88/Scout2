using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RequirementsTraceability {
   public class Graph {
      public static string Create(string input_file_path, List<Node> requirements) {
         var sb = new StringBuilder();
         Prolog(ref sb);
         MainSection(ref sb, requirements);
         SpecialLinks(ref sb);      // Ways of controlling the graph, fragil
         Epilog(ref sb);

         var directory = Path.GetDirectoryName(input_file_path);
         var output_file_path = Path.Combine(directory, "Traceability.dot");
         using (var outputFile = new StreamWriter(output_file_path)) {
            outputFile.WriteLine(sb.ToString());
         }
         return output_file_path;
      }

      private static void Prolog(ref StringBuilder sb) {
         sb.AppendLine("#");
         sb.AppendLine("digraph diagram");
         sb.AppendLine("{");
         sb.AppendLine("label=\"Requirements Graph\"");
         sb.AppendLine("  rankdir = LR;");
         sb.AppendLine("# concentrate = true;");
         sb.AppendLine("# edge[samehead=h1, sametail=t1];");
         sb.AppendLine("  edge[samehead=h1];");
      }

      private static void SpecialLinks(ref StringBuilder sb) {
         sb.AppendLine("\"F0001\" -> \"NF0001\" [style=invis]");
      }

      private static void Epilog(ref StringBuilder sb) {
         sb.AppendLine("}");
      }

      private static void MainSection(ref StringBuilder sb, List<Node> requirements) {
         foreach (var node in requirements) {
            sb.AppendLine($"\"{node.ID.Trim()}\" [label=\"{node.ID.Trim()}\n{node.Title.Trim()}\", shape = rectangle, style = filled, color = gray]");
            foreach (var link in node.Extends) {
               if (string.IsNullOrEmpty(link.Parent.Trim())) {
                  // No edge because no parent
               } else {
                  sb.AppendLine($"\"{link.Parent.Trim()}\" -> \"{link.Child.Trim()}\"");
               }
            }
         }
      }
   }
}