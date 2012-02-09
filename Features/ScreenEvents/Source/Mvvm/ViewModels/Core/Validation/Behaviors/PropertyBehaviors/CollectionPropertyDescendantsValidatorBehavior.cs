namespace Inspiring.Mvvm.ViewModels.Core {
   using System.Collections.Generic;
   using System.Linq;

   internal sealed class CollectionPropertyDescendantsValidatorBehavior<TItemVM> :
      DescendantsValidatorBehavior,
      ICollectionChangeHandlerBehavior<TItemVM>
      where TItemVM : IViewModel {

      public void HandleChange(IBehaviorContext context, CollectionChangedArgs<TItemVM> args) {
         IEnumerable<TItemVM> changedItems = args
            .OldItems
            .Concat(args.NewItems);

         IEnumerable<ValidationResult> changedItemsResults = changedItems
            .Select(x => x.Kernel.GetValidationResult());

         var changedItemsResult = ValidationResult.Join(changedItemsResults);

         this.HandleChangeNext(context, args);

         // If already invalid items are added or removed, the aggregated
         // validation state of the collection owner changes, therefore an
         // ValidationResultChanged event should be raised.
         if (!changedItemsResult.IsValid) {
            context.NotifyChange(ChangeArgs.ValidationResultChanged(args.Reason));
         }
      }

      protected override void RevalidateDescendantsCore(IBehaviorContext context, ValidationScope scope) {
         var items = (IEnumerable<IViewModel>)this.GetValueNext<IVMCollection<TItemVM>>(context);

         Revalidator.RevalidateItems(items, scope);
      }

      protected override ValidationResult GetDescendantsValidationResultCore(IBehaviorContext context) {
         var items = this.GetValueNext<IVMCollection<TItemVM>>(context);
         var descendantsResult = ValidationResult.Join(items.Select(x => x.Kernel.GetValidationResult()));
         return descendantsResult;
      }
   }
}
