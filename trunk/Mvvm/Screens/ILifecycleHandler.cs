namespace Inspiring.Mvvm.Screens {

   public interface ILifecycleHandler {
      ILifecycleHandler Parent { get; set; }

      void Activate();
      void Deactivate();
      bool RequestClose();
      void Close();
   }
}
