namespace Inspiring.Mvvm.Screens {
   using System;
   using System.Linq;

   public static partial class ScreenFactory {
      public static IScreenFactory<TScreen> For<TScreen>(IServiceLocator resolveWith = null) where TScreen : IScreen {
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

         public IScreenFactory<TScreen> For<TScreen>(IServiceLocator resolveWith = null) where TScreen : IScreen {
            return new Factory<TScreen, TSubject>(_subject, resolveWith);
         }
      }
   }

   partial class ScreenFactory {
      private static void Initialize(
         IScreenLifecycle handler,
         Action<IScreenLifecycle> initializer
      ) {
         Initialize(handler, initializer, InvocationOrder.First);
         Initialize(handler, initializer, InvocationOrder.BeforeParent);
         Initialize(handler, initializer, InvocationOrder.Parent);
         Initialize(handler, initializer, InvocationOrder.AfterParent);
         Initialize(handler, initializer, InvocationOrder.Last);
      }

      private static void Initialize(
         IScreenLifecycle handler,
         Action<IScreenLifecycle> initializer,
         InvocationOrder order
      ) {
         // TODO: Handling for multiple Initialize methods...
         LifecycleTreeWalker
            .GetSelfAndChildren(handler) // instead of GetDescendants. HACK: Rethink initialization logic!
            .Where(c => InvocationOrderAttribute.GetOrder(c, "Initialize") == order)
            .ForEach(initializer);
      }

      private class Factory<TScreen> : IScreenFactory<TScreen> where TScreen : IScreen {
         private IServiceLocator _resolveWith;

         public Factory(IServiceLocator resolveWith) {
            _resolveWith = resolveWith ?? ServiceLocator.Current;
         }

         public TScreen Create(Action<TScreen> initializationCallback = null) {
            TScreen screen = _resolveWith.GetInstance<TScreen>();

            if (initializationCallback != null) {
               initializationCallback(screen);
            }


            Initialize(screen, s => {
               INeedsInitialization needsInitialization = s as INeedsInitialization;
               if (needsInitialization != null) {
                  needsInitialization.Initialize();
               }
            });

            return screen;
         }
      }

      private class Factory<TScreen, TSubject> : IScreenFactory<TScreen> where TScreen : IScreen {
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

            Initialize(screen, s => {
               INeedsInitialization<TSubject> needsTypedInitialization = s as INeedsInitialization<TSubject>;
               INeedsInitialization needsInitialization = s as INeedsInitialization;

               if (needsTypedInitialization != null) {
                  needsTypedInitialization.Initialize(_subject);
               } else if (needsInitialization != null) {
                  needsInitialization.Initialize();
               }
            });

            return screen;
         }
      }
   }
}
