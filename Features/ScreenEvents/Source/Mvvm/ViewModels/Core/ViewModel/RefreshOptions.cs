namespace Inspiring.Mvvm.ViewModels.Core {
   using System;

   [Flags]
   public enum RefreshScope {
      Content = 0x01,
      Container = 0x02,
      ContainerAndContent = Container | Content
   }

   public sealed class RefreshOptions {
      public RefreshOptions(bool executeRefreshDependencies = false)
         : this(RefreshScope.ContainerAndContent, executeRefreshDependencies) {
      }

      public RefreshOptions(RefreshScope scope, bool executeRefreshDependencies = false) {
         Scope = scope;
         ExecuteRefreshDependencies = executeRefreshDependencies;
      }

      public bool ExecuteRefreshDependencies { get; private set; }
      public RefreshScope Scope { get; private set; }
   }
}
