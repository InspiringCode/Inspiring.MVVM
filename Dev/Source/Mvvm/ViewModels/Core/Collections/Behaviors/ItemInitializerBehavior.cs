namespace Inspiring.Mvvm.ViewModels.Core {

   internal sealed class ItemInitializerBehavior<TItemVM> :
      Behavior,
      IModificationCollectionBehavior<TItemVM>
      where TItemVM : IViewModel {

      public void HandleChange(IBehaviorContext context, CollectionChangedArgs<TItemVM> args) {
         // The parent is set first and cleared last to allow proper propagation of
         // validation and change notification.

         VMDescriptorBase itemDescriptor = this.GetItemDescriptor();

         foreach (IViewModel item in args.NewItems) {
            item.Kernel.Parents.Add(args.Collection.Owner);
            item.Kernel.OwnerCollections.Add(args.Collection);

            // TODO: Should we check old descriptor, should the Descriptor property 
            //       handle this, or is it OK as it is now?
            item.Descriptor = itemDescriptor;
         }

         this.HandleChangeNext(context, args);

         foreach (IViewModel item in args.OldItems) {
            item.Kernel.Parents.Remove(args.Collection.Owner);
            item.Kernel.OwnerCollections.Remove(args.Collection);
         }
      }
   }
}
