namespace Inspiring.Mvvm.Screens {
   using System;
   using System.Linq;
   using Inspiring.Mvvm.Common;

   internal sealed class DialogLifecycle : ScreenLifecycle {
      private readonly EventAggregator _aggregator;

      public DialogLifecycle(EventAggregator aggregator) {
         _aggregator = aggregator;
      }

      public DialogScreenResult ScreenResult { get; set; }

      public static DialogLifecycle GetDialogLifecycle(IScreenBase forScreen) {
         var lifecycle = TryGetDialogLifecycle(forScreen);

         if (lifecycle == null) {
            throw new ArgumentException(ExceptionTexts.ScreenIsNoDialog);
         }

         return lifecycle;
      }

      public static bool HasDialogLifecycle(IScreenBase forScreen) {
         return TryGetDialogLifecycle(forScreen) != null;
      }

      private static DialogLifecycle TryGetDialogLifecycle(IScreenBase forScreen) {
         return ScreenTreeHelper
            .GetAncestorsOf(forScreen)
            .SelectMany(x => x.Children.Items.OfType<DialogLifecycle>())
            .FirstOrDefault();
      }

      // TODO: Clean this up
      public void RaiseCloseWindow(IScreenBase screen) {
         ScreenHelper.Close(_aggregator, screen, true);
      }
   }
}
