namespace Inspiring.Mvvm.Screens {
   using System;
   using System.Collections.Generic;
   using System.Diagnostics.Contracts;
   using System.Linq;

   public class ScreenConductor : ScreenBase {
      private IScreenBase _activeScreen;
      private ScreenLifecycleCollection<IScreenBase> _screens;
      private bool _isActivated;

      public ScreenConductor() {
         _screens = new ScreenLifecycleCollection<IScreenBase>(this);
      }

      public IScreenBase ActiveScreen {
         get { return _activeScreen; }
         set {
            if (value != _activeScreen) {
               if (_activeScreen != null && _isActivated) {
                  _activeScreen.Deactivate();
               }

               _activeScreen = value;

               if (_activeScreen != null && _isActivated) {
                  _activeScreen.Activate();
               }

               OnPropertyChanged(() => ActiveScreen);
            }
         }
      }

      public IEnumerable<IScreenBase> Screens {
         get { return _screens.Items; }
      }

      // TODO: Maybe set the ScreenConductor as Opener of the new Screen.
      public void OpenScreen<TScreen>(IScreenFactory<TScreen> screen)
         where TScreen : class, IScreenBase {

         var creationBehavior = GetCreationBehavior(typeof(TScreen));

         switch (creationBehavior) {
            case ScreenCreationBehavior.MultipleInstances:
               ActiveScreen = _screens.AddNew(screen);
               break;
            case ScreenCreationBehavior.SingleInstance:
               var alreadyOpenScreen = _screens.Items
                  .OfType<TScreen>()
                  .SingleOrDefault();

               ActiveScreen = alreadyOpenScreen ?? _screens.AddNew(screen);
               break;
         }
      }

      public bool CloseScreen(IScreenBase screen) {
         if (!_screens.Items.Contains(screen)) {
            throw new ArgumentException(ExceptionTexts.ScreenNotContainedByConductor);
         }

         if (screen.RequestClose()) {
            ImmediateCloseScreen(screen);
            return true;
         }

         return false;
      }

      public void ImmediateCloseScreen(IScreenBase screen) {
         IScreenBase next = ChooseNextScreen(screen);
         ActiveScreen = next;

         // It is important to FIRST remove the screen and THEN call 'Close'. The
         // removal triggers a collection change which causes the view reprenstation
         // to close the view. In this stage the screen may still be accessed by the
         // view. If 'Close' was called before, the screen may already be in an
         // disposed state (e.g. database session closed).
         _screens.Items.Remove(screen);

         screen.Close();
      }

      protected virtual IScreenBase ChooseNextScreen(IScreenBase screen) {
         if (_screens.Items.Count == 1) {
            return null;
         }

         int index = _screens.Items.IndexOf(screen);
         Contract.Assert(index != -1);

         return _screens.Items[(index + 1) % _screens.Items.Count];
      }

      protected override void OnActivate() {
         _isActivated = true;
         if (_activeScreen != null) {
            _activeScreen.Activate();
         }
      }

      protected override void OnDeactivate() {
         _isActivated = false;
         if (_activeScreen != null) {
            _activeScreen.Deactivate();
         }
      }

      protected override bool OnRequestClose() {
         return _screens.RequestCloseAll();
      }

      protected override void OnClose() {
         while (_screens.Items.Any()) {
            ImmediateCloseScreen(_screens.Items.Last());
         }
      }

      private ScreenCreationBehavior GetCreationBehavior(Type screenType) {
         var attr = (ScreenCreationBehaviorAttribute)Attribute.GetCustomAttribute(
            screenType,
            typeof(ScreenCreationBehaviorAttribute)
         );

         return attr != null ?
            attr.CreationBehavior :
            ScreenCreationBehavior.MultipleInstances;
      }
   }
}
