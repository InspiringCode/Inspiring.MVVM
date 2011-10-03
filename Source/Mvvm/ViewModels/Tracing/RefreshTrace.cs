namespace Inspiring.Mvvm.ViewModels.Tracing {
   using System;
   using System.IO;
   using Inspiring.Mvvm.ViewModels.Core;

   public static class RefreshTrace {
      private static TraceBuilder _builder = new TraceBuilder();

      public static void StartTrace() {
         _builder.IsEnabled = true;
      }

      public static void EndTrace() {
         _builder.IsEnabled = false;
      }

      internal static void BeginManualRefresh() {
         BeginEntry(new ManualRefreshEntry());
      }

      internal static void BeginRefresh(IViewModel vm) {
         BeginEntry(new InternalRefreshEntry(vm));
      }

      internal static void BeginRefresh(IVMPropertyDescriptor property) {
         BeginEntry(new InternalRefreshEntry(property));
      }

      internal static void BeginRefresh(DeclarativeDependency refreshDependency) {
         BeginEntry(new DeclarativeRefreshEntry(refreshDependency));
      }

      internal static void EndLastRefresh() {
         _builder.EndLastEntry();
      }

      public static void WriteToFile(string path) {
         File.WriteAllText(path, _builder.Root.ToStringWithChildren());
      }

      private static void BeginEntry(TraceEntry entry) {
         _builder.BeginEntry(entry);
      }
   }

   internal sealed class ManualRefreshEntry : TraceEntry {
      public override string ToString() {
         return "Manual refresh";
      }
   }

   internal sealed class InternalRefreshEntry : TraceEntry {
      public InternalRefreshEntry(IVMPropertyDescriptor property) {
         Property = property;
      }

      public InternalRefreshEntry(IViewModel vm) {
         VM = vm;
      }

      public IViewModel VM { get; private set; }
      public IVMPropertyDescriptor Property { get; private set; }

      public override string ToString() {
         object target = Property != null ?
            (object)Property :
            (object)VM;

         return String.Format("Refresh of '{0}'", target);
      }
   }

   internal sealed class DeclarativeRefreshEntry : TraceEntry {
      public DeclarativeRefreshEntry(DeclarativeDependency dependency) {
         Dependency = dependency;
      }

      public DeclarativeDependency Dependency { get; private set; }

      public override string ToString() {
         return String.Format(
            "Refresh by dependency '{0}'",
            Dependency
         );
      }
   }
}
