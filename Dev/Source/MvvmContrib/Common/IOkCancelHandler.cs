namespace Inspiring.Mvvm.Common {
   public interface IOkCancelHandler {
      bool CanOk();
      void Ok();

      bool CanCancel();
      void Cancel();
   }
}
