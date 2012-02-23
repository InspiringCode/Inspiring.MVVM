namespace Inspiring.Mvvm.Screens {
   using Inspiring.Mvvm.Common;

   internal static class ScreenHelper {
      public static void Close(EventAggregator aggregator, IScreenBase screen, bool skipRequestClose) {
         aggregator.Publish(
            ScreenEvents.InitiateClose,
            new InitiateCloseEventArgs(screen, skipRequestClose)
         );
      }
   }
}
