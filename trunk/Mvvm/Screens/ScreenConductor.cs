namespace Inspiring.Mvvm.Screens {
   using System;
   using System.Collections.Generic;
   using System.Diagnostics.Contracts;
   using System.Linq;

   public class ScreenConductor : ScreenBase {
      private IScreen _activeScreen;
      private ScreenLifecycleCollection<IScreen> _screens;

      public ScreenConductor() {
         _screens = new ScreenLifecycleCollection<IScreen>(this);
      }

      public IScreen ActiveScreen {
         get { return _activeScreen; }
         set {
            if (value != _activeScreen) {
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
      }

      public IEnumerable<IScreen> Screens {
         get { return _screens.Items; }
      }

      public void OpenScreen<TScreen>(IScreenFactory<TScreen> screen)
         where TScreen : class, IScreen {

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
