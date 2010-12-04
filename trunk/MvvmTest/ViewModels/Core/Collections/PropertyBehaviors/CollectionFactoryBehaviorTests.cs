using Inspiring.Mvvm.ViewModels.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Inspiring.MvvmTest.Stubs;

namespace Inspiring.MvvmTest.ViewModels.Behaviors {
   [TestClass]
   public class CollectionFactoryBehaviorTests : TestBase {
      [TestMethod]
      public void CreateCollection_ReturnsDifferentInstanceEachTime() {
         var fac = CreateFactory();
         var first = fac.CreateCollection(CreateContext());
         var second = fac.CreateCollection(CreateContext());

         Assert.IsNotNull(first);
         Assert.IsNotNull(second);
         Assert.AreNotSame(first, second);
      }

      [TestMethod]
      public void CreateCollection_AssignsSameBehaviorChainEachTime() {
         var fac = CreateFactory();
         var first = fac.CreateCollection(CreateContext());
         var second = fac.CreateCollection(CreateContext());

         Assert.AreSame(
            first.Reveal<BehaviorChain>("Behaviors"),
            second.Reveal<BehaviorChain>("Behaviors")
         );
      }

      private CollectionFactoryBehavior<ViewModelStub> CreateFactory() {
         var config = new BehaviorChainConfiguration();
         return new CollectionFactoryBehavior<ViewModelStub>(config);
      }

      private IBehaviorContext CreateContext() {
         return new BehaviorContextStub(new ViewModelStub());
      }
   }
}
