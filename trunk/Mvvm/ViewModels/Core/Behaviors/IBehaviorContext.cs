namespace Inspiring.Mvvm.ViewModels.Core {

   public interface IBehaviorContext {
      FieldValueHolder FieldValues { get; }

      ViewModel VM { get; }

      IVMContext VMContext { get; set; } // TODO: Remove/Replace this?

      IServiceLocator ServiceLocator { get; }

      void RaisePropertyChanged<T>(VMPropertyBase<T> property);
   }
}
