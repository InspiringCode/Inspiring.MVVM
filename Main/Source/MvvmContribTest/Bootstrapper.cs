namespace Inspiring.MvvmTest {
   using Inspiring.Mvvm;
   using Inspiring.Mvvm.Testability;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public sealed class Bootstrapper {
      [AssemblyInitialize()]
      public static void Initialize(TestContext context) {
         ServiceLocator.SetServiceLocator(new ReflectionServiceLocator());
         TestFrameworkAdapter.SetTestFrameworkAdapter(new MSTestAdapter());
      }
   }
}
