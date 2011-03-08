using System.Collections.Generic;
using System.Linq;
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
         collectionMock.Setup(x => x.GetEnumerator()).Returns(Enumerable.Empty<ItemVM>().GetEnumerator());

         using (Sequence.Create()) {
            collectionMock.Setup(x => x.ReplaceItems(It.IsAny<IEnumerable<ItemVM>>()))
               .Callback<IEnumerable<ItemVM>>((c) => Assert.IsTrue(c.Contains(itemVM)));

            populator.Repopulate(Mock<IBehaviorContext>(), collectionMock.Object);
         }

         Assert.AreSame(itemSource, itemVM.Source, "ICanInitializeFrom was not called correctly on item VM.");
      }
   }
}
