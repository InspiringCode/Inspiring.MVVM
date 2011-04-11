namespace Inspiring.Mvvm.ViewModels.Core {
   using System.Diagnostics.Contracts;

   internal sealed class ItemDescriptorCollectionBehavior<TItemVM> :
      Behavior,
      IModificationCollectionBehavior<TItemVM>,
      IItemDescriptorProviderCollectionBehavior
      where TItemVM : IViewModel {

      private VMDescriptorBase _itemDescriptor;

      public ItemDescriptorCollectionBehavior(VMDescriptorBase itemDescriptor) {
         Contract.Requires(itemDescriptor != null);
         _itemDescriptor = itemDescriptor;
      }

      public VMDescriptorBase ItemDescriptor {
         get { return _itemDescriptor; }
      }

      public void HandleChange(IBehaviorContext context, CollectionChangedArgs<TItemVM> args) {
         foreach (IViewModel item in args.NewItems) {
            // TODO: Should we check old descriptor, should the Descriptor property handle this, or is it OK as it is now?
            item.Descriptor = _itemDescriptor;
         }

         this.HandleChangeNext(context, args);
      }
   }
}
