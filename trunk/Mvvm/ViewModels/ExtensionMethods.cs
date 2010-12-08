namespace Inspiring.Mvvm.ViewModels {
   using System;
   using Inspiring.Mvvm.ViewModels.Core;

   public static class ExtensionMethods {
      // TODO: Test me!
      public static void CallNext<T>(this T behavior, Action<T> methodSelector) where T : IBehavior {
         //behavior.TryCall<T>(methodSelector);
      }

      public static void CallNext2(this IBehaviorInitializationBehavior behavior, Action<IBehaviorInitializationBehavior> methodSelector) {
         //behavior.TryCall<T>(methodSelector);
      }

      public static void CallNext2(this IDisplayValueAccessorBehavior behavior, Action<IDisplayValueAccessorBehavior> methodSelector) {
         //behavior.TryCall<T>(methodSelector);
      }

      public static void InitializeNext(this Behavior behavior, BehaviorInitializationContext context) {
         IBehaviorInitializationBehavior next;
         if (behavior.TryGetBehavior<IBehaviorInitializationBehavior>(out next)) {
            next.Initialize(context);
         }
      }

      public static void SetDisplayValueNext(this Behavior behavior, IBehaviorContext context, object value) {
         //behavior.TryCall<T>(methodSelector);
      }

      public static TResult CallNext<T, TResult>(this T behavior, Func<T, TResult> methodSelector) where T : Behavior {
         T next = behavior.GetNextBehavior<T>();
         return methodSelector(next);
      }
   }
}
