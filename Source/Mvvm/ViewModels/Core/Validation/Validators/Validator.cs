namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   
   internal abstract class Validator<TArgs> : IValidator
      where TArgs : ValidationArgs {

      private readonly Func<IValidator, ValidationRequest, TArgs> _argsFactory;

      public Validator(Func<IValidator, ValidationRequest, TArgs> argsFactory) {
         Check.NotNull(argsFactory, nameof(argsFactory));
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
