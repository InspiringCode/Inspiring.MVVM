using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
namespace Inspiring.Mvvm.Screens {

   public class ScreenConductor : Screen<ScreenConductorSubject>, IIdentifiedScreen {
      private IScreen _activeScreen;
      private ScreenCollection _screens;
      private object _conductorId;

      public ScreenConductor() {
         _screens = new ScreenCollection(this);
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
         get { return _screens.Screens; }
      }

      object IIdentifiedScreen.ScreenId {
         get { return _conductorId; }
      }

      public void OpenScreen<TScreen>(IScreenFactory<TScreen> screen) where TScreen : IScreen {
         ActiveScreen = _screens.AddNew(screen);
      }

      public void CloseScreen(IScreen screen) {
         if (!_screens.Screens.Contains(screen)) {
            throw new ArgumentException(ExceptionTexts.ScreenNotContainedByConductor);
         }

         if (screen.RequestClose()) {
            IScreen next = ChooseNextScreen(screen);
            ActiveScreen = next;
            screen.Close();
            _screens.Screens.Remove(screen);
         }
      }

      protected virtual IScreen ChooseNextScreen(IScreen screen) {
         if (_screens.Screens.Count == 1) {
            return null;
         }

         int index = _screens.Screens.IndexOf(screen);
         Contract.Assert(index != -1);

         return _screens.Screens[(index + 1) % _screens.Screens.Count];
      }

      protected override void OnInitialize(ScreenConductorSubject subject) {
         _conductorId = subject.ConductorId;
      }

      protected override bool OnRequestClose() {
         return _screens.RequestClose();
      }

      protected override void OnClose() {
         _screens.Close();
      }
   }

   public class ScreenConductorSubject {
      public ScreenConductorSubject(object conductorId) {
         ConductorId = conductorId;
      }

      public object ConductorId { get; private set; }
   }
}
