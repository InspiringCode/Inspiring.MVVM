namespace Inspiring.Mvvm.ViewModels.Core.Validation.Validators {
   using System;
   using System.Diagnostics.Contracts;

   internal abstract class Validator<TArgs> : IValidator
      where TArgs : ValidationArgs {

      private readonly Func<ValidationRequest, IValidator, TArgs> _argsFactory;

      public Validator(Func<ValidationRequest, IValidator, TArgs> argsFactory) {
         Contract.Requires(argsFactory != null);
         _argsFactory = argsFactory;
      }

      public ValidationResult Execute(ValidationRequest request) {
         TArgs args = _argsFactory(request, this);
         Execute(args);
         return args.Result;
      }

      protected abstract void Execute(TArgs args);
   }
}
