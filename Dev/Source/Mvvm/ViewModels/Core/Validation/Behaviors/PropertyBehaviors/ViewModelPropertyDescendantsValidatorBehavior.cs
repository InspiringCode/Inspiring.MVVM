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

         if (previousChild != null) {
            Revalidator.Revalidate(previousChild, ValidationScope.SelfAndLoadedDescendants);
         }

         if (value != null) {
            Revalidator.Revalidate(value, ValidationScope.SelfAndLoadedDescendants);
         }

         this.SetValueNext(context, value);
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
