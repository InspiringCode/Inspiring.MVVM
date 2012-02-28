namespace Inspiring.MvvmTest.Screens {
   using System;
   using System.Linq;
   using Inspiring.Mvvm.Common;
   using Inspiring.Mvvm.Screens;
   using Inspiring.MvvmTest.ViewModels;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class ScreenCreationBehaviorTest : TestBase {
      private EventAggregator Aggregator { get; set; }
      private ServiceLocatorStub Locator { get; set; }

      [TestInitialize]
      public void Setup() {
         Aggregator = new EventAggregator();
         Locator = new ServiceLocatorStub();
         Locator.Register<MultipleInstancesScreen>(() => new MultipleInstancesScreen(Aggregator));
         Locator.Register<SingleInstanceScreen>(() => new SingleInstanceScreen(Aggregator));
         Locator.Register<DefaultCreationBehaviorScreen>(() => new DefaultCreationBehaviorScreen(Aggregator));
         Locator.Register<LocatableScreen>(() => new LocatableScreen(Aggregator));
         Locator.Register<DerivedLocatableScreen>(() => new DerivedLocatableScreen(Aggregator));
      }

      [TestMethod]
      public void MultipleInstances() {
         ScreenConductor c = CreateConductor();
         c.OpenScreen(ScreenFactory.For<MultipleInstancesScreen>(Locator));
         c.OpenScreen(ScreenFactory.For<MultipleInstancesScreen>(Locator));
         Assert.AreEqual(2, c.Screens.Count());
      }

      [TestMethod]
      public void SingleInstance() {
         ScreenConductor c = CreateConductor();
         c.OpenScreen(ScreenFactory.For<SingleInstanceScreen>(Locator));
         c.OpenScreen(ScreenFactory.For<SingleInstanceScreen>(Locator));
         Assert.AreEqual(1, c.Screens.Count());
      }

      [TestMethod]
      public void DefaultCreationBehavior() {
         ScreenConductor c = CreateConductor();
         c.OpenScreen(ScreenFactory.For<DefaultCreationBehaviorScreen>(Locator));
         c.OpenScreen(ScreenFactory.For<DefaultCreationBehaviorScreen>(Locator));
         Assert.AreEqual(2, c.Screens.Count());
      }

      [TestMethod]
      public void OpenScreenUseScreenLocation_WhenNoMatchingScreenIsOpen_CreatesNewScreen() {
         var conductor = CreateConductor();
         var firstSubject = new BaseSubject { Value = "First Subject" };
         var differentSubject = new BaseSubject { Value = "Different Subject" };

         conductor.OpenScreen(
            ScreenFactory
               .For<MultipleInstancesScreen>(Locator)
         );

         conductor.OpenScreen(
            ScreenFactory
               .WithSubject(firstSubject)
               .For<LocatableScreen>(Locator)
         );

         conductor.OpenScreen(
            ScreenFactory
               .WithSubject(differentSubject)
               .For<LocatableScreen>(Locator)
         );

         CollectionAssert.AreEqual(
            new[] { firstSubject, differentSubject },
            GetSubjectsOfOpenScreens(conductor)
         );
      }

      [TestMethod]
      public void OpenScreenUseScreenLocation_WhenMatchingScreenIsAlreadyOpen_ActivatesScreen() {
         var conductor = CreateConductor();
         var singleSubject = new BaseSubject { Value = "Single Subject" };

         conductor.OpenScreen(
            ScreenFactory
               .WithSubject(singleSubject)
               .For<LocatableScreen>(Locator)
         );

         conductor.OpenScreen(
            ScreenFactory
               .WithSubject(singleSubject)
               .For<LocatableScreen>(Locator)
         );

         CollectionAssert.AreEqual(
            new[] { singleSubject },
            GetSubjectsOfOpenScreens(conductor)
         );
      }

      [TestMethod]
      public void OpenScreenUseScreenLocation_WhenAttributeIsOnBaseClass_Works() {
         var conductor = CreateConductor();
         var singleSubject = new BaseSubject { Value = "Single Subject" };

         conductor.OpenScreen(
            ScreenFactory
               .WithSubject(singleSubject)
               .For<DerivedLocatableScreen>(Locator)
         );

         conductor.OpenScreen(
            ScreenFactory
               .WithSubject(singleSubject)
               .For<DerivedLocatableScreen>(Locator)
         );

         CollectionAssert.AreEqual(
            new[] { singleSubject },
            GetSubjectsOfOpenScreens(conductor)
         );
      }


      [TestMethod]
      public void OpenScreenUseScreenLocation_WhenScreenFactoryIsDowncasted_Works() {
         var conductor = CreateConductor();
         var singleSubject = new BaseSubject { Value = "Single Subject" };

         conductor.OpenScreen(
            ScreenFactory
               .WithSubject(singleSubject)
               .For<DerivedLocatableScreen>(Locator)
         );

         IScreenFactory<IScreenBase> downcasted = ScreenFactory
            .WithSubject(singleSubject)
            .For<DerivedLocatableScreen>(Locator);

         conductor.OpenScreen(downcasted);

         CollectionAssert.AreEqual(
            new[] { singleSubject },
            GetSubjectsOfOpenScreens(conductor)
         );
      }

      [TestMethod]
      public void OpenScreenUseScreenLocation_WhenScreenImplementsLocatableScreenForBaseClass_ActivatesScreen() {
         var conductor = CreateConductor();
         var singleSubject = new DerivedSubject { Value = "Single Subject" };

         conductor.OpenScreen(
            ScreenFactory
               .WithSubject(singleSubject)
               .For<LocatableScreen>(Locator)
         );

         conductor.OpenScreen(
            ScreenFactory
               .WithSubject(singleSubject)
               .For<LocatableScreen>(Locator)
         );

         CollectionAssert.AreEqual(
            new[] { singleSubject },
            GetSubjectsOfOpenScreens(conductor)
         );
      }

      private BaseSubject[] GetSubjectsOfOpenScreens(ScreenConductor conductor) {
         return conductor
            .Screens
            .OfType<LocatableScreen>()
            .Select(x => x.Subject)
            .ToArray();
      }

      private ScreenConductor CreateConductor() {
         return new ScreenConductor(Aggregator);
      }

      [ScreenCreationBehavior(ScreenCreationBehavior.MultipleInstances)]
      public class MultipleInstancesScreen : DefaultTestScreen {
         public MultipleInstancesScreen(EventAggregator aggregator)
            : base(aggregator) {
         }
      }

      [ScreenCreationBehavior(ScreenCreationBehavior.SingleInstance)]
      public class SingleInstanceScreen : DefaultTestScreen {
         public SingleInstanceScreen(EventAggregator aggregator)
            : base(aggregator) {
         }
      }

      public class DefaultCreationBehaviorScreen : DefaultTestScreen {
         public DefaultCreationBehaviorScreen(EventAggregator aggregator)
            : base(aggregator) {
         }
      }

      [ScreenCreationBehavior(ScreenCreationBehavior.UseScreenLocation)]
      public class LocatableScreen :
         DefaultTestScreen,
         ILocatableScreen<BaseSubject> {

         public LocatableScreen(EventAggregator aggregator)
            : base(aggregator) {

            Lifecycle.RegisterHandler(
               ScreenEvents.Initialize<BaseSubject>(),
               args => Subject = args.Subject
            );
         }

         public BaseSubject Subject { get; set; }

         public bool PresentsSubject(BaseSubject subject) {
            return Subject.Value == subject.Value;
         }
      }

      public class DerivedLocatableScreen : LocatableScreen {
         public DerivedLocatableScreen(EventAggregator aggregator)
            : base(aggregator) {
         }
      }

      public class BaseSubject {
         public string Value { get; set; }

         public override int GetHashCode() {
            return 0;
         }

         public override bool Equals(object obj) {
            var other = obj as BaseSubject;
            return other != null && Object.Equals(Value, other.Value);
         }
      }

      public class DerivedSubject : BaseSubject {
      }
   }
}