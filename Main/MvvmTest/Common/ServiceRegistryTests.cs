namespace Inspiring.MvvmTest.Common {
   using System;
   using Inspiring.Mvvm.Common;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class ServiceRegistryTests {
      [TestMethod]
      public void GetService_ServiceNotRegistered_ThrowsException() {
         ServiceRegistry registry = new ServiceRegistry();

         AssertHelper.Throws<ArgumentException>(
            () => registry.GetService<TestClass>()
         )
         .Containing("TestClass");
      }

      [TestMethod]
      public void RegisterService_GetService_ReturnsRegisteredInstance() {
         ServiceRegistry registry = new ServiceRegistry();
         TestClass registeredInstance = new TestClass();
         registry.RegisterService(registeredInstance);

         TestClass resolvedInstance = registry.GetService<TestClass>();

         Assert.AreSame(registeredInstance, resolvedInstance);
      }

      [TestMethod]
      public void RegisterService_ServiceAlreadyRegistered_ThrowsException() {
         ServiceRegistry registry = new ServiceRegistry();
         registry.RegisterService(new TestClass());

         AssertHelper.Throws<ArgumentException>(
            () => registry.RegisterService(new TestClass())
         )
         .Containing("TestClass");
      }

      private class TestClass {
      }
   }
}
