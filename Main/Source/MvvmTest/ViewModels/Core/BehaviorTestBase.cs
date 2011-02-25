namespace Inspiring.MvvmTest.ViewModels.Core {
   using System;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;
   using Moq;

   [TestClass]
   public class BehaviorTestBase : TestBase {

      protected static IViewModelFactoryBehavior<TVM> CreateViewModelFactory<TVM>(
         Func<TVM> instanceFactory
      ) where TVM : IViewModel {
         var stub = new Mock<IViewModelFactoryBehavior<TVM>>();

         stub
            .Setup(x => x.CreateInstance(It.IsAny<IBehaviorContext>()))
            .Returns(instanceFactory);

         stub.SetupAllProperties();

         return stub.Object;
      }
   }
}
