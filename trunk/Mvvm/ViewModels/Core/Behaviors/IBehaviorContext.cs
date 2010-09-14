namespace Inspiring.Mvvm.ViewModels.Core {

   public interface IBehaviorContext {
      FieldValueHolder FieldValues { get; }

      IVMContext VMContext { get; set; }

      void RaisePropertyChanged<T>(VMProperty<T> property);
   }
}
