namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Diagnostics.Contracts;
   using Inspiring.Mvvm.Common;

   internal static class DelegateValidator {
      public static IValidator For<TArgs>(
         Func<IValidator, ValidationRequest, TArgs> argsFactory,
         Action<TArgs> validatorAction
      ) where TArgs : ValidationArgs {
         return new DelegateValidator<TArgs>(argsFactory, validatorAction);
      }

      public static IValidator For<TOwnerVM, TTargetVM, TValue>(
         Action<PropertyValidationArgs<TOwnerVM, TTargetVM, TValue>> validationAction
      )
         where TOwnerVM : IViewModel
         where TTargetVM : IViewModel {

         return DelegateValidator.For(
            PropertyValidationArgs<TOwnerVM, TTargetVM, TValue>.Create,
            validationAction
         );
      }

      public static IValidator For<TOwnerVM, TItemVM>(
         Action<CollectionValidationArgs<TOwnerVM, TItemVM>> validationAction
      )
         where TOwnerVM : IViewModel
         where TItemVM : IViewModel {

         return DelegateValidator.For(
            CollectionValidationArgs<TOwnerVM, TItemVM>.Create,
            validationAction
         );
      }

      public static IValidator For<TOwnerVM, TItemVM, TValue>(
         Action<CollectionValidationArgs<TOwnerVM, TItemVM, TValue>> validationAction
      )
         where TOwnerVM : IViewModel
         where TItemVM : IViewModel {

         return DelegateValidator.For(
            CollectionValidationArgs<TOwnerVM, TItemVM, TValue>.Create,
            validationAction
         );
      }

      public static IValidator For<TOwnerVM, TTargetVM>(
         Action<ViewModelValidationArgs<TOwnerVM, TTargetVM>> validationAction
      )
         where TOwnerVM : IViewModel
         where TTargetVM : IViewModel {

         return DelegateValidator.For(
            ViewModelValidationArgs<TOwnerVM, TTargetVM>.Create,
            validationAction
         );
      }
   }

   internal sealed class DelegateValidator<TArgs> : Validator<TArgs>
      where TArgs : ValidationArgs {

      private readonly Action<TArgs> _validatorAction;

      public DelegateValidator(
         Func<IValidator, ValidationRequest, TArgs> argsFactory,
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
