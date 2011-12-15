namespace Inspiring.Mvvm.Screens {
   using System;

   // TODO: Temporary solution. Refactor (see WindowService)!
   internal sealed class ScreenService {
      public TScreen CreateAndActivateScreen<TScreen>(
         IScreenFactory<TScreen> factory,
         Action<TScreen> initializationCallback = null
      ) where TScreen : IScreenBase {
         TScreen screen = default(TScreen);

         screen = factory.Create(initializationCallback);
         screen.Activate();
         return screen;
      }

      public void DeactivateAndCloseScreen(IScreenBase screen) {
         screen.Deactivate();
         screen.Close();
      }
   }
}
