namespace Inspiring.MvvmTest.ViewModels.Core.Properties.Common {
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class ServiceLocatorValueFactoryBehaviorTests : TestBase {
      [TestMethod]
      public void CreateValue_UsesServiceLocator() {
         var valueToResolve = new TestObject();

         var context = BehaviorContextStub.Build();
         context.ServiceLocator = new ServiceLocatorStub()
            .Register<TestObject>(valueToResolve);

         var behavior = new ServiceLocatorValueFactoryBehavior<TestObject>();

         var actualValue = behavior.CreateValue(context);

         Assert.AreEqual(valueToResolve, actualValue);
      }

      private class TestObject { }
   }
}