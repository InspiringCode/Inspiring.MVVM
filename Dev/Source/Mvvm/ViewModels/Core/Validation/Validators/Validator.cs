namespace Inspiring.Mvvm.ViewModels.Core.Validation.Validators {
   using System;
   using System.Diagnostics.Contracts;

   internal abstract class Validator<TArgs> : IValidator
      where TArgs : ValidationArgs {

      public Validator(Func<ValidationRequest, TArgs> argsFactory) {
         Contract.Requires(argsFactory != null);
         ArgsFactory = argsFactory;
      }

      public Func<ValidationRequest, TArgs> ArgsFactory { get; private set; }

      public ValidationResult Execute(ValidationRequest request) {
         TArgs args = ArgsFactory(request);
         Execute(args);
         return args.Result;
      }

      protected abstract void Execute(TArgs args);
   }
}
