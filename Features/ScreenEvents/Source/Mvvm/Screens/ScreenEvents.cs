namespace Inspiring.Mvvm.Screens {
   using System;

   internal sealed class ScreenEvents {
      public static readonly ScreenEvent<ScreenEventArgs> Activate = new ScreenEvent<ScreenEventArgs>();

      private static readonly ScreenEvent<ScreenEventArgs> InitializeInstance = new ScreenEvent<ScreenEventArgs>();

      public static ScreenEvent<ScreenEventArgs> Initialize() {
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

   public class ScreenEventArgs : EventArgs {
      public IScreenLifecycle Target { get; private set; }
   }

   public class InitializeEventArgs<TSubject> : ScreenEventArgs {
      public TSubject Subject { get; private set; }
   }
}
