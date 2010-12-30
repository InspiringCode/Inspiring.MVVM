namespace Inspiring.Mvvm.ViewModels.Core {
   using System.Collections;
   using System.Diagnostics.Contracts;

   internal sealed class DescendantValidationBehavior<TValue> :
      Behavior,
      IDescendantValidationBehavior,
      IValidationStateProviderBehavior {

      private readonly bool _isViewModelProperty;
      private readonly bool _isCollectionProperty;

      public DescendantValidationBehavior() {
         _isViewModelProperty = PropertyTypeHelper.IsViewModel(typeof(TValue));
         _isCollectionProperty = PropertyTypeHelper.IsViewModelCollection(typeof(TValue));

         Contract.Assert(_isViewModelProperty || _isCollectionProperty);
      }


      public void RevalidateDescendants(
         IBehaviorContext context,
         ValidationContext validationContext,
         ValidationScope scope,
         ValidationMode mode
      ) {
         if (_isViewModelProperty) {
            var childVM = (IViewModel)this.GetValueNext<TValue>(context, ValueStage.None); // TODO: What stage?
            if (childVM != null) {
               childVM.Kernel.Revalidate(validationContext, scope, mode);
            }
         }

         if (_isCollectionProperty) {
            var collection = (IEnumerable)this.GetValueNext<TValue>(context, ValueStage.None); // TODO: What stage?

            if (collection != null) {
               foreach (IViewModel childVM in collection) {
                  childVM.Kernel.Revalidate(validationContext, scope, mode);
               }
            }
         }
      }

      public ValidationState GetValidationState(IBehaviorContext context) {
         return this.GetValidationStateNext(context);
      }

      public ValidationState GetDescendantsValidationState(IBehaviorContext context) {
         if (this.IsLoadedNext(context)) {
            if (_isViewModelProperty) {
               var childVM = (IViewModel)this.GetValueNext<TValue>(context, ValueStage.None); // TODO: What stage?
               if (childVM != null) {
                  return childVM.Kernel.GetValidationState(ValidationStateScope.All);
               }
            }

            if (_isCollectionProperty) {
               var collection = (IEnumerable)this.GetValueNext<TValue>(context, ValueStage.None); // TODO: What stage?

               if (collection != null) {
                  foreach (IViewModel childVM in collection) {
                     childVM.Kernel.GetValidationState(ValidationStateScope.All);
                  }
               }
            }
         }

         return ValidationState.Valid;
      }
   }
}
