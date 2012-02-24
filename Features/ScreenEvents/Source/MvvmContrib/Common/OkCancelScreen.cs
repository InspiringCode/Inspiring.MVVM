namespace Inspiring.Mvvm.Common {
   using System;
   using Inspiring.Mvvm.Screens;

   public sealed class OkCancelScreen : ScreenBase, INeedsInitialization<OkCancelScreenSubject> {
      public OkCancelScreen(EventAggregator aggregator)
         : base(aggregator) {
      }

      public IScreenBase Content { get; internal set; }

      public OkCancelVM VM { get; internal set; }

      public void Initialize(OkCancelScreenSubject subject) {
         subject.InitializeOkCancelScreen(this);
      }
   }

   public sealed class OkCancelScreenSubject {
      private OkCancelScreenSubject() {
      }

      internal Action<OkCancelScreen> InitializeOkCancelScreen { get; private set; }

      public static OkCancelScreenSubject For<TScreen>(
         IScreenFactory<TScreen> content
      ) where TScreen : IScreenBase, IOkCancelHandler {
         return new OkCancelScreenSubject {
            InitializeOkCancelScreen = dialogScreen => {
               TScreen contentScreen = dialogScreen.Children.AddScreen(content);

               dialogScreen.Content = contentScreen;

               dialogScreen.VM = new OkCancelVM();
               dialogScreen.VM.InitializeFrom(contentScreen);
            }
         };
      }
   }
}
