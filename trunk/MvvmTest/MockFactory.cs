namespace Inspiring.MvvmTest {
   using Inspiring.Mvvm;
   using Inspiring.Mvvm.Screens;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
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

      public static IBehavior Behavior() {
         return new Mock<IBehavior>().Object;
      }

      public static Mock<IBehaviorFactory> BehaviorFactory<TValue>(IBehavior behavior) {
         var mock = new Mock<IBehaviorFactory>();
         mock.Setup(x => x.Create<TValue>()).Returns(behavior);
         return mock;
      }

      public static Mock<IBehaviorContext> MockBehaviorContext(
         VMDescriptor descriptor
      ) {
         IViewModel vm = new Mock<IViewModel>(ServiceLocator.Current).Object;
         var behaviorContext = new Mock<IBehaviorContext>();
         behaviorContext.Setup(x => x.FieldValues).Returns(descriptor.GetService<FieldDefinitionCollection>().CreateValueHolder());
         behaviorContext.Setup(x => x.VM).Returns(vm);
         return behaviorContext;
      }

      public static VMProperty<T> MockProperty<T>(BehaviorConfiguration config, VMDescriptor descriptor) {
         VMProperty<T> property = new VMProperty<T>();
         property.Initialize("Test", descriptor);
         property.ConfigureBehaviors(config, descriptor);
         return property;
      }
   }
}
