namespace Inspiring.Mvvm.ViewModels.Behaviors {

   public interface IBehaviorContext {
      FieldValueHolder FieldValues { get; }

      void RaisePropertyChanged<T>(VMProperty<T> property);
   }
}
