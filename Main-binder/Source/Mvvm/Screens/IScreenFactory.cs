namespace Inspiring.Mvvm.Screens {
   using System;

   public interface IScreenFactory<out TScreen> where TScreen : IScreenBase {
      TScreen Create(Action<TScreen> initializationCallback = null);
   }
}
