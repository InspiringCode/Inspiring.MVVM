namespace Inspiring.Mvvm.ViewModels.Core {
   using System.Collections;
   using System.Diagnostics.Contracts;

   internal sealed class DescendantValidationBehavior<TValue> :
      Behavior,
      IDescendantValidationBehavior {

      private readonly bool _isViewModelProperty;
      private readonly bool _isCollectionProperty;

      public DescendantValidationBehavior() {
         _isViewModelProperty = PropertyTypeHelper.IsViewModel(typeof(TValue));
         _isCollectionProperty = PropertyTypeHelper.IsViewModelCollection(typeof(TValue));

         Contract.Assert(_isViewModelProperty || _isCollectionProperty);
      }


      public void RevalidateDescendants(
         IBehaviorContext context,
         ValidationScope scope,
         ValidationMode mode
      ) {
         if (_isViewModelProperty) {
            var childVM = (IViewModel)this.GetValueNext<TValue>(context, ValueStage.None); // TODO: What stage?
            if (childVM != null) {
               childVM.Kernel.Revalidate(scope, mode);
            }
         }

         if (_isCollectionProperty) {
            var collection = (IEnumerable)this.GetValueNext<TValue>(context, ValueStage.None); // TODO: What stage?

            if (collection != null) {
               foreach (IViewModel childVM in collection) {
                  childVM.Kernel.Revalidate(scope, mode);
               }
            }
         }
      }
   }
}
