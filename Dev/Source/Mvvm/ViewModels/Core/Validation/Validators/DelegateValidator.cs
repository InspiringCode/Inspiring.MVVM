namespace Inspiring.Mvvm.ViewModels.Core.Validation.Validators {
   using System;
   using System.Diagnostics.Contracts;
   using Inspiring.Mvvm.Common;

   internal sealed class DelegateValidator<TArgs> : Validator<TArgs>
      where TArgs : ValidationArgs {

      private readonly Action<TArgs> _validatorAction;

      public DelegateValidator(
         Func<ValidationRequest, TArgs> argsFactory,
         Action<TArgs> validatorAction
      )
         : base(argsFactory) {

         Contract.Requires(validatorAction != null);
         _validatorAction = validatorAction;
      }

      public override string ToString() {
         return String.Format(
            "{{DelegateValidator: {0}}}",
            DelegateUtils.GetFriendlyName(_validatorAction)
         );
      }

      protected override void Execute(TArgs args) {
         _validatorAction(args);
      }
   }
}
