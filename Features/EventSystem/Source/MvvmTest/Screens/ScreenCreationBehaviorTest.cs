namespace Inspiring.MvvmTest.Screens {
   using System;
   using System.Linq;
   using Inspiring.Mvvm.Screens;
   using Inspiring.MvvmTest.ViewModels;
   using Microsoft.VisualStudio.TestTools.UnitTesting;
   using Moq;

   [TestClass]
   public class ScreenCreationBehaviorTest : TestBase {
      [TestMethod]
      public void MultipleInstances() {
         ScreenConductor c = new ScreenConductor();
         c.OpenScreen(ScreenFactory.For<MultipleInstancesScreen>());
         c.OpenScreen(ScreenFactory.For<MultipleInstancesScreen>());
         Assert.AreEqual(2, c.Screens.Count());
      }

      [TestMethod]
      public void SingleInstance() {
         ScreenConductor c = new ScreenConductor();

         var firstFactoryMock = new Mock<IScreenFactory<SingleInstanceScreen>>(MockBehavior.Strict);
         var secondFactoryMock = new Mock<IScreenFactory<SingleInstanceScreen>>(MockBehavior.Strict);

         firstFactoryMock
            .Setup(x => x.Create(It.IsAny<Action<SingleInstanceScreen>>()))
            .Returns<Action<SingleInstanceScreen>>(init =>
               ScreenFactory.For<SingleInstanceScreen>().Create(init)
            );

         c.OpenScreen(firstFactoryMock.Object);
         c.OpenScreen(secondFactoryMock.Object);

         firstFactoryMock.Verify(
            x => x.Create(It.IsAny<Action<SingleInstanceScreen>>()),
            Times.Once()
         );

         Assert.AreEqual(1, c.Screens.Count());
      }

      [TestMethod]
      public void DefaultCreationBehavior() {
         ScreenConductor c = new ScreenConductor();
         c.OpenScreen(ScreenFactory.For<DefaultCreationBehaviorScreen>());
         c.OpenScreen(ScreenFactory.For<DefaultCreationBehaviorScreen>());
         Assert.AreEqual(2, c.Screens.Count());
      }

      [TestMethod]
      public void OpenScreenUseScreenLocation_WhenNoMatchingScreenIsOpen_CreatesNewScreen() {
         var conductor = new ScreenConductor();
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
         var conductor = new ScreenConductor();
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
      public void OpenScreenUseScreenLocation_WhenScreenImplementsLocatableScreenForBaseClass_ActivatesScreen() {
         var conductor = new ScreenConductor();
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

      [ScreenCreationBehavior(ScreenCreationBehavior.MultipleInstances)]
      public class MultipleInstancesScreen : ScreenBase {
      }

      [ScreenCreationBehavior(ScreenCreationBehavior.SingleInstance)]
      public class SingleInstanceScreen : ScreenBase {
      }

      public class DefaultCreationBehaviorScreen : ScreenBase {
      }

      [ScreenCreationBehavior(ScreenCreationBehavior.UseScreenLocation)]
      public class LocatableScreen :
         ScreenBase,
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