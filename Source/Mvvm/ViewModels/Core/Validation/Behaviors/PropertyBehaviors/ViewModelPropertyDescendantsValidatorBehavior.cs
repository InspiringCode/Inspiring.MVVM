using System;
namespace Inspiring.Mvvm.ViewModels.Core {

   internal sealed class ViewModelPropertyDescendantsValidatorBehavior<TValue> :
      DescendantsValidatorBehavior,
      IValueAccessorBehavior<TValue>
      where TValue : IViewModel {

      public TValue GetValue(IBehaviorContext context) {
         return this.GetValueNext<TValue>(context);
      }

      public void SetValue(IBehaviorContext context, TValue value) {
         DetectValidationResultChange(
            context,
            delegate {
               TValue previousChild = this.GetValueNext<TValue>(context);

               // The former parent of 'previousChild' (= still the current value of the 
               // property) was already removed by the 'ViewModelInitializerBehavior'.
               if (previousChild != null) {
                  Revalidator.Revalidate(previousChild, ValidationScope.SelfAndLoadedDescendants);
               }

               this.SetValueNext(context, value);
               
               // We MUST revalidate the value after updated the property value defined by
               // ancestors would not match before (because the 'PathDefinition' calls
               // 'GetValue' when matching paths). The drawback is that the new view model
               // is not revalidated yet when 'PropertyChanged' is raised. If this becomes
               // a problem, it would be possible to cache the new child VM temporarily and
               // return it in 'GetValue' if one is cached.
               if (value != null) {
                  Revalidator.Revalidate(value, ValidationScope.SelfAndLoadedDescendants);
               }
            }
         );
      }

      public override void Refresh(IBehaviorContext context, RefreshOptions options) {
         DetectValidationResultChange(
            context,
            () => base.Refresh(context, options)
         );
      }

      public override void InitializeValue(IBehaviorContext context) {
         base.InitializeValue(context);

         TValue childVM = this.GetValueNext<TValue>(context);
         if (childVM != null) {
            ValidationResult childValidatonResult = childVM
               .Kernel
               .GetValidationResult();

            if (!childValidatonResult.IsValid) {
               // If 'PopulatedWith' returns an already invalid item, the aggregated
               // validation state of the collection owner changes, therefore an
               // 'ValidationResultChanged' event should be raised.
               if (!childValidatonResult.IsValid) {
                  context.NotifyChange(ChangeArgs.ValidationResultChanged());
               }
            }
         }
      }

      protected override void RevalidateDescendantsCore(IBehaviorContext context, ValidationScope scope) {
         var childVM = this.GetValueNext<TValue>(context);
         if (childVM != null) {
            Revalidator.Revalidate(childVM, scope);
         }
      }

      protected override ValidationResult GetDescendantsValidationResultCore(IBehaviorContext context) {
         var childVM = this.GetValueNext<TValue>(context);

         return childVM != null ?
            childVM.Kernel.GetValidationResult() :
            ValidationResult.Valid;
      }

      // If an already invalid child is set or the previous child was invalid,
      // the aggregated validation state of the owner changes, therefore an
      // ValidationResultChanged event should be raised.
      private void DetectValidationResultChange(IBehaviorContext context, Action action) {
         IViewModel oldChild = this.GetValueNext<TValue>(context);

         action();

         IViewModel newChild = this.GetValueNext<TValue>(context);

         // It is important to query the 'oldResult' after executing the 'action'. The
         // following scenario could happen otherwise:
         //   1. 'oldResult' is assigned before the action and is valid.
         //   2. The action is executed and makes 'oldChild' invalid and 'newChild'
         //      stays valid. Since 'oldChild' is still in the hierarchy at this
         //      time, the error is propagated to the validation caches of the
         //      parent.
         //   3. 'newResult' is assigned to result of the 'newChild' (valid).
         //   4. We would not detect a change in the 'if' below and therefore not
         //      raise a change which would leave the validation cache of the parents
         //      with the stale error.
         ValidationResult oldResult = GetValidationResultOrValidIfNull(oldChild);
         ValidationResult newResult = GetValidationResultOrValidIfNull(newChild);

         if (!Object.Equals(oldResult, newResult)) {
            context.NotifyChange(ChangeArgs.ValidationResultChanged());
         }
      }

      private static ValidationResult GetValidationResultOrValidIfNull(IViewModel vm) {
         if (vm == null) {
            return ValidationResult.Valid;
         }

         return vm
            .Kernel
            .GetValidationResult();
      }
   }
}
