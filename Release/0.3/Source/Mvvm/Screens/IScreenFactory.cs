namespace Inspiring.Mvvm.Screens {
   using System;

   public interface IScreenFactory<out TScreen> where TScreen : IScreenBase {
      TScreen Create(Action<TScreen> initializationCallback = null);

      /// <summary>
      ///   Returns true, if the <see cref="IScreenFactory{TScreen}"/> would create 
      ///   a screen that represents the same data as the screen given by <paramref 
      ///   name="concreteScreen"/>.
      /// </summary>
      bool CreatesScreensEquivalentTo(IScreenBase concreteScreen);
   }
}
