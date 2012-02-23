namespace Inspiring.Mvvm.Screens {

   public interface IScreenLifecycle {
      void Activate();
      void Deactivate();
      bool RequestClose();
      void Close();
      void Corrupt(object data = null);
   }
}
