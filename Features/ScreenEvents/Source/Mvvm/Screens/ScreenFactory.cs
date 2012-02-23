namespace Inspiring.Mvvm.Screens {
   using System;
   using System.Diagnostics.Contracts;
   using Inspiring.Mvvm.Common;

   public static partial class ScreenFactory {
      public static IScreenFactory<TScreen> For<TScreen>(IServiceLocator resolveWith = null) where TScreen : IScreenBase {
         return new Factory<TScreen>(resolveWith);
      }

      public static SubjectExpression<TSubject> WithSubject<TSubject>(TSubject subject) {
         return new SubjectExpression<TSubject>(subject);
      }

      public static IScreenFactory<TScreen> For<TScreen>(
         TScreen instance
      ) where TScreen : IScreenBase {
         Contract.Requires<ArgumentNullException>(instance != null);
         return new InstanceFactory<TScreen>(instance);
      }

      public class SubjectExpression<TSubject> {
         private TSubject _subject;

         public SubjectExpression(TSubject subject) {
            _subject = subject;
         }

         public IScreenFactory<TScreen> For<TScreen>(IServiceLocator resolveWith = null) where TScreen : IScreenBase {
            return new Factory<TScreen, TSubject>(_subject, resolveWith);
         }
      }

      private abstract class AbstractFactory<TScreen> where TScreen : IScreenBase {
         public TScreen Create(EventAggregator aggregator, Action<TScreen> preInitializationCallback = null) {
            TScreen screen = CreateScreen();

            if (preInitializationCallback != null) {
               preInitializationCallback(screen);
            }

            InitializeScreen(aggregator, screen);

            return screen;
         }

         protected abstract void InitializeScreen(EventAggregator aggregator, TScreen screen);

         protected abstract TScreen CreateScreen();
      }

      private abstract class ServiceLocatorFactory<TScreen> :
         AbstractFactory<TScreen>
         where TScreen : IScreenBase {

         private readonly IServiceLocator _serviceLocator;

         public ServiceLocatorFactory(IServiceLocator serviceLocator) {
            _serviceLocator = serviceLocator ?? ServiceLocator.Current;
         }

         public Type ScreenType {
            get { return typeof(TScreen); }
         }

         protected override TScreen CreateScreen() {
            return _serviceLocator.GetInstance<TScreen>();
         }
      }


      private class Factory<TScreen> :
         ServiceLocatorFactory<TScreen>,
         IScreenFactory<TScreen>
         where TScreen : IScreenBase {

         public Factory(IServiceLocator serviceLocator)
            : base(serviceLocator) {
         }

         public bool CreatesScreensEquivalentTo(IScreenBase concreteScreen) {
            return false;
         }

         protected override void InitializeScreen(EventAggregator aggregator, TScreen screen) {
            new ScreenLifecycleOperations(aggregator, screen)
               .Initialize();
         }
      }

      private class Factory<TScreen, TSubject> :
         ServiceLocatorFactory<TScreen>,
         IScreenFactory<TScreen>
         where TScreen : IScreenBase {

         private readonly TSubject _subject;

         public Factory(TSubject subject, IServiceLocator serviceLocator)
            : base(serviceLocator) {
            _subject = subject;
         }

         public bool CreatesScreensEquivalentTo(IScreenBase concreteScreen) {
            var locatableScreen = concreteScreen as ILocatableScreen<TSubject>;

            return locatableScreen != null ?
               locatableScreen.PresentsSubject(_subject) :
               false;
         }

         protected override void InitializeScreen(EventAggregator aggregator, TScreen screen) {
            new ScreenLifecycleOperations(aggregator, screen)
               .Initialize(_subject);
         }
      }

      private class InstanceFactory<TScreen> :
         AbstractFactory<TScreen>,
         IScreenFactory<TScreen>
         where TScreen : IScreenBase {

         private readonly TScreen _instance;

         public InstanceFactory(TScreen instance) {
            _instance = instance;
         }

         public Type ScreenType {
            get { return _instance.GetType(); }
         }

         public bool CreatesScreensEquivalentTo(IScreenBase concreteScreen) {
            return Object.Equals(concreteScreen, _instance);
         }

         protected override void InitializeScreen(EventAggregator aggregator, TScreen screen) {
            new ScreenLifecycleOperations(aggregator, screen)
               .Initialize();
         }

         protected override TScreen CreateScreen() {
            return _instance;
         }
      }
   }
}
