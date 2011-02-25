namespace Inspiring.MvvmTest {
   using System;
   using Inspiring.Mvvm;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public sealed class Bootstrapper {
      [AssemblyInitialize()]
      public static void Initialize(TestContext context) {
         ServiceLocator.SetServiceLocator(new ReflectionServiceLocator());
      }
   }
}
