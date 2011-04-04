namespace Inspiring.MvvmTest.ViewModels.Core.PropertyBehaviors {
   using Inspiring.Mvvm.ViewModels.Core;
   using Inspiring.MvvmTest.Stubs;
   using Microsoft.VisualStudio.TestTools.UnitTesting;
   using Moq;

   [TestClass]
   public class ViewModelFactoryBehaviorTests : TestBase {
      [TestMethod]
      public void CreateViewModel_UsesServiceLocator() {
         var vm = new ViewModelStub();
         var sl = new ServiceLocatorStub();
         sl.Register<ViewModelStub>(vm);

         var ctxMock = new Mock<IBehaviorContext>();
         ctxMock.Setup(x => x.ServiceLocator).Returns(sl);

         var fac = new ServiceLocatorValueFactoryBehavior<ViewModelStub>();
         var actualVM = fac.CreateInstance(ctxMock.Object);

         Assert.AreSame(vm, actualVM);
      }
   }
}