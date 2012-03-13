namespace Inspiring.Mvvm.Screens {
   using System;
   using Inspiring.Mvvm.Common;

   public sealed class ScreenEvents {
      public static readonly ScreenLifecycleEvent<ScreenEventArgs> Activate = new ScreenLifecycleEvent<ScreenEventArgs>("Activate");
      public static readonly ScreenLifecycleEvent<ScreenEventArgs> Deactivate = new ScreenLifecycleEvent<ScreenEventArgs>("Deactivate");
      public static readonly ScreenLifecycleEvent<RequestCloseEventArgs> RequestClose = new ScreenLifecycleEvent<RequestCloseEventArgs>("RequestClose");
      public static readonly ScreenLifecycleEvent<ScreenEventArgs> Close = new ScreenLifecycleEvent<ScreenEventArgs>("Close");
      public static readonly ScreenLifecycleEvent<ScreenEventArgs> LifecycleExceptionOccured = new ScreenLifecycleEvent<ScreenEventArgs>("LifecycleExceptionOccured");

      private static readonly ScreenLifecycleEvent<InitializeEventArgs> InitializeInstance = new ScreenLifecycleEvent<InitializeEventArgs>("Initialize");

      public static ScreenLifecycleEvent<InitializeEventArgs> Initialize() {
         return InitializeInstance;
      }

      public static ScreenLifecycleEvent<InitializeEventArgs<TSubject>> Initialize<TSubject>() {
         return GenericInitializeSingleton<TSubject>.Instance;
      }

      private static class GenericInitializeSingleton<TSubject> {
         public static readonly ScreenLifecycleEvent<InitializeEventArgs<TSubject>> Instance =
            new ScreenLifecycleEvent<InitializeEventArgs<TSubject>>(
               String.Format("Initialize<{0}>", TypeService.GetFriendlyName(typeof(TSubject)))
            );
      }
   }

   public class ScreenEventArgs : HierarchicalEventArgs<IScreenBase> {
      public ScreenEventArgs(IScreenBase target)
         : base(target) {
      }
   }

   public sealed class RequestCloseEventArgs : ScreenEventArgs {
      public RequestCloseEventArgs(IScreenBase target)
         : base(target) {
         IsCloseAllowed = true;
      }

      public bool IsCloseAllowed { get; set; }
   }
}
