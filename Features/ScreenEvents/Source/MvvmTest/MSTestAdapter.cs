namespace Inspiring.MvvmTest {
   using Inspiring.Mvvm.Testability;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   internal sealed class MSTestAdapter : ITestFrameworkAdapter {

      public void Fail(string message) {
         Assert.Fail(message);
      }
   }
}
