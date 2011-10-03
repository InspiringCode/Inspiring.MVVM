namespace Inspiring.Mvvm.ViewModels.Tracing {
   using System.Collections.Generic;
   using System.Text;

   internal class TraceEntry {
      public TraceEntry() {
         Children = new List<TraceEntry>();
      }

      public ICollection<TraceEntry> Children { get; private set; }

      public string ToStringWithChildren() {
         var builder = new StringBuilder();
         ToStringWithChildren(builder, depth: 0);
         return builder.ToString();
      }

      private void ToStringWithChildren(StringBuilder builder, int depth) {
         builder.AppendLine(ToString());

         depth++;
         foreach (TraceEntry child in Children) {
            builder.Append('\t', depth);
            child.ToStringWithChildren(builder, depth);
         }
      }
   }
}
