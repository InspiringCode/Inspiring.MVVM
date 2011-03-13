namespace Inspiring.Mvvm.ViewModels.Core.Validation.Validators {
   using System.Diagnostics.Contracts;

   public abstract class ValidationArgs {
      protected ValidationArgs(IValidator validator) {
         Contract.Requires(validator != null);
         Validator = validator;
      }

      internal IValidator Validator { get; private set; }

      internal ValidationResult Result { get; private set; }

      protected void AddError(IViewModel target, string message) {
         var error = new ValidationError(target, Validator, message);
         var result = new ValidationResult(error);
         Result = ValidationResult.Join(Result, result);
      }
   }

   public abstract class ValidationArgs<TOwnerVM> : ValidationArgs
      where TOwnerVM : IViewModel {

      protected ValidationArgs(IValidator validator, TOwnerVM owner)
         : base(validator) {

         Contract.Requires(owner != null);
         Owner = owner;
      }

      public TOwnerVM Owner { get; private set; }
   }
}
