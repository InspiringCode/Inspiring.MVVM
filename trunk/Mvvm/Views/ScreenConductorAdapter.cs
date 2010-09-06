namespace Inspiring.Mvvm.Views {
   using System;
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
      private Dictionary<IScreen, object> _screenViews = new Dictionary<IScreen, object>();
      private ObservableCollection<object> _views = new ObservableCollection<object>();
      private object _activeView;

      public ScreenConductorAdapter(ScreenConductor screens) {
         _screens = screens;

         INotifyCollectionChanged notify = (INotifyCollectionChanged)_screens.Screens;
         notify.CollectionChanged += HandleScreensChanged;

         _screens.PropertyChanged += HandlePropertyChanged;

         _screens.Screens.ForEach(AddScreen);

         object view = ViewFor(_screens.ActiveScreen);
         OnActiveViewChanged(view);
      }


      public IEnumerable<object> Views {
         get { return _views; }
      }

      public object ActiveView {
         get { return _activeView; }
         set {
            _screens.ActiveScreen = ScreenFor(value);
         }
      }

      public void CloseView(object view) {
         IScreen screen = ScreenFor(view);
         _screens.CloseScreen(screen);
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

      protected virtual object CreateView(IScreen forScreen) {
         return ViewFactory.CreateView(forScreen);
      }

      private void HandlePropertyChanged(object sender, PropertyChangedEventArgs e) {
         if (e.PropertyName == ActiveScreenPropertyName) {
            object view = ViewFor(_screens.ActiveScreen);
            OnActiveViewChanged(view);
         }
      }

      private void HandleScreensChanged(object sender, NotifyCollectionChangedEventArgs e) {
         IScreen s;
         switch (e.Action) {
            case NotifyCollectionChangedAction.Add:
               s = e.NewItems.Cast<IScreen>().Single();
               AddScreen(s);
               break;
            case NotifyCollectionChangedAction.Remove:
               s = e.OldItems.Cast<IScreen>().Single();
               RemoveScreen(s);
               break;
            default:
               throw new NotSupportedException(
                  ExceptionTexts.UnsupportedCollectionChangedAction
               );
         }
      }

      private void AddScreen(IScreen screen) {
         object view = CreateView(forScreen: screen);
         _screenViews.Add(screen, view);
         OnViewAdded(view);
      }

      private void RemoveScreen(IScreen screen) {
         object view = ViewFor(screen);
         _screenViews.Remove(screen);
         OnViewRemoved(view);
      }

      private object ViewFor(IScreen screen) {
         return screen != null ?
            _screenViews[screen] :
            null;
      }

      private IScreen ScreenFor(object view) {
         return _screenViews.SingleOrDefault(x => x.Value == view).Key;
      }
   }
}
