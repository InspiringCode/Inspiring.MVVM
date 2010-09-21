using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
namespace Inspiring.Mvvm.Screens {

   public class ScreenConductor : ScreenBase {
      private IScreen _activeScreen;
      private ScreenLifecycleCollection<IScreen> _screens;

      public ScreenConductor() {
         _screens = new ScreenLifecycleCollection<IScreen>(this);
      }

      public IScreen ActiveScreen {
         get { return _activeScreen; }
         set {
            if (_activeScreen != null) {
               _activeScreen.Deactivate();
            }

            _activeScreen = value;

            if (_activeScreen != null) {
               _activeScreen.Activate();
            }

            OnPropertyChanged(() => ActiveScreen);
         }
      }

      public IEnumerable<IScreen> Screens {
         get { return _screens.Items; }
      }

      public void OpenScreen<TScreen>(IScreenFactory<TScreen> screen) where TScreen : IScreen {
         ActiveScreen = _screens.AddNew(screen);
      }

      public void CloseScreen(IScreen screen) {
         if (!_screens.Items.Contains(screen)) {
            throw new ArgumentException(ExceptionTexts.ScreenNotContainedByConductor);
         }

         if (screen.RequestClose()) {
            IScreen next = ChooseNextScreen(screen);
            ActiveScreen = next;
            screen.Close();
            _screens.Items.Remove(screen);
         }
      }

      protected virtual IScreen ChooseNextScreen(IScreen screen) {
         if (_screens.Items.Count == 1) {
            return null;
         }

         int index = _screens.Items.IndexOf(screen);
         Contract.Assert(index != -1);

         return _screens.Items[(index + 1) % _screens.Items.Count];
      }

      protected override bool OnRequestClose() {
         return _screens.RequestCloseAll();
      }

      protected override void OnClose() {
         _screens.CloseAll();
      }
   }
}
