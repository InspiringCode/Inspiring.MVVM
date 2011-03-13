namespace Inspiring.Mvvm.ViewModels.Core.Validation.Validators {
   using System.Diagnostics.Contracts;

   public abstract class ValidationArgs<TOwnerVM> where TOwnerVM : IViewModel {
      protected ValidationArgs(IValidator validator, TOwnerVM owner) {
         Contract.Requires(validator != null);
         Contract.Requires(owner != null);

         Validator = validator;
         Owner = owner;
      }

      public TOwnerVM Owner { get; private set; }

      internal IValidator Validator { get; private set; }

      internal ValidationState State { get; private set; }

      protected void AddError(IViewModel target, string message) {
         var error = new ValidationError(target, Validator, message);
         var state = new ValidationState(error);
         State = ValidationState.Join(State, state);
      }
   }
}
