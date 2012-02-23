namespace Inspiring.MvvmTest.Testing {
   using System;
   using Inspiring.Mvvm.Common;
   using Inspiring.Mvvm.Screens;
   using Inspiring.Mvvm.Testing;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class DialogTestServiceTests {
      private DialogTestService Service { get; set; }

      [TestInitialize]
      public void Setup() {
         Service = new DialogTestService(new EventAggregator());
      }

      [TestMethod]
      public void ShowDialog_ReturnsEnqueuedDialogResultAndDoesNotCreateScreen() {
         var expectedResult = new DialogScreenResult(true);
         Service.EnqueueShowDialogResponder(expectedResult);

         IDialogService svc = Service;
         var actualResult = svc.ShowDialog(new TestScreenFactory());
         Assert.AreSame(expectedResult, actualResult);
      }

      private class TestScreenFactory : IScreenFactory<IScreenBase> {
         public IScreenBase Create(EventAggregator aggregator, Action<IScreenBase> initializationCallback = null) {
            Assert.Fail();
            throw new InvalidOperationException();
         }

         public bool CreatesScreensEquivalentTo(IScreenBase concreteScreen) {
            Assert.Fail();
            throw new InvalidOperationException();
         }

         public Type ScreenType {
            get {
               Assert.Fail();
               throw new InvalidOperationException();
            }
         }
      }
   }
}