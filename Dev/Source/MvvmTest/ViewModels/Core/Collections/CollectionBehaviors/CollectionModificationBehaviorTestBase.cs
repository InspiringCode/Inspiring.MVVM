namespace Inspiring.MvvmTest.ViewModels.Core.Collections {
   using System.Collections.ObjectModel;
   using Inspiring.Mvvm;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Inspiring.MvvmTest.Stubs;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public abstract class CollectionModificationBehaviorTestBase<TItemVM> :
      VMCollectionTestBase
      where TItemVM : IViewModel {

      protected IViewModel CollectionOwner { get; set; }

      protected IBehaviorContext BehaviorContext { get; set; }

      protected IVMCollection<TItemVM> Collection { get; set; }

      protected IModificationCollectionBehavior<TItemVM> Behavior { get; set; }

      protected abstract TItemVM CreateAnonymousItem();

      [TestInitialize]
      public virtual void Setup() {
         CollectionOwner = new ViewModelStub();
         BehaviorContext = Mock<IBehaviorContext>();
         Collection = CreateCollectionStub(itemCount: 1);
      }

      protected void Behavior_ItemInserted(TItemVM item, int index = 0) {
         var args = CollectionChangedArgs<TItemVM>.ItemInserted(Collection, item, index);
         Behavior.HandleChange(BehaviorContext, args);
         //Behavior.ItemInserted(BehaviorContext, Collection, item, index);
      }

      protected void Behavior_ItemRemoved(TItemVM item, int index = 0) {
         var args = CollectionChangedArgs<TItemVM>.ItemRemoved(Collection, item, index);
         Behavior.HandleChange(BehaviorContext, args);
         //Behavior.ItemRemoved(BehaviorContext, Collection, item, index);
      }

      protected void Behavior_ItemSet(TItemVM previousItem, TItemVM item, int index = 0) {
         var args = CollectionChangedArgs<TItemVM>.ItemSet(Collection, previousItem, item, index);
         Behavior.HandleChange(BehaviorContext, args);
         //Behavior.ItemSet(BehaviorContext, Collection, previousItem, item, index);
      }

      protected void Behavior_ItemsCleared(TItemVM[] previousItems) {
         var args = CollectionChangedArgs<TItemVM>.CollectionCleared(Collection, previousItems);
         Behavior.HandleChange(BehaviorContext, args);
         //Behavior.CollectionCleared(BehaviorContext, Collection, previousItems);
      }

      protected void Behavior_CollectionPopulated(TItemVM[] previousItems) {
         var args = CollectionChangedArgs<TItemVM>.CollectionPopulated(Collection, previousItems);
         Behavior.HandleChange(BehaviorContext, args);
         //Behavior.CollectionPopulated(BehaviorContext, Collection, previousItems);
      }

      protected IViewModel CreateItem() {
         var item = new ViewModelStub();
         item.OverrideContext(BehaviorContext);
         //item.Kernel.Parent = new ViewModelStub();
         return item;
      }

      protected IVMCollection<TItemVM> CreateCollectionStub(int itemCount) {
         var stub = new VMCollectionMock(CollectionOwner);
         
         for (int i = 0; i < itemCount; i++) {
            stub.Add(CreateAnonymousItem());
         }
         
         return stub;
      }

      private class VMCollectionMock : Collection<TItemVM>, IVMCollection<TItemVM> {
         private readonly IViewModel _owner;

         public VMCollectionMock(IViewModel owner) {
            _owner = owner;
         }

         public void ReplaceItems(System.Collections.Generic.IEnumerable<TItemVM> newItems) {
            throw new System.NotImplementedException();
         }

         public BehaviorChain Behaviors {
            get { throw new System.NotImplementedException(); }
         }

         public bool IsPopulating {
            get {
               throw new System.NotImplementedException();
            }
            set {
               throw new System.NotImplementedException();
            }
         }

         public IViewModel Owner {
            get { return _owner; }
         }
      }
   }
}
