namespace Inspiring.Mvvm.Screens {
   using System;
   using System.Collections.Generic;
   using System.Diagnostics.Contracts;
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
         Contract.Requires(subject != null);

         var typed = screen as INeedsInitialization<TSubject>;

         if (typed != null) {
            typed.Initialize(subject);
            return true;
         }

         var interfaceImpl = GetCompatibleInitializationInterfaces(screen.GetType(), subject.GetType())
            .FirstOrDefault();

         if (interfaceImpl != null) {
            interfaceImpl
               .GetMethod("Initialize")
               .Invoke(screen, new object[] { subject });

            return true;
         }

         return false;
      }

      private static IEnumerable<Type> GetCompatibleInitializationInterfaces(Type screenType, Type subjectType) {
         var canditateSubjectTypes = GetBaseTypesAndInterfacesOf(subjectType);
         return canditateSubjectTypes
            .Select(c => typeof(INeedsInitialization<>).MakeGenericType(c))
            .Where(itf => itf.IsAssignableFrom(screenType));
      }

      private static IEnumerable<Type> GetBaseTypesAndInterfacesOf(Type type) {
         for (Type t = type.BaseType; t != null && t != typeof(Object); t = t.BaseType) {
            yield return t;
         }

         foreach (Type itf in type.GetInterfaces()) {
            yield return itf;
         }

         yield return typeof(Object);
      }
   }
}
