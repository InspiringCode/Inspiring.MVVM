namespace Inspiring.Mvvm.Screens {
   using System;
   using System.Collections.Generic;
   using System.Linq;

   public class ScreenConductor : ScreenBase {
      private IScreenBase _activeScreen;
      private ScreenLifecycleCollection<IScreenBase> _screens;
      private IList<IScreenBase> _activatedScreensHistory;
      private bool _isActivated;

      public ScreenConductor() {
         _screens = new ScreenLifecycleCollection<IScreenBase>(this);
         _activatedScreensHistory = new List<IScreenBase>();
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

               if (_activeScreen != null) {
                  _activatedScreensHistory.Remove(_activeScreen);
                  _activatedScreensHistory.Add(_activeScreen);
               }
               
               OnPropertyChanged(() => ActiveScreen);
            }
         }
      }

      public IEnumerable<IScreenBase> Screens {
         get { return _screens.Items; }
      }

      // TODO: Maybe set the ScreenConductor as Opener of the new Screen.
      public void OpenScreen<TScreen>(IScreenFactory<TScreen> factory)
         where TScreen : class, IScreenBase {

         var creationBehavior = GetCreationBehavior(typeof(TScreen));

         IScreenBase alreadyOpenScreen = null;

         switch (creationBehavior) {
            case ScreenCreationBehavior.MultipleInstances:
               break;
            case ScreenCreationBehavior.SingleInstance:
               alreadyOpenScreen = Screens
                  .OfType<TScreen>()
                  .SingleOrDefault();
               break;
            case ScreenCreationBehavior.UseScreenLocation:
               alreadyOpenScreen = Screens
                  .FirstOrDefault(s => factory.CreatesScreensEquivalentTo(s));
               break;
         }

         ActiveScreen = alreadyOpenScreen ?? _screens.AddNew(factory);
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
         _activatedScreensHistory.Remove(screen);
         ActiveScreen = _activatedScreensHistory.LastOrDefault();

         // It is important to FIRST remove the screen and THEN call 'Close'. The
         // removal triggers a collection change which causes the view reprenstation
         // to close the view. In this stage the screen may still be accessed by the
         // view. If 'Close' was called before, the screen may already be in an
         // disposed state (e.g. database session closed).
         _screens.Items.Remove(screen);

         screen.Close();
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
