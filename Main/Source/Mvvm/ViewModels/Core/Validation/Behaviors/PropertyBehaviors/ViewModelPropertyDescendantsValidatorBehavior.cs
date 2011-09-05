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
         TValue previousChild = this.GetValueNext<TValue>(context);

         var oldResult = ValidationResult.Valid;
         var newResult = ValidationResult.Valid;

         if (previousChild != null) {
            oldResult = previousChild
               .Kernel
               .GetValidationResult();

            Revalidator.Revalidate(previousChild, ValidationScope.SelfAndLoadedDescendants);
         }

         if (value != null) {
            Revalidator.Revalidate(value, ValidationScope.SelfAndLoadedDescendants);

            newResult = value
               .Kernel
               .GetValidationResult();
         }

         this.SetValueNext(context, value);

         // If an already invalid child is set or the previous child was invalid,
         // the aggregated validation state of the owner changes, therefore an
         // ValidationResultChanged event should be raised.
         if (!Object.Equals(oldResult, newResult)) {
            context.NotifyChange(ChangeArgs.ValidationResultChanged());
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
   }
}
