namespace Inspiring.Mvvm.Screens {
   using Inspiring.Mvvm.Common;
   using System;

   internal sealed class ScreenEvents {
      public static readonly ScreenEvent<ScreenEventArgs> Activate = new ScreenEvent<ScreenEventArgs>("Activate");
      public static readonly ScreenEvent<ScreenEventArgs> Deactivate = new ScreenEvent<ScreenEventArgs>("Deactivate");
      public static readonly ScreenEvent<RequestCloseEventArgs> RequestClose = new ScreenEvent<RequestCloseEventArgs>("RequestClose");
      public static readonly ScreenEvent<ScreenEventArgs> Close = new ScreenEvent<ScreenEventArgs>("Close");
      public static readonly ScreenEvent<ScreenEventArgs> LifecycleExceptionOccured = new ScreenEvent<ScreenEventArgs>("LifecycleExceptionOccured");

      private static readonly ScreenEvent<InitializeEventArgs> InitializeInstance = new ScreenEvent<InitializeEventArgs>("Initialize");

      public static ScreenEvent<InitializeEventArgs> Initialize() {
         return InitializeInstance;
      }

      public static ScreenEvent<InitializeEventArgs<TSubject>> Initialize<TSubject>() {
         return GenericInitializeSingleton<TSubject>.Instance;
      }

      private static class GenericInitializeSingleton<TSubject> {
         public static readonly ScreenEvent<InitializeEventArgs<TSubject>> Instance =
            new ScreenEvent<InitializeEventArgs<TSubject>>(
               String.Format("Initialize<{0}>", TypeService.GetFriendlyName(typeof(TSubject)))
            );
      }
   }

   internal class InitiateCloseEventArgs : ScreenEventArgs {
      public InitiateCloseEventArgs(IScreenBase target, bool skipRequestClose)
         : base(target) {
         SkipRequestClose = skipRequestClose;
      }

      public bool SkipRequestClose { get; private set; }

      public bool Handled { get; set; }
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
