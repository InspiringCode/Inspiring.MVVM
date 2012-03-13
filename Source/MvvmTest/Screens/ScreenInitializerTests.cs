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
         var parent = new ParentScreenWithHandler<DerivedSubject>(Aggregator);
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
         var parent = new ParentScreenWithoutHandler(Aggregator);
         var child = new TestScreen<BaseSubject>(Aggregator);
         parent.Children.Attach(child);

         Initialize(parent, subject);
         Assert.AreEqual(subject, child.LastSubject, "Initialize was not called on child.");
      }

      [TestMethod]
      public void Initialize_WhenScreenImplementsINeedsInitialization_CallsVariousImplementations() {
         var subject = new DerivedSubject();
         var screen = new InitializableDerivedScreen(Aggregator);

         Initialize(screen, subject);

         Assert.AreEqual(subject, screen.DerivedSubject);
         Assert.AreEqual(subject, screen.BaseSubject);
         Assert.AreEqual(subject, screen.ISubject);
         Assert.AreEqual(1, screen.GeneralInitializeDerivedInvocations);
         Assert.AreEqual(1, screen.GeneralInitializeBaseInvocations);
      }

      private void Initialize<TSubject>(IScreenBase screen, TSubject subject) {
         new ScreenLifecycleOperations(Aggregator, screen)
            .Initialize(subject);
      }

      private class ParentScreenWithoutHandler : ScreenBase {
         public ParentScreenWithoutHandler(EventAggregator aggregator)
            : base(aggregator) {
         }
      }

      private class ParentScreenWithHandler<TSubject> : DefaultTestScreen {
         public ParentScreenWithHandler(EventAggregator aggregator)
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

      private class InitializableBaseScreen :
         DefaultTestScreen,
         INeedsInitialization<DerivedSubject>,
         INeedsInitialization<ISubject>,
         INeedsInitialization {

         public InitializableBaseScreen(EventAggregator aggregator)
            : base(aggregator) {
         }

         public ISubject ISubject { get; private set; }
         public DerivedSubject DerivedSubject { get; private set; }
         public int GeneralInitializeBaseInvocations { get; private set; }

         void INeedsInitialization<ISubject>.Initialize(ISubject subject) {
            ISubject = subject;
         }

         void INeedsInitialization<DerivedSubject>.Initialize(DerivedSubject subject) {
            DerivedSubject = subject;
         }

         void INeedsInitialization.Initialize() {
            GeneralInitializeBaseInvocations++;
         }
      }

      private class InitializableDerivedScreen :
         InitializableBaseScreen,
         INeedsInitialization<BaseSubject>,
         INeedsInitialization {

         public InitializableDerivedScreen(EventAggregator aggregator)
            : base(aggregator) {
         }

         public BaseSubject BaseSubject { get; private set; }
         public int GeneralInitializeDerivedInvocations { get; private set; }

         void INeedsInitialization<BaseSubject>.Initialize(BaseSubject subject) {
            BaseSubject = subject;
         }

         void INeedsInitialization.Initialize() {
            GeneralInitializeDerivedInvocations++;
         }
      }
   }
}