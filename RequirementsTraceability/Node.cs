using System.Collections.Generic;

namespace RequirementsTraceability {
   public class Node {
      public string ID { get; }
      public string Title { get; }
      public List<Edge> Extends { get; }

      public Node() { ID = Title = string.Empty; Extends = new List<Edge>(); }
      public Node(string a, string b, List<Edge> c) { ID=a; Title=b; Extends=c; }

      public static bool operator !=(Node lhs, Node rhs) { return !(lhs==rhs); }
      public static bool operator ==(Node lhs, Node rhs) {
         if (lhs is null && rhs is null) return true;    // If both are null then they are equal
         if (lhs is null || rhs is null) return false;   // Else if either is null, they are not equal
         return lhs.ID == rhs.ID && lhs.Title == rhs.Title && lhs.Extends == rhs.Extends;
      }

      public override bool Equals(object other) { return this == (other as Node); }
      public override int GetHashCode() { return ID.GetHashCode() ^ Title.GetHashCode(); }
   }
}