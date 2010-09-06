using System;
using System.Collections.Generic;
using Inspiring.Mvvm.Common;
namespace Inspiring.Mvvm.Screens {

   public class Screen : NotifyObject, IScreen {
      public Screen() {
         Children = new ScreenCollection(this);
      }

      IScreen IScreen.Parent { get; set; }

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

      void IScreen.Initialize() {
         OnInitialize();
      }

      void IScreen.Activate() {
         OnActivate();
         Children.Activate();
      }

      void IScreen.Deactivate() {
         Children.Deactivate();
         OnDeactivate();
      }

      bool IScreen.RequestClose() {
         return Children.RequestClose() && OnRequestClose();
      }

      void IScreen.Close() {
         Children.Close();
         OnClose();
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
   }

   public class Screen<TSubject> : Screen, IScreen<TSubject> {
      void IScreen<TSubject>.Initialize(TSubject subject) {
         OnInitialize(subject);
      }

      protected virtual void OnInitialize(TSubject subject) {
      }
   }
}
