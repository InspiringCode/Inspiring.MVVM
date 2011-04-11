namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using Inspiring.Mvvm.ViewModels.Core.Validation.Validators;

   public abstract class Validator : IValidator {
      internal void Validate(ValidationArgs args) {
         ValidateCore(args.SetTargetValidator(this));
      }

      public abstract void ValidateCore(ValidationArgs args);

      public ValidationResult Execute(ValidationRequest request) {
         throw new NotImplementedException();
      }
   }
}
