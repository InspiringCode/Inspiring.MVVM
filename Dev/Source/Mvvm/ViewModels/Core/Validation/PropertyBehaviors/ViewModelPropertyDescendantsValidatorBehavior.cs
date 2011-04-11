namespace Inspiring.Mvvm.ViewModels.Core.Validation.PropertyBehaviors {

   internal sealed class ViewModelPropertyDescendantsValidatorBehavior<TValue> :
      DescendantsValidatorBehavior
      where TValue : IViewModel {

      protected override void RevalidateDescendantsCore(IBehaviorContext context, ValidationScope scope) {
         var childVM = this.GetValueNext<TValue>(context);
         Revalidator.Revalidate(childVM, scope);
      }

      protected override ValidationResult GetDescendantsValidationResultCore(IBehaviorContext context) {
         var childVM = this.GetValueNext<TValue>(context);
         return childVM.Kernel.GetValidationState();
      }
   }
}
