namespace Inspiring.MvvmTest.ViewModels.Core.Collections {
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Inspiring.MvvmTest.Stubs;
   using Microsoft.VisualStudio.TestTools.UnitTesting;
   using Moq;

   [TestClass]
   public abstract class CollectionModificationBehaviorTestBase<TItemVM> :
      VMCollectionTestBase
      where TItemVM : IViewModel {

      protected IViewModel CollectionOwner { get; set; }

      protected IBehaviorContext BehaviorContext { get; set; }

      protected IVMCollection<TItemVM> Collection { get; set; }

      protected IModificationCollectionBehavior<TItemVM> Behavior { get; set; }

      [TestInitialize]
      public virtual void Setup() {
         CollectionOwner = new ViewModelStub();
         BehaviorContext = Mock<IBehaviorContext>();
         Collection = CreateCollectionStub(itemCount: 1);
      }

      protected void Behavior_ItemInserted(TItemVM item, int index = 0) {
         Behavior.ItemInserted(BehaviorContext, Collection, item, index);
      }

      protected void Behavior_ItemRemoved(TItemVM item, int index = 0) {
         Behavior.ItemRemoved(BehaviorContext, Collection, item, index);
      }

      protected void Behavior_ItemSet(TItemVM previousItem, TItemVM item, int index = 0) {
         Behavior.ItemSet(BehaviorContext, Collection, previousItem, item, index);
      }

      protected void Behavior_ItemsCleared(TItemVM[] previousItems) {
         Behavior.CollectionCleared(BehaviorContext, Collection, previousItems);
      }

      protected void Behavior_CollectionPopulated(TItemVM[] previousItems) {
         Behavior.CollectionPopulated(BehaviorContext, Collection, previousItems);
      }

      protected IViewModel CreateItem() {
         var item = new ViewModelStub();
         item.OverrideContext(BehaviorContext);
         item.Kernel.Parent = new ViewModelStub();
         return item;
      }

      protected IVMCollection<TItemVM> CreateCollectionStub(int itemCount) {
         var stub = new Mock<IVMCollection<TItemVM>>();

         stub.Setup(x => x.Owner).Returns(CollectionOwner);
         stub.Setup(x => x.Count).Returns(itemCount);

         return stub.Object;
      }
   }
}
