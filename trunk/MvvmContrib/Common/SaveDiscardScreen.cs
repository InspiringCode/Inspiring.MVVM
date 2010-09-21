namespace Inspiring.Mvvm.Common {
   using System;
   using Inspiring.Mvvm.Screens;

   public sealed class SaveDiscardScreen : Screen, INeedsInitialization<SaveDiscardScreenSubject> {
      public IScreen Content { get; internal set; }

      public SaveDiscardVM VM { get; internal set; }

      public void Initialize(SaveDiscardScreenSubject subject) {
         subject.InitializeSaveDiscardScreen(this);
      }
   }


   public sealed class SaveDiscardScreenSubject {
      private SaveDiscardScreenSubject() {
      }

      internal Action<SaveDiscardScreen> InitializeSaveDiscardScreen { get; private set; }

      public static SaveDiscardScreenSubject For<TScreen>(
         IScreenFactory<TScreen> content
      ) where TScreen : IScreen, ISaveDiscardHandler {
         return new SaveDiscardScreenSubject {
            InitializeSaveDiscardScreen = dialogScreen => {
               TScreen contentScreen = dialogScreen.Children.AddNew(content);

               dialogScreen.Content = contentScreen;

               dialogScreen.VM = new SaveDiscardVM();
               dialogScreen.VM.InitializeFrom(contentScreen);
            }
         };
      }
   }
}
