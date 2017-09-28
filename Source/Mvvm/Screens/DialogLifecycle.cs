namespace Inspiring.Mvvm.Screens {
   using System;
   using System.Linq;

   internal sealed class DialogLifecycle {
      private readonly IScreenBase _parent;

      public DialogLifecycle(IScreenBase parent) {
         Check.NotNull(parent, nameof(parent));
         _parent = parent;
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
            .GetAncestorsOf(forScreen, includeSelf: true)
            .SelectMany(x => x.Children.OfType<DialogLifecycle>())
            .FirstOrDefault();
      }
   }
}
