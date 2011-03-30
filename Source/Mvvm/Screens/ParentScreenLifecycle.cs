namespace Inspiring.Mvvm.Screens {
   using Inspiring.Mvvm.Common;

   public class ParentScreenLifecycle : NotifyObject, IScreenLifecycle {
      public ParentScreenLifecycle() {
         Children = new ScreenLifecycleCollection<IScreenLifecycle>(this);
         Children.Add(new ScreenHierarchyLifecycle());
      }

      public ScreenLifecycleCollection<IScreenLifecycle> Children { get; private set; }

      public IScreenLifecycle Parent { get; set; }

      void IScreenLifecycle.Activate() {
         Children.ActivateAll(parentCallback: OnActivate);
      }

      void IScreenLifecycle.Deactivate() {
         Children.DeactivateAll(parentCallback: OnDeactivate);
      }

      bool IScreenLifecycle.RequestClose() {
         return Children.RequestCloseAll(parentCallback: OnRequestClose);
      }

      void IScreenLifecycle.Close() {
         Children.CloseAll(parentCallback: OnClose);
      }

      void IScreenLifecycle.Corrupt(object data) {
         Children.CorruptAll(data, parentCallback: () => OnCorrupt(data));
      }

      protected virtual void OnActivate() {
      }

      protected virtual void OnDeactivate() {
      }

      protected virtual bool OnRequestClose() {
         return true;
      }

      protected virtual void OnClose() {
      }

      protected virtual void OnCorrupt(object data) {
      }
   }
}
