namespace Inspiring.MvvmTest.Screens {
   using System;
   using System.Linq;
   using Inspiring.Mvvm.Screens;
   using Microsoft.VisualStudio.TestTools.UnitTesting;
   using Moq;

   [TestClass]
   public class ScreenCreationBehaviorTest {
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

      [ScreenCreationBehavior(ScreenCreationBehavior.MultipleInstances)]
      public class MultipleInstancesScreen : ScreenBase {
      }

      [ScreenCreationBehavior(ScreenCreationBehavior.SingleInstance)]
      public class SingleInstanceScreen : ScreenBase {
      }

      public class DefaultCreationBehaviorScreen : ScreenBase {
      }
   }
}