namespace Inspiring.MvvmTest.ViewModels {
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
   using System.Text;

   [TestClass]
   public class VMCollectionTests : TestBase {
      [TestMethod]
      public void Move_FromIndexOneToZero_Success() {
         var item0 = CreateItem();
         var item1 = CreateItem();

         var collection = CreateCollection();
         collection.Add(item0);
         collection.Add(item1);

         collection.Move(fromIndex: 1, toIndex: 0);

         CollectionAssert.AreEqual(new IViewModel[] { item1, item0 }, collection);
      }

      [TestMethod]
      public void Move_FromIndexZeroToOne_Success() {
         var item0 = CreateItem();
         var item1 = CreateItem();

         var collection = CreateCollection();
         collection.Add(item0);
         collection.Add(item1);

         collection.Move(fromIndex: 0, toIndex: 1);

         CollectionAssert.AreEqual(new IViewModel[] { item1, item0 }, collection);
      }

      [TestMethod]
      public void GetItemProperties_ReturnsPropertyDescriptorCollection() {
         var itemTypeDescriptorBehavior = new TypeDescriptorProviderBehavior();

         var itemDescriptor = DescriptorStub
            .WithBehaviors(itemTypeDescriptorBehavior)
            .Build();

         var ownerProperty = PropertyStub
            .WithBehaviors(new ItemDescriptorProviderBehavior(itemDescriptor))
            .Build();

         var ownerVM = ViewModelStub
            .WithProperties(ownerProperty)
            .Build();

         var collection = new VMCollection<IViewModel>(ViewModelStub.Build(), ownerProperty);

         Assert.AreSame(itemTypeDescriptorBehavior.PropertyDescriptors, collection.GetItemProperties(null));
      }

      [TestMethod]
      public void SetItems_ListChangedIsRaisedAfterBehaviors() {
         var actionLog = new StringBuilder();
         
         var changeListener = new TestChangeListener();
         
         var ownerProperty = PropertyStub
            .WithBehaviors(changeListener)
            .Build();

         var ownerVM = ViewModelStub
            .WithProperties(ownerProperty)
            .Build();

         var collection = new VMCollection<IViewModel>(ownerVM, ownerProperty);

         changeListener.HandleChange += delegate {
            actionLog.Append("ChangeHandlerBehavior ");
         };

         collection.ListChanged += delegate {
            actionLog.Append("ListChanged ");
         };
         
         IVMCollection<IViewModel> c = collection;
         c.ReplaceItems(new[] { ViewModelStub.Build(), ViewModelStub.Build() }, null);

         Assert.AreEqual("ChangeHandlerBehavior ListChanged ", actionLog.ToString());         
      }

      private class TestChangeListener : Behavior, ICollectionChangeHandlerBehavior<IViewModel> {
         public event Action<CollectionChangedArgs<IViewModel>> HandleChange;

         void ICollectionChangeHandlerBehavior<IViewModel>.HandleChange(
            IBehaviorContext context, 
            CollectionChangedArgs<IViewModel> args
         ) {
            if (HandleChange != null) {
               HandleChange(args);
            }
         }
      }
      

      private VMCollection<IViewModel> CreateCollection() {
         return new VMCollection<IViewModel>(ViewModelStub.Build(), PropertyStub.Build());
      }

      private static IViewModel CreateItem() {
         return Mock<IViewModel>();
      }
   }
}