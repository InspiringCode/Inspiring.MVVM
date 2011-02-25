namespace Inspiring.Mvvm.Screens {

   public interface IScreenLifecycle {
      IScreenLifecycle Parent { get; set; }

      void Activate();
      void Deactivate();
      bool RequestClose();
      void Close();
   }
}
