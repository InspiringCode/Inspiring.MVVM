namespace Inspiring.MvvmTest.ViewModels.Core.Collections {
   using System.Collections.Generic;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Inspiring.MvvmTest.Stubs;
   using Microsoft.VisualStudio.TestTools.UnitTesting;
   using Moq;

   [TestClass]
   public abstract class VMCollectionTestBase : BehaviorTestBase {

      protected static IPropertyAccessorBehavior<IEnumerable<ItemSource>> CreateCollectionSourceBehavior(
         IEnumerable<ItemSource> sourceItems
      ) {
         var stub = new Mock<IPropertyAccessorBehavior<IEnumerable<ItemSource>>>(MockBehavior.Strict);

         stub
            .Setup(x => x.GetValue(It.IsAny<IBehaviorContext>(), It.IsAny<ValueStage>()))
            .Returns(sourceItems);

         stub.SetupAllProperties();

         return stub.Object;
      }

      public class ItemSource {
      }

      public class ItemVM : ViewModelStub, IVMCollectionItem<ItemSource> {
         public void InitializeFrom(ItemSource source) {
            Source = source;
         }

         public ItemSource Source { get; private set; }
      }
   }
}
