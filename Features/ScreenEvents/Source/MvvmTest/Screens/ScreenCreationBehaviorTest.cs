namespace Inspiring.MvvmTest.Screens {
   using System;
   using System.Linq;
   using Inspiring.Mvvm.Common;
   using Inspiring.Mvvm.Screens;
   using Inspiring.MvvmTest.ViewModels;
   using Microsoft.VisualStudio.TestTools.UnitTesting;
   using Moq;

   [TestClass]
   public class ScreenCreationBehaviorTest : TestBase {
      [TestMethod]
      public void MultipleInstances() {
         ScreenConductor c = CreateConductor();
         c.OpenScreen(ScreenFactory.For<MultipleInstancesScreen>());
         c.OpenScreen(ScreenFactory.For<MultipleInstancesScreen>());
         Assert.AreEqual(2, c.Screens.Count());
      }

      [TestMethod]
      public void SingleInstance() {
         ScreenConductor c = CreateConductor();
         c.OpenScreen(ScreenFactory.For<SingleInstanceScreen>());
         c.OpenScreen(ScreenFactory.For<SingleInstanceScreen>());
         Assert.AreEqual(1, c.Screens.Count());
      }

      [TestMethod]
      public void DefaultCreationBehavior() {
         ScreenConductor c = CreateConductor();
         c.OpenScreen(ScreenFactory.For<DefaultCreationBehaviorScreen>());
         c.OpenScreen(ScreenFactory.For<DefaultCreationBehaviorScreen>());
         Assert.AreEqual(2, c.Screens.Count());
      }

      [TestMethod]
      public void OpenScreenUseScreenLocation_WhenNoMatchingScreenIsOpen_CreatesNewScreen() {
         var conductor = CreateConductor();
         var firstSubject = new BaseSubject { Value = "First Subject" };
         var differentSubject = new BaseSubject { Value = "Different Subject" };

         conductor.OpenScreen(
            ScreenFactory
               .For<MultipleInstancesScreen>()
         );

         conductor.OpenScreen(
            ScreenFactory
               .WithSubject(firstSubject)
               .For<LocatableScreen>()
         );

         conductor.OpenScreen(
            ScreenFactory
               .WithSubject(differentSubject)
               .For<LocatableScreen>()
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
               .For<LocatableScreen>()
         );

         conductor.OpenScreen(
            ScreenFactory
               .WithSubject(singleSubject)
               .For<LocatableScreen>()
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
               .For<DerivedLocatableScreen>()
         );

         conductor.OpenScreen(
            ScreenFactory
               .WithSubject(singleSubject)
               .For<DerivedLocatableScreen>()
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
               .For<DerivedLocatableScreen>()
         );

         IScreenFactory<IScreenBase> downcasted = ScreenFactory
            .WithSubject(singleSubject)
            .For<DerivedLocatableScreen>();

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
               .For<LocatableScreen>()
         );

         conductor.OpenScreen(
            ScreenFactory
               .WithSubject(singleSubject)
               .For<LocatableScreen>()
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
         return new ScreenConductor(new EventAggregator());
      }

      [ScreenCreationBehavior(ScreenCreationBehavior.MultipleInstances)]
      public class MultipleInstancesScreen : DefaultTestScreen {
      }

      [ScreenCreationBehavior(ScreenCreationBehavior.SingleInstance)]
      public class SingleInstanceScreen : DefaultTestScreen {
      }

      public class DefaultCreationBehaviorScreen : DefaultTestScreen {
      }

      [ScreenCreationBehavior(ScreenCreationBehavior.UseScreenLocation)]
      public class LocatableScreen :
         DefaultTestScreen,
         ILocatableScreen<BaseSubject>,
         INeedsInitialization<BaseSubject> {

         public BaseSubject Subject { get; set; }

         public bool PresentsSubject(BaseSubject subject) {
            return Subject.Value == subject.Value;
         }

         public void Initialize(BaseSubject subject) {
            Subject = subject;
         }
      }

      public class DerivedLocatableScreen : LocatableScreen { }

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