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

         try {
            screen.Activate();
         } catch (Exception) {
            screen.Close();
            throw;
         }

         return screen;
      }

      public void DeactivateAndCloseScreen(IScreenBase screen) {
         try {
            screen.Deactivate();
         } finally {
            screen.Close();
         }
      }
   }
}
