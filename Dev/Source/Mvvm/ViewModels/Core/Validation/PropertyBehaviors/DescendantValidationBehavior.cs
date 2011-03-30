namespace Inspiring.Mvvm.ViewModels.Core {
   using System.Collections;
   using System.Diagnostics.Contracts;
   using System.Linq;


   /// <remarks>
   ///   Requirements: If there is a <see cref="IIsLoadedBehavior"/> in the chain
   ///   it should come after this behavior.
   /// </remarks>
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
         if (scope == ValidationScope.SelfAndLoadedDescendants && !this.IsLoadedNext(context)) {
            this.RevalidateDescendantsNext(context, validationContext, scope, mode);
            return;
         }

         if (_isViewModelProperty) {
            var childVM = (IViewModel)this.GetValueNext<TValue>(context); // TODO: What stage?
            if (childVM != null) {
               childVM.Kernel.Revalidate(validationContext, scope, mode);
            }
         }

         if (_isCollectionProperty) {
            var collection = (IEnumerable)this.GetValueNext<TValue>(context); // TODO: What stage?

            if (collection != null) {
               foreach (IViewModel childVM in collection) {
                  childVM.Kernel.Revalidate(validationContext, scope, mode);
               }
            }
         }

         this.RevalidateDescendantsNext(context, validationContext, scope, mode); // TODO: There is some duplication!
      }

      public ValidationResult GetValidationState(IBehaviorContext context) {
         return this.GetValidationStateNext(context);
      }

      public ValidationResult GetDescendantsValidationState(IBehaviorContext context) {
         if (this.IsLoadedNext(context)) {
            if (_isViewModelProperty) {
               var childVM = (IViewModel)this.GetValueNext<TValue>(context); // TODO: What stage?
               if (childVM != null) {
                  var state = childVM.Kernel.GetValidationState(ValidationStateScope.All);
                  return state;
               }
            }

            if (_isCollectionProperty) {
               var collection = (IEnumerable)this.GetValueNext<TValue>(context); // TODO: What stage?

               if (collection != null) {
                  var state = ValidationResult.Join(
                     collection
                        .Cast<IViewModel>()
                        .Select(x => x.Kernel.GetValidationState(ValidationStateScope.All))
                        .ToArray()
                  );
                  return state;
               }
            }
         }

         return ValidationResult.Valid;
      }
   }
}
