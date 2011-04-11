namespace Inspiring.Mvvm.ViewModels.Core {
   using System.Collections.Generic;
   using System.Linq;
   using Inspiring.Mvvm.ViewModels.Core.Validation;
   using Inspiring.Mvvm.ViewModels.Core.Validation.PropertyBehaviors;

   internal sealed class CollectionPropertyDescendantsValidatorBehavior<TItemVM> :
      DescendantsValidatorBehavior
      where TItemVM : IViewModel {

      protected override void RevalidateDescendantsCore(IBehaviorContext context, ValidationScope scope) {
         var items = (IEnumerable<IViewModel>)this.GetValueNext<IVMCollection<TItemVM>>(context);

         Revalidator.RevalidateItems(items, scope);
      }

      protected override ValidationResult GetDescendantsValidationResultCore(IBehaviorContext context) {
         var items = this.GetValueNext<IVMCollection<TItemVM>>(context);
         var descendantsResult = ValidationResult.Join(items.Select(x => x.Kernel.GetValidationState()));
         return descendantsResult;
      }
   }
}
