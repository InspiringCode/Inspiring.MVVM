namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Diagnostics.Contracts;
   using Inspiring.Mvvm.Common;

   public sealed class ValidatorConditionArgs<TOwnerVM, TTargetVM> {
      public ValidatorConditionArgs(TOwnerVM owner, TTargetVM target) {
         Owner = owner;
         Target = target;
      }

      public TOwnerVM Owner { get; private set; }
      public TTargetVM Target { get; private set; }
   }

   internal sealed class DelegateValidatorCondition<TOwnerVM, TTargetVM> :
      ICondition<ValidationRequest> {

      private Func<ValidatorConditionArgs<TOwnerVM, TTargetVM>, bool> _predicate;

      public DelegateValidatorCondition(
         Func<ValidatorConditionArgs<TOwnerVM, TTargetVM>, bool> predicate
      ) {
         Contract.Requires(predicate != null);
         _predicate = predicate;
      }

      public bool IsTrue(ValidationRequest request) {
         var args = new ValidatorConditionArgs<TOwnerVM, TTargetVM>(
            (TOwnerVM)PathHelper.GetRootVM(request.TargetPath),
            (TTargetVM)request.Target
         );

         return _predicate(args);
      }

      public override string ToString() {
         return DelegateUtils.GetFriendlyName(_predicate);
      }
   }
}
