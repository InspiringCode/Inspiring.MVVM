namespace Inspiring.Mvvm.ViewModels {
   using Inspiring.Mvvm.ViewModels.Core;

   public interface IViewModel {
      VMKernel Kernel { get; }
      object GetValue(IVMProperty property, ValueStage stage = ValueStage.PreValidation);
      void SetValue(IVMProperty property, object value);
      IBehaviorContext GetContext();
      void RaisePropertyChanged(string propertyName);
   }
}
