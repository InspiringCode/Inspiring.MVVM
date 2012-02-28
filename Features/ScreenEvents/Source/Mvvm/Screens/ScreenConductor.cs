namespace Inspiring.Mvvm.Screens {
   using System;
   using System.Collections.Generic;
   using System.ComponentModel;
   using System.Diagnostics.Contracts;
   using System.Linq;
   using Inspiring.Mvvm.Common;

   public class ConductorEventArgs : EventArgs {
      internal ConductorEventArgs(
         ScreenConductor conductor,
         IScreenBase screen
      ) {
         Contract.Requires(conductor != null);
         Contract.Requires(screen != null);

         Conductor = conductor;
         Screen = screen;
      }

      public ScreenConductor Conductor { get; private set; }
      public IScreenBase Screen { get; private set; }
   }

   public sealed class ScreenOpenedEventArgs : ConductorEventArgs {
      internal ScreenOpenedEventArgs(
         ScreenConductor conductor,
         IScreenBase screen,
         bool wasAlreadyOpen
      )
         : base(conductor, screen) {
         WasAlreadyOpen = wasAlreadyOpen;
      }

      public bool WasAlreadyOpen { get; private set; }
   }

   public class ScreenConductor : ScreenBase, INotifyPropertyChanged {
      /// <summary>
      ///   An event that is raised whenever <see cref="OpenScreen{TScreen}"/> is called.
      /// </summary>
      /// <remarks>
      ///   This event may for example be handled by a view to restore already opened
      ///   MDI windows that are currently minimized.
      /// </remarks>
      public static readonly Event<ScreenOpenedEventArgs> ScreenOpenedEvent = new Event<ScreenOpenedEventArgs>();

      /// <summary>
      ///   An event that is raised whenever <see cref="CloseScreen{TScreen}"/> is called.
      /// </summary>
      /// <remarks>
      ///   This event may for example be handled by a view to save view settings.
      /// </remarks>
      public static readonly Event<ConductorEventArgs> ScreenClosedEvent = new Event<ConductorEventArgs>();

      private readonly List<IScreenBase> _activatedScreensHistory = new List<IScreenBase>();
      private readonly ScreenChildrenCollection<IScreenBase> _screens;
      private readonly EventAggregator _eventAggregator;

      private IScreenBase _activeScreen;
      private bool _updatingActiveScreen;

      public ScreenConductor(EventAggregator eventAggregator)
         : base(eventAggregator) {

         Contract.Requires<ArgumentNullException>(eventAggregator != null);

         _screens = new ScreenChildrenCollection<IScreenBase>(eventAggregator, this);
         _eventAggregator = eventAggregator;

         Lifecycle.RegisterHandler(ScreenEvents.Activate, HandleActivate);
         Lifecycle.RegisterHandler(ScreenEvents.Deactivate, HandleDeactivate);
         Lifecycle.RegisterHandler(ScreenEvents.RequestClose, HandleRequestClose);
         Lifecycle.RegisterHandler(ScreenEvents.Close, HandleClose);
      }

      public IScreenBase ActiveScreen {
         get { return _activeScreen; }
         set {
            IScreenBase oldActive = _activeScreen;
            IScreenBase newActive = value;

            if (newActive == oldActive) {
               return;
            }

            try {
               // When activation/deactivate fails, the failing screen is removed from
               // the 'Screens' collection WHILE we are updating the 'ActiveScreen'.
               // A client may handle the 'CollectionChanged' event and try to update
               // the 'ActiveScreen' property again which may lead to a corrupt state.
               // We just ignore this second set operation and always raise 'PropertyChanged'
               // event so the UI is guaranteed to be updated.
               //
               // An alternative approach would be to remove the failed screens at the end
               // of this method, but this could lead to scenarios, where the UI tries to
               // set the 'ActiveScreen' property to a failed screen.
               if (_updatingActiveScreen) {
                  return;
               }

               _updatingActiveScreen = true;

               // If the following code fails to activate the new screen, 'ActiveScreen'
               // is set to null.
               _activeScreen = null;

               try {
                  try {
                     // If the deactivation fails, the exception is propagated to the 
                     // caller but we still try to activate the new screen and update 
                     // the 'ActivateScreen'.
                     DeactivateScreen(oldActive);
                  } finally {
                     // If 'ActivateScreen' and 'DeactivateScreen' throws an exception,
                     // .NET swallows exception of the deactivation which is the most
                     // natural behavior (since activation is what we are actually
                     // doing and deactivation is more a side effect).
                     ActivateScreen(newActive);
                     UpdateHistory(newActive);
                     _activeScreen = newActive;
                  }
               } finally {
                  // We always raise the 'PropertyChanged' event to make sure the UI is
                  // updated.
                  OnPropertyChanged(ExpressionService.GetPropertyName(() => ActiveScreen));
               }
            } finally {
               _updatingActiveScreen = false;
            }
         }
      }

      public IEnumerable<IScreenBase> Screens {
         get { return _screens.ObservableItems; }
      }

      public event PropertyChangedEventHandler PropertyChanged;

      public void OpenScreen<TScreen>(IScreenFactory<TScreen> factory)
         where TScreen : class, IScreenBase {

         ScreenCreationBehavior creationBehavior = GetCreationBehavior(factory);
         IScreenBase alreadyOpenScreen = null;

         switch (creationBehavior) {
            case ScreenCreationBehavior.MultipleInstances:
               break;
            case ScreenCreationBehavior.SingleInstance:
               alreadyOpenScreen = _screens
                  .OfType<TScreen>() // TODO: This is incorrect!
                  .SingleOrDefault();
               break;
            case ScreenCreationBehavior.UseScreenLocation:
               alreadyOpenScreen = Screens
                  .FirstOrDefault(x => factory.CreatesScreensEquivalentTo(x));
               break;
         }

         bool wasAlreadyOpen = alreadyOpenScreen != null;

         // We the constructor or Initialize handler of the screen throws an 
         // exception we exit here and 'ActiveScreen' is not changed. The screen
         // itself makes sure that is consistently closed in case of an exception.
         IScreenBase s = wasAlreadyOpen ?
            alreadyOpenScreen :
            _screens.AddScreen(factory);

         s.Children
            .Add(new ScreenCloseHandler(skipRequestClose => CloseScreen(s, skipRequestClose)));

         // Activate does handle exceptions correctly and rethrows them, so we skip
         // the publishing of the 'ScreenOpenedEvent'.
         ActiveScreen = s;

         _eventAggregator.Publish(
            ScreenOpenedEvent,
            new ScreenOpenedEventArgs(this, s, wasAlreadyOpen)
         );
      }

      public bool CloseScreen(IScreenBase screen, bool skipRequestClose = false) {
         if (!_screens.Contains(screen)) {
            throw new ArgumentException(ExceptionTexts.ScreenNotContainedByConductor);
         }

         bool shouldClose;

         try {
            shouldClose = skipRequestClose ?
               true :
               GetLifecycleOps(screen).RequestClose();
         } catch (ScreenLifecycleException) {
            // We always propagate the exception that occured in 'RequestClose' to
            // the client, because thats the method he is actually calling.
            try {
               _screens.Remove(screen);
               _activatedScreensHistory.Remove(screen);
               ActiveScreen = _activatedScreensHistory.LastOrDefault();
            } catch { }

            throw;
         }

         if (!shouldClose) {
            return false;
         }

         // We have to remove the screen BEFORE we say 'LastOrDefault'.
         _activatedScreensHistory.Remove(screen);

         try {
            if (ActiveScreen == screen) {
               // Exception cases:
               //   (1) Deactivate of 'screen' fails: It is correctly removed by the
               //       'ActiveScreen' property and a new screen is activated.
               //   (2) Activate of new screen fails. The old screen is correctly
               //       deactivated.
               ActiveScreen = _activatedScreensHistory.LastOrDefault();
            }
         } finally {
            // If Deactivate fails, 'DeactivateScreen' removes the screen from the 
            // 'Screens' collection.
            bool deactivateSucceded = Screens.Contains(screen);
            if (deactivateSucceded) {
               try {
                  // It is important to FIRST remove the screen and THEN call 'Close'. The
                  // removal triggers a collection change which causes the view reprenstation
                  // to close the view. In this stage the screen may still be accessed by the
                  // view. If 'Close' was called before, the screen may already be in an
                  // disposed state (e.g. database session closed).
                  // 
                  // 'Remove' may throw an arbitrary exception if a 'CollectionChanged' handler
                  // accesses a failed screen. In this case we still try to close the screen.
                  _screens.Remove(screen);
               } finally {
                  GetLifecycleOps(screen).Close();
               }

               // We only publish the event if 'Deactivate' and 'Close' succeeds.
               _eventAggregator.Publish(
                  ScreenClosedEvent,
                  new ConductorEventArgs(this, screen)
               );
            }

            var closeHandlers = screen
               .Children
               .OfType<ScreenCloseHandler>()
               .ToArray();

            foreach (var h in closeHandlers) {
               screen.Children.Remove(h);
            }
         }

         return true;
      }

      protected virtual void OnPropertyChanged(string propertyName) {
         var h = PropertyChanged;
         if (h != null) {
            h(this, new PropertyChangedEventArgs(propertyName));
         }
      }

      private bool RequestClose(IScreenBase screen) {
         try {
            return GetLifecycleOps(screen).RequestClose();
         } catch (ScreenLifecycleException) {
            _activatedScreensHistory.Remove(screen);

            try {
               // 'Remove' may throw an arbitrary exception
               _screens.Remove(screen);
            } finally {
               if (ActiveScreen == screen) {
                  ActiveScreen = _activatedScreensHistory.LastOrDefault();
               }
            }

            throw;
         }
      }

      private void DeactivateScreen(IScreenBase screen) {
         if (screen == null || Lifecycle.State != LifecycleState.Activated) {
            return;
         }

         // 'ScreenConductor.RequestClose' removes the screen if the 'RequestClose'
         // event throws an exception and sets the 'ActiveScreen' to the last active
         // screen. In this case the currently active screen may already be in an
         // error state in which case we must not call 'Deactivate'.
         if (!Screens.Contains(screen)) {
            return;
         }

         try {
            GetLifecycleOps(screen).Deactivate();
         } catch (ScreenLifecycleException) {
            _activatedScreensHistory.Remove(screen);

            // 'Remove' may throw an arbitrary exception
            _screens.Remove(screen);
            throw;
         }
      }

      private void ActivateScreen(IScreenBase screen) {
         if (screen == null || Lifecycle.State != LifecycleState.Activated) {
            return;
         }

         try {
            GetLifecycleOps(screen).Activate();
         } catch (ScreenLifecycleException) {
            _activatedScreensHistory.Remove(screen);

            // 'Remove' may throw an arbitrary exception
            _screens.Remove(screen);
            throw;
         }
      }

      private void HandleActivate(ScreenEventArgs args) {
         if (_activeScreen != null) {
            GetLifecycleOps(_activeScreen).Activate();
         }
      }

      private void HandleDeactivate(ScreenEventArgs args) {
         if (_activeScreen != null) {
            GetLifecycleOps(_activeScreen).Deactivate();
         }
      }

      private void HandleRequestClose(RequestCloseEventArgs args) {
         args.IsCloseAllowed = _screens
            .All(s => GetLifecycleOps(s).RequestClose());
      }

      private void HandleClose(ScreenEventArgs args) {
         while (_screens.Any()) {
            CloseScreen(_screens.Last(), skipRequestClose: true);
         }
      }

      private void UpdateHistory(IScreenBase mostRecentlyActivatedScreen) {
         if (mostRecentlyActivatedScreen != null) {
            _activatedScreensHistory.Remove(mostRecentlyActivatedScreen);
            _activatedScreensHistory.Add(mostRecentlyActivatedScreen);
         }
      }

      private ScreenCreationBehavior GetCreationBehavior(IScreenFactory<IScreenBase> screenFactory) {
         var attr = (ScreenCreationBehaviorAttribute)Attribute.GetCustomAttribute(
            screenFactory.ScreenType,
            typeof(ScreenCreationBehaviorAttribute)
         );

         return attr != null ?
            attr.CreationBehavior :
            ScreenCreationBehavior.MultipleInstances;
      }

      private ScreenLifecycleOperations GetLifecycleOps(IScreenBase screen) {
         return new ScreenLifecycleOperations(_eventAggregator, screen);
      }
   }
}
