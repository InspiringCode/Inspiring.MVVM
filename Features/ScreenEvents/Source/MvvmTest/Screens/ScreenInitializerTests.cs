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
         var screen = new TestScreen<DerivedSubject>();
         Initialize(screen, subject);
         Assert.AreEqual(subject, screen.LastSubject);
      }

      [TestMethod]
      public void Initialize_WithSubjectOfDerivedType_CallsInitialize() {
         var subject = new DerivedSubject();
         var screen = new TestScreen<BaseSubject>();
         Initialize(screen, subject);
         Assert.AreEqual(subject, screen.LastSubject);
      }

      [TestMethod]
      public void Initialize_WithSubjectOfDerivedType_CallsInitializeOnScreenAndItsChildren() {
         var subject = new DerivedSubject();
         var parent = new InitializableParentScreen<DerivedSubject>();
         var child = new TestScreen<BaseSubject>();
         parent.Children.Add(child);

         Initialize(parent, subject);
         Assert.AreEqual(subject, parent.LastSubject, "Initialize was not called on parent.");
         Assert.AreEqual(subject, child.LastSubject, "Initialize was not called on child.");
      }

      [TestMethod]
      public void Initialize_WhenScreenExpectsInterface_CallsInitialize() {
         var subject = new DerivedSubject();
         var screen = new ScreenWithInterfaceSubject();
         Initialize(screen, subject);
         Assert.AreEqual(subject, screen.LastSubject);
      }

      [TestMethod]
      public void Initialize_WhenCompileTimeTypeOfSubjectIsInterface_CallsInitializeOverloadOfRuntimeType() {
         var subject = new DerivedSubject();
         var screen = new TestScreen<BaseSubject>();
         Initialize(screen, (ISubject)subject);
         Assert.AreEqual(subject, screen.LastSubject);
      }

      [TestMethod]
      public void Initialize_WhenScreenDoesNotImplementInterface_CallsInitializeOnChildren() {
         var subject = new DerivedSubject();
         var parent = new NonInitializableParentScreen();
         var child = new TestScreen<BaseSubject>();
         parent.Children.Add(child);

         Initialize(parent, subject);
         Assert.AreEqual(subject, child.LastSubject, "Initialize was not called on child.");
      }

      private void Initialize<TSubject>(IScreenBase screen, TSubject subject) {
         new ScreenLifecycleOperations(Aggregator, screen)
            .Initialize(subject);
      }

      private class NonInitializableParentScreen : ScreenBase {
         public NonInitializableParentScreen()
            : base(new EventAggregator()) {
         }
      }

      private class InitializableParentScreen<TSubject> : DefaultTestScreen, INeedsInitialization<TSubject> {
         public TSubject LastSubject { get; set; }

         public void Initialize(TSubject subject) {
            LastSubject = subject;
         }
      }

      private class TestScreen<TSubject> : DefaultTestScreen, INeedsInitialization<TSubject> {
         public TSubject LastSubject { get; set; }

         public void Initialize(TSubject subject) {
            LastSubject = subject;
         }
      }

      private class ScreenWithInterfaceSubject : DefaultTestScreen, INeedsInitialization<ISubject> {
         public ISubject LastSubject { get; set; }

         void INeedsInitialization<ISubject>.Initialize(ISubject subject) {
            LastSubject = subject;
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