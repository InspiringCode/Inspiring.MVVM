namespace Inspiring.Mvvm.Common {
   using System;
   using Inspiring.Mvvm.Screens;

   public sealed class SaveDiscardScreen : ScreenBase, INeedsInitialization<SaveDiscardScreenSubject> {
      public SaveDiscardScreen(EventAggregator aggregator)
         : base(aggregator) {
      }

      public IScreenBase Content { get; internal set; }

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
      ) where TScreen : IScreenBase, ISaveDiscardHandler {
         return new SaveDiscardScreenSubject {
            InitializeSaveDiscardScreen = dialogScreen => {
               TScreen contentScreen = dialogScreen.Children.AddScreen(content);

               dialogScreen.Content = contentScreen;

               dialogScreen.VM = new SaveDiscardVM();
               dialogScreen.VM.InitializeFrom(contentScreen);
            }
         };
      }
   }
}
