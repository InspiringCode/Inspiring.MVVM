namespace Inspiring.Mvvm.ViewModels.Core {

   public sealed class RefreshReason : IChangeReason {
      private RefreshReason() {
      }

      // TODO: Make this internal?
      public static RefreshReason Create(bool executeRefreshDependencies) {
         return new RefreshReason { ExecuteRefreshDependencies = executeRefreshDependencies };
      }

      public bool ExecuteRefreshDependencies { get; private set; }
   }
}
