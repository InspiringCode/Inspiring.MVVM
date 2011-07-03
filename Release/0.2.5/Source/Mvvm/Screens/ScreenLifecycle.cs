namespace Inspiring.Mvvm.Screens {

   public abstract class ScreenLifecycle : IScreenLifecycle {
      public virtual IScreenLifecycle Parent { get; set; }

      public virtual void Activate() {
      }

      public virtual void Deactivate() {
      }

      public virtual bool RequestClose() {
         return true;
      }

      public virtual void Close() {
      }

      public void Corrupt(object data = null) {
      }
   }
}
