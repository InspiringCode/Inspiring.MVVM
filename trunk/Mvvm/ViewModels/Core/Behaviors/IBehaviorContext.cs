namespace Inspiring.Mvvm.ViewModels.Core {

   public interface IBehaviorContext {
      FieldValueHolder FieldValues { get; }

      void RaisePropertyChanged<T>(VMProperty<T> property);
   }
}
