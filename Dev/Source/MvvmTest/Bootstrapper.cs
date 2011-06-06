namespace Inspiring.MvvmTest {
   using System;
   using Inspiring.Mvvm;
   using Microsoft.VisualStudio.TestTools.UnitTesting;
   using Inspiring.Mvvm.Testability;

   [TestClass]
   public sealed class Bootstrapper {
      [AssemblyInitialize()]
      public static void Initialize(TestContext context) {
         //Contract.ContractFailed += (sender, e) => {
         //   e.SetHandled();
         //   e.SetUnwind();
         //   Assert.Fail(e.Message);
         //};

         ServiceLocator.SetServiceLocator(new ReflectionServiceLocator());
         TestFrameworkAdapter.SetTestFrameworkAdapter(new MSTestAdapter());
      }
   }
}
