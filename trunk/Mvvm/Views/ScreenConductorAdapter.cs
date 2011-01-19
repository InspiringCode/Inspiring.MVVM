namespace Inspiring.Mvvm.Views {
   using System;
   using System.Collections;
   using System.Collections.Generic;
   using System.Collections.ObjectModel;
   using System.Collections.Specialized;
   using System.ComponentModel;
   using System.Linq;
   using Inspiring.Mvvm.Common;
   using Inspiring.Mvvm.Screens;

   public class ScreenConductorAdapter : NotifyObject {
      private static readonly string ActiveScreenPropertyName =
         ExpressionService.GetPropertyName((ScreenConductor x) => x.ActiveScreen);

      private ScreenConductor _screens;
      private Dictionary<IScreenBase, object> _screenViews = new Dictionary<IScreenBase, object>();
      private IList _views;
      private object _activeView;

      public ScreenConductorAdapter(ScreenConductor screens)
         : this(screens, new ObservableCollection<object>()) {
      }

      public ScreenConductorAdapter(ScreenConductor screens, IList viewCollection) {
         _views = viewCollection;
         _screens = screens;

         INotifyCollectionChanged notify = (INotifyCollectionChanged)_screens.Screens;
         notify.CollectionChanged += HandleScreensChanged;

         _screens.PropertyChanged += HandlePropertyChanged;

         _screens.Screens.ForEach(AddScreen);

         object view = ViewFor(_screens.ActiveScreen);
         OnActiveViewChanged(view);
      }

      public IEnumerable Views {
         get { return _views; }
      }

      public object ActiveView {
         get { return _activeView; }
         set {
            _screens.ActiveScreen = ScreenFor(value);
         }
      }

      public bool CloseView(object view) {
         IScreenBase screen = ScreenFor(view);
         return _screens.CloseScreen(screen);
      }

      protected virtual void OnActiveViewChanged(object view) {
         _activeView = view;
         OnPropertyChanged(() => ActiveView);
      }

      protected virtual void OnViewAdded(object view) {
         _views.Add(view);
      }

      protected virtual void OnViewRemoved(object view) {
         _views.Remove(view);
      }

      protected virtual object CreateView(IScreenBase forScreen) {
         return ViewFactory.CreateView(forScreen);
      }

      private void HandlePropertyChanged(object sender, PropertyChangedEventArgs e) {
         if (e.PropertyName == ActiveScreenPropertyName) {
            object view = ViewFor(_screens.ActiveScreen);
            OnActiveViewChanged(view);
         }
      }

      private void HandleScreensChanged(object sender, NotifyCollectionChangedEventArgs e) {
         IScreenBase s;
         switch (e.Action) {
            case NotifyCollectionChangedAction.Add:
               s = e.NewItems.Cast<IScreenBase>().Single();
               AddScreen(s);
               break;
            case NotifyCollectionChangedAction.Remove:
               s = e.OldItems.Cast<IScreenBase>().Single();
               RemoveScreen(s);
               break;
            default:
               throw new NotSupportedException(
                  ExceptionTexts.UnsupportedCollectionChangedAction
               );
         }
      }

      private void AddScreen(IScreenBase screen) {
         object view = CreateView(forScreen: screen);
         _screenViews.Add(screen, view);
         OnViewAdded(view);
      }

      private void RemoveScreen(IScreenBase screen) {
         object view = ViewFor(screen);
         _screenViews.Remove(screen);
         OnViewRemoved(view);
      }

      private object ViewFor(IScreenBase screen) {
         return screen != null ?
            _screenViews[screen] :
            null;
      }

      private IScreenBase ScreenFor(object view) {
         return _screenViews.SingleOrDefault(x => x.Value == view).Key;
      }
   }
}
