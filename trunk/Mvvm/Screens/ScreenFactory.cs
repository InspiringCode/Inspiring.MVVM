namespace Inspiring.Mvvm.Screens {

   public static class ScreenFactory {
      public static IScreenFactory<TScreen> For<TScreen>() where TScreen : IScreen {
         return new Factory<TScreen>();
      }

      public static SubjectExpression<TSubject> WithSubject<TSubject>(TSubject subject) {
         return new SubjectExpression<TSubject>(subject);
      }

      public class SubjectExpression<TSubject> {
         private TSubject _subject;

         public SubjectExpression(TSubject subject) {
            _subject = subject;
         }

         public IScreenFactory<TScreen> For<TScreen>() where TScreen : IScreen<TSubject> {
            return new Factory<TScreen, TSubject>(_subject);
         }
      }

      private class Factory<TScreen> : IScreenFactory<TScreen> where TScreen : IScreen {
         public TScreen Create(IScreenInitializer initializer) {
            TScreen screen = ServiceLocator.Current.GetInstance<TScreen>();

            if (initializer != null) {
               initializer.Initialize(screen);
            }

            return screen;
         }
      }

      private class Factory<TScreen, TSubject> : IScreenFactory<TScreen> where TScreen : IScreen<TSubject> {
         private TSubject _subject;

         public Factory(TSubject subject) {
            _subject = subject;
         }

         public TScreen Create(IScreenInitializer initializer) {
            TScreen screen = ServiceLocator.Current.GetInstance<TScreen>();

            if (initializer != null) {
               initializer.Initialize(screen, _subject);
            }

            return screen;
         }
      }
   }
}
