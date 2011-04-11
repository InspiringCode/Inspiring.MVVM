namespace Inspiring.Mvvm.ViewModels.Core {

   internal sealed class ParentSetterCollectionBehavior<TItemVM> :
      Behavior,
      IModificationCollectionBehavior<TItemVM>
      where TItemVM : IViewModel {

      public void HandleChange(IBehaviorContext context, CollectionChangedArgs<TItemVM> args) {
         // Set the parent first so that validation and change notification can
         // propagate properly.
         foreach (IViewModel item in args.NewItems) {
            item.Kernel.Parents.Add(args.Collection.Owner);
            item.Kernel.OwnerCollections.Add(args.Collection);
         }

         this.HandleChangeNext(context, args);

         // Clear the parent last so that validation and change notification can
         // propagate properly.
         foreach (IViewModel item in args.OldItems) {
            item.Kernel.Parents.Remove(args.Collection.Owner);
            item.Kernel.OwnerCollections.Remove(args.Collection);
         }
      }
   }
}
