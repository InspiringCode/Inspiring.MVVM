namespace Inspiring.Mvvm.Screens {
   using System;
   using System.Linq;

   public static class ScreenInitializer {
      public static void Initialize(IScreenLifecycle screen) {
         Initialize(screen, s => {
            INeedsInitialization needsInitialization = s as INeedsInitialization;
            if (needsInitialization != null) {
               needsInitialization.Initialize();
            }
         });
      }

      public static void Initialize<TSubject>(
         IScreenLifecycle screen,
         TSubject subject
      ) {
         Initialize(screen, s => {
            INeedsInitialization<TSubject> needsTypedInitialization = s as INeedsInitialization<TSubject>;
            INeedsInitialization needsInitialization = s as INeedsInitialization;

            if (!TryTypedInitialize(s, subject)) {
               if (needsInitialization != null) {
                  needsInitialization.Initialize();
               }
            }
         });
      }

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

      private static bool TryTypedInitialize<TSubject>(IScreenLifecycle screen, TSubject subject) {
         var typed = screen as INeedsInitialization<TSubject>;

         if (typed != null) {
            typed.Initialize(subject);
            return true;
         }

         for (Type t = typeof(TSubject).BaseType; t != null; t = t.BaseType) {
            Type itf = typeof(INeedsInitialization<>).MakeGenericType(t);
            if (itf.IsAssignableFrom(screen.GetType())) {
               itf
                  .GetMethod("Initialize")
                  .Invoke(screen, new object[] { subject });

               return true;
            }
         }

         return false;
      }
   }
}
