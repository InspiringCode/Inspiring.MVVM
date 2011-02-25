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

            if (needsTypedInitialization != null) {
               needsTypedInitialization.Initialize(subject);
            } else if (needsInitialization != null) {
               needsInitialization.Initialize();
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
   }
}
