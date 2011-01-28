namespace Inspiring.Mvvm.Screens {
   using System;
   using System.Linq;

   public static partial class ScreenFactory {
      public static IScreenFactory<TScreen> For<TScreen>(IServiceLocator resolveWith = null) where TScreen : IScreenBase {
         return new Factory<TScreen>(resolveWith);
      }

      public static SubjectExpression<TSubject> WithSubject<TSubject>(TSubject subject) {
         return new SubjectExpression<TSubject>(subject);
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

      private class Factory<TScreen> : IScreenFactory<TScreen> where TScreen : IScreenBase {
         private IServiceLocator _resolveWith;

         public Factory(IServiceLocator resolveWith) {
            _resolveWith = resolveWith ?? ServiceLocator.Current;
         }

         public TScreen Create(Action<TScreen> initializationCallback = null) {
            TScreen screen = _resolveWith.GetInstance<TScreen>();

            if (initializationCallback != null) {
               initializationCallback(screen);
            }

            ScreenInitializer.Initialize(screen);
            return screen;
         }
      }

      private class Factory<TScreen, TSubject> : IScreenFactory<TScreen> where TScreen : IScreenBase {
         private TSubject _subject;
         private IServiceLocator _resolveWith;

         public Factory(TSubject subject, IServiceLocator resolveWith) {
            _subject = subject;
            _resolveWith = resolveWith ?? ServiceLocator.Current;
         }

         public TScreen Create(Action<TScreen> initializationCallback = null) {
            TScreen screen = _resolveWith.GetInstance<TScreen>();

            if (initializationCallback != null) {
               initializationCallback(screen);
            }

            ScreenInitializer.Initialize(screen, _subject);
            return screen;
         }
      }
   }
}
