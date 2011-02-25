namespace Inspiring.Mvvm.Screens {
   using System;
   using System.Collections.ObjectModel;
   using System.Diagnostics.Contracts;
   using System.Linq;

   public sealed class ScreenCollection {
      private IScreenInitializer _initializer;

      internal ScreenCollection(IScreen parent) {
         Contract.Requires(parent != null);
         _initializer = new ScreenInitializer(parent);
         Screens = new ObservableCollection<IScreen>();
      }

      internal ObservableCollection<IScreen> Screens { get; private set; }

      public TScreen AddNew<TScreen>(IScreenFactory<TScreen> screen) where TScreen : IScreen {
         Contract.Requires<ArgumentNullException>(screen != null);
         TScreen s = screen.Create(_initializer);
         Screens.Add(s);
         return s;
      }

      internal void Activate() {
         Screens.ForEach(s => s.Activate());
      }

      internal void Deactivate() {
         Screens.ForEach(s => s.Deactivate());
      }

      internal bool RequestClose() {
         return Screens.All(s => s.RequestClose());
      }

      internal void Close() {
         Screens.ForEach(s => s.Close());
      }

      private class ScreenInitializer : IScreenInitializer {
         private IScreen _parent;

         public ScreenInitializer(IScreen parent) {
            _parent = parent;
         }

         public void Initialize(IScreen screen) {
            screen.Initialize();
            screen.Parent = _parent;
         }

         public void Initialize<TSubject>(IScreen<TSubject> screen, TSubject subject) {
            Initialize(screen);
            screen.Initialize(subject);
         }
      }
   }
}
