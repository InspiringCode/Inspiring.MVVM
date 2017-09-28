namespace Inspiring.Mvvm.ViewModels.Core {
   public interface IDisplayValueAccessorBehavior : IBehavior {
      object GetDisplayValue(IBehaviorContext vm);
      void SetDisplayValue(IBehaviorContext vm, object value);
   }
}
