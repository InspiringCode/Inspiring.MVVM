namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
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

      private readonly Func<ValidatorConditionArgs<TOwnerVM, TTargetVM>, bool> _predicate;
      private readonly int _pathTargetIndex;

      public DelegateValidatorCondition(
         Func<ValidatorConditionArgs<TOwnerVM, TTargetVM>, bool> predicate,
         int pathTargetIndex
      ) {
         Check.NotNull(predicate, nameof(predicate));

         _predicate = predicate;
         _pathTargetIndex = pathTargetIndex;
      }

      public bool IsTrue(ValidationRequest request) {
         TOwnerVM owner = (TOwnerVM)request
            .TargetPath[0]
            .ViewModel;

         TTargetVM target = (TTargetVM)request
            .TargetPath[_pathTargetIndex]
            .ViewModel;

         Check.Requires<InvalidOperationException>(owner != null);
         Check.Requires<InvalidOperationException>(target != null);

         var args = new ValidatorConditionArgs<TOwnerVM, TTargetVM>(owner, target);
         return _predicate(args);
      }

      public override string ToString() {
         return DelegateUtils.GetFriendlyName(_predicate);
      }
   }
}
