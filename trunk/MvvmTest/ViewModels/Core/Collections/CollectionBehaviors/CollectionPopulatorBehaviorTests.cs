using Inspiring.Mvvm.ViewModels;
using Inspiring.Mvvm.ViewModels.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Sequences;

namespace Inspiring.MvvmTest.ViewModels.Core.Collections {
   [TestClass]
   public class CollectionPopulatorBehaviorTests : VMCollectionTestBase {
      [TestMethod]
      public void Repopulate_Success() {
         var itemSource = new ItemSource();
         var itemVM = new ItemVM();

         var sourceAccessor = CreateCollectionSourceBehavior(new ItemSource[] { itemSource });
         var vmFactory = CreateViewModelFactory(() => itemVM);

         var populator = new PopulatorCollectionBehavior<ItemVM, ItemSource>();
         populator.Successor = vmFactory;
         populator.Successor.Successor = sourceAccessor;

         var collectionMock = new Mock<IVMCollection<ItemVM>>();

         using (Sequence.Create()) {
            collectionMock.SetupSet(x => x.IsPopulating = true).InSequence();
            collectionMock.Setup(x => x.Clear()).InSequence();
            collectionMock.Setup(x => x.Add(itemVM)).InSequence();
            collectionMock.SetupSet(x => x.IsPopulating = false).InSequence();

            populator.Repopulate(Mock<IBehaviorContext>(), collectionMock.Object);
         }

         Assert.AreSame(itemSource, itemVM.Source, "ICanInitializeFrom was not called correctly on item VM.");
      }
   }
}
