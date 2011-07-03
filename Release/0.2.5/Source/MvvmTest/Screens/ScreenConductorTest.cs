using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Inspiring.MvvmTest.Screens {
   [TestClass]
   public class ScreenConductorTest {
      [TestMethod]
      public void TestMethod1() {
         //ScreenConductor cd = new ScreenConductor();

         //var firstScreen = new Mock<ILifecycleHandler>(MockBehavior.Strict);
         //firstScreen.Setup(x => x.Initialize());
         //firstScreen.Setup(x => x.Activate());

         //var firstFactory = MockObjectFactory.MockScreenFactory(firstScreen.Object);

         //cd.OpenScreen(firstFactory.Object);

         //Assert.AreEqual(firstScreen.Object, cd.ActiveScreen);

         //firstFactory.Verify(x => x.Create(It.IsAny<IScreenInitializer>()), Times.Once());
         //firstScreen.Verify(x => x.Initialize(), Times.Once());
         //firstScreen.Verify(x => x.Activate(), Times.Once());

         //var secondScreen = new Mock<IScreen<string>>(MockBehavior.Strict);
         //secondScreen.Setup(x => x.Initialize());
         //secondScreen.Setup(x => x.Initialize("Test"));
         //secondScreen.Setup(x => x.Activate());

         //var secondFactory = MockObjectFactory.MockScreenFactory(secondScreen.Object, "Test");

         //firstScreen.Setup(x => x.Deactivate());

         //cd.OpenScreen(secondFactory.Object);
         //Assert.AreEqual(secondScreen.Object, cd.ActiveScreen);

         //secondFactory.Verify(x => x.Create(It.IsAny<IScreenInitializer>()), Times.Once());
         //secondScreen.Verify(x => x.Initialize(), Times.Once());
         //secondScreen.Verify(x => x.Initialize("Test"), Times.Once());
         //secondScreen.Verify(x => x.Activate(), Times.Once());
         //firstScreen.Verify(x => x.Deactivate());
      }



      [TestMethod]
      public void MockTest() {
         //var screen = new Mock<ILifecycleHandler>();


         //screen.Object.Initialize();
         //screen.Object.Activate();
         //screen.Object.Deactivate();

         //screen.Verify();
      }

      //private class Test : IScreenInitializer {

      //   public void Initialize(ILifecycleHandler screen) {

      //   }

      //   public void Initialize<TSubject>(IScreen<TSubject> screen, TSubject subject) {

      //   }
      //}

   }
}
