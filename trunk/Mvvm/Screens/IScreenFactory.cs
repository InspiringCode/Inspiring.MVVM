namespace Inspiring.Mvvm.Screens {
   using System;

   public interface IScreenFactory<TScreen> where TScreen : IScreenBase {
      TScreen Create(Action<TScreen> initializationCallback = null);
   }
}
