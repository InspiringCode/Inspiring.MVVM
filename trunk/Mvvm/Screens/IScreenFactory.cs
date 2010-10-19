namespace Inspiring.Mvvm.Screens {
   using System;

   public interface IScreenFactory<TScreen> where TScreen : IScreen {
      TScreen Create(Action<TScreen> initializationCallback = null);
   }
}
