namespace Inspiring.Mvvm.Screens {
   using Inspiring.Mvvm.Common;

   public class ParentLifecycleHandler : NotifyObject, ILifecycleHandler {
      public ParentLifecycleHandler() {
         Children = new LifecycleHandlerCollection<ILifecycleHandler>(this);
      }

      public LifecycleHandlerCollection<ILifecycleHandler> Children { get; private set; }

      public ILifecycleHandler Parent { get; set; }

      public void Add(ILifecycleHandler handler) {
         handler.Parent = Parent;
      }

      void ILifecycleHandler.Activate() {
         Children.ActivateAll(parentCallback: OnActivate);
      }

      void ILifecycleHandler.Deactivate() {
         Children.DeactivateAll(parentCallback: OnDeactivate);
      }

      bool ILifecycleHandler.RequestClose() {
         return Children.RequestCloseAll(parentCallback: OnRequestClose);
      }

      void ILifecycleHandler.Close() {
         Children.CloseAll(parentCallback: OnClose);
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
   }
}
