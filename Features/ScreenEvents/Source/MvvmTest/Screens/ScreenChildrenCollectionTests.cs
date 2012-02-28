namespace Inspiring.MvvmTest.Screens {
   using System;
   using Inspiring.Mvvm.Common;
   using Inspiring.Mvvm.Screens;
   using Inspiring.MvvmTest.Stubs;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class ScreenChildrenCollectionTests {
      private DefaultTestScreen Parent { get; set; }
      private EventAggregator Aggregator { get; set; }
      private ScreenChildrenCollection<object> Collection { get; set; }
      private ServiceLocatorStub Locator { get; set; }

      [TestInitialize]
      public void Setup() {
         Aggregator = new EventAggregator();
         Parent = new DefaultTestScreen();
         Collection = new ScreenChildrenCollection<object>(Aggregator, Parent);
         Locator = new ServiceLocatorStub();
         Locator.Register(new TestScreen(Aggregator));
      }

      [TestMethod]
      public void AddScreen_SetsParent() {
         TestScreen s = Collection.AddScreen(ScreenFactory.For<TestScreen>(Locator));
         Assert.AreEqual(Parent, s.Parent);
      }

      [TestMethod]
      public void AddScreen_CallsInitialize() {
         Object subject = new Object();
         TestScreen s = Collection.AddScreen(
            ScreenFactory.WithSubject(subject).For<TestScreen>(Locator)
         );
         Assert.AreEqual(subject, s.Subject);
      }

      private class TestScreen : DefaultTestScreen {
         public TestScreen(EventAggregator aggregator)
            : base(aggregator) {

            Lifecycle.RegisterHandler(
               ScreenEvents.Initialize<object>(),
               args => Subject = args.Subject
            );
         }

         public object Subject { get; private set; }
      }
   }
}
