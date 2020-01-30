
using System.Data;
namespace RequirementsTraceability {
   public class Edge {
      public string Parent { get; }
      public string Child { get; }

      public Edge(string a, string b) { Parent=a; Child=b;  }
      public Edge() { Parent = Child = string.Empty; }

      public static bool operator !=(Edge lhs, Edge rhs) { return !(lhs==rhs); }
      public static bool operator ==(Edge lhs, Edge rhs) {
         if (lhs is null && rhs is null) return true;    // If both are null then they are equal
         if (lhs is null || rhs is null) return false;   // Else if either is null, they are not equal
         return lhs.Parent == rhs.Parent && lhs.Child == rhs.Child;
      }

      public override bool Equals(object other) { return this == (other as Edge); }
      public override int GetHashCode() { return Parent.GetHashCode() ^ Child.GetHashCode(); }
   }
}