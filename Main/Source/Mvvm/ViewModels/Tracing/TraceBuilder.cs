namespace Inspiring.Mvvm.ViewModels.Tracing {
   using System;
   using System.Collections.Generic;

   internal sealed class TraceBuilder {
      private readonly TraceEntry _root;
      private readonly Stack<TraceEntry> _traceStack;

      public TraceBuilder() {
         _root = new RootEntry();
         _traceStack = new Stack<TraceEntry>();
         _traceStack.Push(_root);
         IsEnabled = false;
      }

      public TraceEntry Root {
         get { return _root; }
      }

      public bool IsEnabled { get; set; }

      private TraceEntry Current {
         get { return _traceStack.Peek(); }
      }

      public void BeginEntry(TraceEntry entry) {
         if (IsEnabled) {
            Current.Children.Add(entry);
            _traceStack.Push(entry);
         }
      }

      public void EndLastEntry() {
         if (IsEnabled) {
            if (Current == _root) {
               throw new InvalidOperationException();
            }

            _traceStack.Pop();
         }
      }

      private class RootEntry : TraceEntry {
         public override string ToString() {
            return "Root";
         }
      }
   }
}
