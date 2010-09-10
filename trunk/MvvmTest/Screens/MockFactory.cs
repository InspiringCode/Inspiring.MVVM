namespace Inspiring.MvvmTest.Screens {
   using Inspiring.Mvvm.Screens;
   using Inspiring.Mvvm.Views;
   using Moq;

   internal sealed class MockObjectFactory {
      public static Mock<IScreenFactory<TScreen>> MockScreenFactory<TScreen, TSubject>(TScreen screen, TSubject subject)
      where TScreen : IScreen {
         //var factory = new Mock<IScreenFactory<TScreen>>();
         //factory
         //   .Setup(x => x.Create(It.IsAny<IScreenInitializer>()))
         //   .Callback<IScreenInitializer>(x => x.Initialize<TSubject>(screen, subject))
         //   .Returns(screen);
         //return factory;
         return null;
      }

      public static Mock<IScreenFactory<TScreen>> MockScreenFactory<TScreen>(TScreen screen)
         where TScreen : IScreen {
         //var factory = new Mock<IScreenFactory<TScreen>>();
         //factory
         //   .Setup(x => x.Create(It.IsAny<IScreenInitializer>()))
         //   .Callback<IScreenInitializer>(x => x.Initialize(screen))
         //   .Returns(screen);
         //return factory;
         return null;
      }

      public static IScreen MockScreen() {
         var mock = new Mock<IScreen>();
         mock.Setup(x => x.RequestClose()).Returns(true);
         return mock.Object;
      }

      public static IView<object> MockView() {
         return new Mock<IView<object>>().Object;
      }
   }
}
