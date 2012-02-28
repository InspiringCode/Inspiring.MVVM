namespace Inspiring.MvvmTest.Screens {
   using Inspiring.Mvvm.Common;
   using Inspiring.Mvvm.Screens;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class ScreenInitializerTests {
      private EventAggregator Aggregator { get; set; }

      [TestInitialize]
      public void Setup() {
         Aggregator = new EventAggregator();
      }

      [TestMethod]
      public void Initialize_WithSubjectOfExactType_CallsInitialize() {
         var subject = new DerivedSubject();
         var screen = new TestScreen<DerivedSubject>(Aggregator);
         Initialize(screen, subject);
         Assert.AreEqual(subject, screen.LastSubject);
      }

      [TestMethod]
      public void Initialize_WithSubjectOfDerivedType_CallsInitialize() {
         var subject = new DerivedSubject();
         var screen = new TestScreen<BaseSubject>(Aggregator);
         Initialize(screen, subject);
         Assert.AreEqual(subject, screen.LastSubject);
      }

      [TestMethod]
      public void Initialize_WithSubjectOfDerivedType_CallsInitializeOnScreenAndItsChildren() {
         var subject = new DerivedSubject();
         var parent = new InitializableParentScreen<DerivedSubject>(Aggregator);
         var child = new TestScreen<BaseSubject>(Aggregator);
         parent.Children.Attach(child);

         Initialize(parent, subject);
         Assert.AreEqual(subject, parent.LastSubject, "Initialize was not called on parent.");
         Assert.AreEqual(subject, child.LastSubject, "Initialize was not called on child.");
      }

      [TestMethod]
      public void Initialize_WhenScreenExpectsInterface_CallsInitialize() {
         var subject = new DerivedSubject();
         var screen = new ScreenWithInterfaceSubject(Aggregator);
         Initialize(screen, subject);
         Assert.AreEqual(subject, screen.LastSubject);
      }

      [TestMethod]
      public void Initialize_WhenCompileTimeTypeOfSubjectIsInterface_CallsInitializeOverloadOfRuntimeType() {
         var subject = new DerivedSubject();
         var screen = new TestScreen<BaseSubject>(Aggregator);
         Initialize(screen, (ISubject)subject);
         Assert.AreEqual(subject, screen.LastSubject);
      }

      [TestMethod]
      public void Initialize_WhenScreenDoesNotImplementInterface_CallsInitializeOnChildren() {
         var subject = new DerivedSubject();
         var parent = new NonInitializableParentScreen(Aggregator);
         var child = new TestScreen<BaseSubject>(Aggregator);
         parent.Children.Attach(child);

         Initialize(parent, subject);
         Assert.AreEqual(subject, child.LastSubject, "Initialize was not called on child.");
      }

      private void Initialize<TSubject>(IScreenBase screen, TSubject subject) {
         new ScreenLifecycleOperations(Aggregator, screen)
            .Initialize(subject);
      }

      private class NonInitializableParentScreen : ScreenBase {
         public NonInitializableParentScreen(EventAggregator aggregator)
            : base(aggregator) {
         }
      }

      private class InitializableParentScreen<TSubject> : DefaultTestScreen {
         public InitializableParentScreen(EventAggregator aggregator)
            : base(aggregator) {
            Lifecycle.RegisterHandler(ScreenEvents.Initialize<TSubject>(), HandleInitialize);
         }

         public TSubject LastSubject { get; set; }

         private void HandleInitialize(InitializeEventArgs<TSubject> args) {
            LastSubject = args.Subject;
         }
      }

      private class TestScreen<TSubject> : DefaultTestScreen {
         public TestScreen(EventAggregator aggregator)
            : base(aggregator) {
            Lifecycle.RegisterHandler(ScreenEvents.Initialize<TSubject>(), HandleInitialize);
         }

         public TSubject LastSubject { get; set; }

         private void HandleInitialize(InitializeEventArgs<TSubject> args) {
            LastSubject = args.Subject;
         }
      }

      private class ScreenWithInterfaceSubject : DefaultTestScreen {
         public ScreenWithInterfaceSubject(EventAggregator aggregator)
            : base(aggregator) {
            Lifecycle.RegisterHandler(ScreenEvents.Initialize<ISubject>(), HandleInitialize);
         }

         public ISubject LastSubject { get; set; }

         private void HandleInitialize(InitializeEventArgs<ISubject> args) {
            LastSubject = args.Subject;
         }
      }

      private interface ISubject {
         object Dummy { get; set; }
      }

      private class BaseSubject : ISubject {
         object ISubject.Dummy { get; set; }
      }

      private class DerivedSubject : BaseSubject {
      }
   }
}