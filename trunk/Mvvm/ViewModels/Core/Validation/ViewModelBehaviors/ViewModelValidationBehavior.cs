namespace Inspiring.Mvvm.ViewModels.Core {
   using Inspiring.Mvvm.Common;
   using Inspiring.Mvvm.ViewModels.Core.Validation;

   internal sealed class ViewModelValidationBehavior : ViewModelBehavior {


      public void AddValidation(PropertyPath propertyPath, ViewModelValidator validator) {

      }


      protected internal override void OnChanged(ChangeArgs args) {
         base.OnChanged(args);

      }

      //public void Validate(IViewModelBehaviorContext context) {
      //   ViewModelValidationArgs args = new ViewModelValidationArgs(
      //}

      private void Validate(ViewModelValidationArgs args) {

      }

      private class ValidationInvoker {
         public ValidationInvoker(PropertyPath validatorProperty, ViewModelValidator validator) {
            ValidatorProperty = validatorProperty;
            Validator = validator;
         }
         public PropertyPath ValidatorProperty { get; private set; }
         public ViewModelValidator Validator { get; private set; }

         public void InvokeValidation(ViewModelValidationArgs args) {

         }
      }
   }
}
