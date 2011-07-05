namespace Inspiring.MvvmTest.ViewModels.Core {
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Inspiring.MvvmTest.ViewModels.Core.Properties.CollectionProperty;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public abstract class CollectionChangeHandlerBehaviorTestBase<TItemVM> :
      VMCollectionTestBase
      where TItemVM : IViewModel {

      protected ViewModelStub CollectionOwner { get; set; }

      protected BehaviorContextStub Context { get; set; }

      protected ICollectionChangeHandlerBehavior<TItemVM> Behavior { get; set; }

      protected IVMCollection<TItemVM> Collection { get; set; }

      protected void SetupFixture(
         ICollectionChangeHandlerBehavior<TItemVM> behavior,
         params IBehavior[] additionalBehaviors
      ) {
         Behavior = behavior;

         CollectionOwner = ViewModelStub
            .WithProperties(PropertyStub
               .WithBehaviors(behavior)
               .WithBehaviors(additionalBehaviors)
               .Build())
            .Build();

         Context = BehaviorContextStub
            .DecoratingContextOf(CollectionOwner)
            .Build();
      }

      protected void HandleItemInserted(TItemVM item, int index = 0) {
         var args = CollectionChangedArgs<TItemVM>.ItemInserted(Collection, item, index);
         Behavior.HandleChange(Context, args);
      }

      protected void HandleItemRemoved(TItemVM item, int index = 0) {
         var args = CollectionChangedArgs<TItemVM>.ItemRemoved(Collection, item, index);
         Behavior.HandleChange(Context, args);
      }

      protected void HandleItemSet(TItemVM previousItem, TItemVM item, int index = 0) {
         var args = CollectionChangedArgs<TItemVM>.ItemSet(Collection, previousItem, item, index);
         Behavior.HandleChange(Context, args);
      }

      protected void HandleCollectionCleared(TItemVM[] previousItems) {
         var args = CollectionChangedArgs<TItemVM>.CollectionCleared(Collection, previousItems);
         Behavior.HandleChange(Context, args);
      }

      protected void HandleCollectionPopulated(TItemVM[] previousItems) {
         var args = CollectionChangedArgs<TItemVM>.CollectionPopulated(Collection, previousItems);
         Behavior.HandleChange(Context, args);
      }

      protected IViewModel CreateItem(string description = "Item") {
         return ViewModelStub
            .Named(description)
            .Build();
      }

      protected VMCollectionStub<TItemVM> CreateCollection(params TItemVM[] items) {
         return VMCollectionStub
            .Of<TItemVM>()
            .WithItems(items)
            .WithOwner(CollectionOwner)
            .Build();
      }
   }
}
