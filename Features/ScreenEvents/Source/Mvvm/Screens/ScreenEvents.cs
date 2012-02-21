namespace Inspiring.Mvvm.Screens {
   using Inspiring.Mvvm.Common;

   internal sealed class ScreenEvents {
      public static readonly ScreenEvent<ScreenEventArgs> Activate = new ScreenEvent<ScreenEventArgs>();
      public static readonly ScreenEvent<ScreenEventArgs> Deactivate = new ScreenEvent<ScreenEventArgs>();
      public static readonly ScreenEvent<RequestCloseEventArgs> RequestClose = new ScreenEvent<RequestCloseEventArgs>();
      public static readonly ScreenEvent<ScreenEventArgs> Close = new ScreenEvent<ScreenEventArgs>();
      public static readonly ScreenEvent<ScreenEventArgs> LifecycleExceptionOccured = new ScreenEvent<ScreenEventArgs>();

      private static readonly ScreenEvent<InitializeEventArgs> InitializeInstance = new ScreenEvent<InitializeEventArgs>();

      public static ScreenEvent<InitializeEventArgs> Initialize() {
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
