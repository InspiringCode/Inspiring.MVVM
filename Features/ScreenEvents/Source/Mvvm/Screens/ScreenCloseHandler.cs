namespace Inspiring.Mvvm.Screens {
   using System;

   internal sealed class ScreenCloseHandler {
      private readonly Action<bool> _closeAction;

      public ScreenCloseHandler(Action<bool> closeAction) {
         _closeAction = closeAction;
      }

      public void Execute(bool skipRequestClose) {
         _closeAction(skipRequestClose);
      }
   }
}
