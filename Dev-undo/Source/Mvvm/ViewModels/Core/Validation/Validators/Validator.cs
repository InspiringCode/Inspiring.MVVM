namespace Inspiring.Mvvm.ViewModels.Core.Validation.Validators {
   using System;
   using System.Diagnostics.Contracts;

   internal abstract class Validator<TArgs> : IValidator
      where TArgs : ValidationArgs {

      private readonly Func<IValidator, ValidationRequest, TArgs> _argsFactory;

      public Validator(Func<IValidator, ValidationRequest, TArgs> argsFactory) {
         Contract.Requires(argsFactory != null);
         _argsFactory = argsFactory;
      }

      public ValidationResult Execute(ValidationRequest request) {
         TArgs args = _argsFactory(this, request);
         Execute(args);
         return args.Result;
      }

      protected abstract void Execute(TArgs args);
   }
}
