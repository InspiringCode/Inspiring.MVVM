namespace Inspiring.Mvvm.ViewModels.Core {

   public interface IPropertyBehaviorContext {
      FieldValueHolder FieldValues { get; }

      IViewModel VM { get; }

      void NotifyPropertyValidating(IVMProperty property, ValidationState validationState);

      void NotifyChange(ChangeArgs args);
   }
}
