namespace Inspiring.Mvvm.ViewModels.Core.Validation.Validators {

   public abstract class ValidationArgs<TOwnerVM> {
      private readonly TOwnerVM _owner;
      private ValidationState _state;
   }
}
