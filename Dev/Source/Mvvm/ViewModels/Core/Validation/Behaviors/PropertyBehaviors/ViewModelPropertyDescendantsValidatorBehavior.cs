namespace Inspiring.Mvvm.ViewModels.Core {

   internal sealed class ViewModelPropertyDescendantsValidatorBehavior<TValue> :
      DescendantsValidatorBehavior
      where TValue : IViewModel {

      protected override void RevalidateDescendantsCore(IBehaviorContext context, ValidationScope scope) {
         var childVM = this.GetValueNext<TValue>(context);
         if (childVM != null) {
            Revalidator.Revalidate(childVM, scope);
         }
      }

      protected override ValidationResult GetDescendantsValidationResultCore(IBehaviorContext context) {
         var childVM = this.GetValueNext<TValue>(context);
         return childVM.Kernel.GetValidationState();
      }
   }
}
