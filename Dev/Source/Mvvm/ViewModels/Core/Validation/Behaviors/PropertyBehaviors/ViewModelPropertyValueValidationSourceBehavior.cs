namespace Inspiring.Mvvm.ViewModels.Core {

   internal sealed class ViewModelPropertyValueValidationSourceBehavior<TValue> :
      ValueValidationSourceBehavior<TValue> 
      where TValue : IViewModel {

      protected override void SetValueNext(IBehaviorContext context, TValue value) {
         TValue previousChild = GetValueNext(context);

         if (previousChild != null) {
            Revalidator.Revalidate(previousChild, ValidationScope.SelfAndLoadedDescendants);
         }

         if (value != null) {
            Revalidator.Revalidate(value, ValidationScope.SelfAndLoadedDescendants);
         }

         base.SetValueNext(context, value);
      }
   }
}
