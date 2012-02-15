namespace Inspiring.Mvvm.Screens {
   using System;

   internal sealed class ScreenEvents {
      public static readonly ScreenEvent<ScreenEventArgs_> Activate = new ScreenEvent<ScreenEventArgs_>();

      private static readonly ScreenEvent<ScreenEventArgs_> InitializeInstance = new ScreenEvent<ScreenEventArgs_>();

      public static ScreenEvent<ScreenEventArgs_> Initialize() {
         return InitializeInstance;
      }

      public static ScreenEvent<InitializeEventArgs<TSubject>> Initialize<TSubject>() {
         return GenericInitializeSingleton<TSubject>.Instance;
      }

      private static class GenericInitializeSingleton<TSubject> {
         public static readonly ScreenEvent<InitializeEventArgs<TSubject>> Instance =
            new ScreenEvent<InitializeEventArgs<TSubject>>();
      }
   }

   public class ScreenEventArgs_ : EventArgs {
      public IScreenLifecycle Target { get; private set; }
   }

   public class InitializeEventArgs<TSubject> : ScreenEventArgs_ {
      public TSubject Subject { get; private set; }
   }
}
