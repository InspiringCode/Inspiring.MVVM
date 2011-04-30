namespace Inspiring.Mvvm.ViewModels.Core {

   internal sealed class ItemInitializerBehavior<TItemVM> :
      Behavior,
      ICollectionChangeHandlerBehavior<TItemVM>
      where TItemVM : IViewModel {

      public void HandleChange(IBehaviorContext context, CollectionChangedArgs<TItemVM> args) {
         // The parent is set first and cleared last to allow proper propagation of
         // validation and change notification.

         VMDescriptorBase itemDescriptor = this.GetItemDescriptor();

         foreach (IViewModel item in args.NewItems) {
            // TODO: Should we check old descriptor, should the Descriptor property 
            //       handle this, or is it OK as it is now?
            // TODO: Test this feature.
            // The 'Descriptor' must be set before accessing the 'Kernel'!
            item.Descriptor = itemDescriptor;

            item.Kernel.Parents.Add(args.Collection.OwnerVM);
            item.Kernel.OwnerCollections.Add(args.Collection);
         }

         this.HandleChangeNext(context, args);

         foreach (IViewModel item in args.OldItems) {
            item.Kernel.Parents.Remove(args.Collection.OwnerVM);
            item.Kernel.OwnerCollections.Remove(args.Collection);
         }
      }
   }
}
