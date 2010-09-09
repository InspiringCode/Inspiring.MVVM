namespace Inspiring.Mvvm.Screens {
   using System;
   using System.Collections.Generic;
   using System.Diagnostics.Contracts;
   using System.Linq;
   using Inspiring.Mvvm.Common;

   public class Screen : NotifyObject, IScreen {
      public Screen() {
         Children = new ScreenCollection(this);
         Behaviors = new List<ScreenBehavior>();
         AddBehavior(new ChildrenBehavior(Children));
      }

      IScreen IScreen.Parent { get; set; }

      internal List<ScreenBehavior> Behaviors { get; private set; }

      protected ScreenCollection Children { get; private set; }

      public void OpenScreen<TScreen>(
         IScreenFactory<TScreen> screen,
         object conductorId = null
      ) where TScreen : IScreen {
         ScreenConductor conductor = FindScreen<ScreenConductor>(conductorId);

         if (conductor == null) {
            throw new ArgumentException(
               ExceptionTexts.ScreenConductorNotFound.FormatWith(conductorId ?? "NULL")
            );
         }

         conductor.OpenScreen(screen);
      }

      public static void AddBehavior(Screen screen, ScreenBehavior behavior) {
         screen.AddBehavior(behavior);
      }

      public static TBehavior GetBehavior<TBehavior>(Screen screen) where TBehavior : ScreenBehavior {
         return screen.Behaviors.OfType<TBehavior>().FirstOrDefault();
      }

      void IScreen.Initialize() {
         Behaviors.ForEach(b => b.BeforeInitialize());
         OnInitialize();
         Behaviors.ForEach(b => b.AfterInitialize());
      }

      void IScreen.Activate() {
         Behaviors.ForEach(b => b.BeforeActivate());
         OnActivate();
         Behaviors.ForEach(b => b.AfterActivate());

         InvokeAfterInitialized();
      }

      void IScreen.Deactivate() {
         Behaviors.ForEach(b => b.BeforeDeactivate());
         OnDeactivate();
         Behaviors.ForEach(b => b.AfterDeactivate());
      }

      bool IScreen.RequestClose() {
         return
            Behaviors.All(b => b.BeforeRequestClose()) &&
            OnRequestClose() &&
            Behaviors.All(b => b.AfterRequestClose());
      }

      void IScreen.Close() {
         Behaviors.ForEach(b => b.BeforeClose());
         OnClose();
         Behaviors.ForEach(b => b.AfterClose());
      }

      internal virtual void InvokeAfterInitialized() {
         Behaviors.ForEach(b => b.AfterInitialized());
      }

      protected void AddBehavior(ScreenBehavior behavior) {
         Contract.Requires<ArgumentNullException>(behavior != null);
         Behaviors.Add(behavior);
      }

      protected virtual void OnInitialize() {
      }

      protected virtual void OnActivate() {
      }

      protected virtual void OnDeactivate() {
      }

      protected virtual bool OnRequestClose() {
         return true;
      }

      protected virtual void OnClose() {
      }

      protected TScreen FindScreen<TScreen>(object screenId = null) where TScreen : IScreen {
         return (TScreen)FindScreen(s => {
            if (!(s is TScreen)) {
               return false;
            }

            IIdentifiedScreen identified = s as IIdentifiedScreen;
            return screenId == null || (
               identified != null &&
               Object.Equals(identified.ScreenId, screenId)
            );
         });
      }

      private IScreen FindScreen(Func<IScreen, bool> searchSpecification) {
         IScreen s = this;
         IScreen root = this;

         for (s = s.Parent; s != null; s = s.Parent) {
            if (searchSpecification(s)) {
               return s;
            }

            root = s;
         }

         Queue<IScreen> screens = new Queue<IScreen>();
         screens.Enqueue(root);

         while (screens.Count > 0) {
            s = screens.Dequeue();

            if (searchSpecification(s)) {
               return s;
            }

            Screen scr = s as Screen;
            if (scr != null) {
               scr.Children.Screens.ForEach(screens.Enqueue);
            }

            ScreenConductor conductor = s as ScreenConductor;
            if (conductor != null) {
               conductor.Screens.ForEach(screens.Enqueue);
            }
         }

         return null;
      }

      private class ChildrenBehavior : ScreenBehavior {
         private ScreenCollection _children;

         public ChildrenBehavior(ScreenCollection children) {
            _children = children;
         }

         protected internal override void AfterActivate() {
            _children.Activate();
         }

         protected internal override void BeforeDeactivate() {
            _children.Deactivate();
         }

         protected internal override bool BeforeRequestClose() {
            return _children.RequestClose();
         }

         protected internal override void BeforeClose() {
            _children.Close();
         }
      }
   }

   public class Screen<TSubject> : Screen, IScreen<TSubject> {
      void IScreen<TSubject>.Initialize(TSubject subject) {
         Behaviors.ForEach(b => b.BeforeInitialize(subject));
         OnInitialize(subject);
         Behaviors.ForEach(b => b.AfterInitialize(subject));
         InvokeAfterInitialized();
      }

      internal override void InvokeAfterInitialized() {
         // Do not invoke AfterInitalized this time
      }

      protected virtual void OnInitialize(TSubject subject) {
      }
   }
}
