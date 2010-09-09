namespace Inspiring.Mvvm.Screens {
   public abstract class ScreenBehavior {
      protected internal virtual void BeforeInitialize() {
      }

      protected internal virtual void AfterInitialize() {
      }

      protected internal virtual void BeforeInitialize<TSubject>(TSubject subject) {
      }

      protected internal virtual void AfterInitialize<TSubject>(TSubject subject) {
      }

      protected internal virtual void AfterInitialized() {
      }

      protected internal virtual void BeforeActivate() {
      }

      protected internal virtual void AfterActivate() {
      }

      protected internal virtual void BeforeDeactivate() {
      }

      protected internal virtual void AfterDeactivate() {
      }

      protected internal virtual bool BeforeRequestClose() {
         return true;
      }

      protected internal virtual bool AfterRequestClose() {
         return true;
      }

      protected internal virtual void BeforeClose() {
      }

      protected internal virtual void AfterClose() {
      }
   }
}
