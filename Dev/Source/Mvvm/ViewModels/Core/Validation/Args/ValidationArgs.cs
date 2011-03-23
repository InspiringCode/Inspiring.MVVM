namespace Inspiring.Mvvm.ViewModels.Core.Validation.Validators {
   using System.Diagnostics.Contracts;

   public abstract class ValidationArgs {
      protected ValidationArgs(IValidator validator) {
         Contract.Requires(validator != null);
         Validator = validator;
         Result = ValidationResult.Valid;
      }

      internal IValidator Validator { get; private set; }

      internal ValidationResult Result { get; private set; }

      protected void AddError(ValidationError error) {
         Result = ValidationResult.Join(Result, new ValidationResult(error));
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
