﻿namespace Inspiring.Mvvm.Screens {
   using System;
   using Inspiring.Mvvm.Common;

   public interface IScreenFactory<out TScreen> where TScreen : IScreenBase {
      /// <remarks>
      ///   If an exception occurs during initialization it should be rethrown by
      ///   the implementation.
      /// </remarks>
      /// <param name="preInitializationCallback">
      ///   Indended for things that should happen before the screen is initialized.
      /// </param>
      TScreen Create(EventAggregator aggregator, Action<TScreen> preInitializationCallback = null);

      /// <summary>
      ///   Returns true, if the <see cref="IScreenFactory{TScreen}"/> would create 
      ///   a screen that represents the same data as the screen given by <paramref 
      ///   name="concreteScreen"/>.
      /// </summary>
      bool CreatesScreensEquivalentTo(IScreenBase concreteScreen);

      /// <summary>
      ///   The type of the screen that gets created. You cannot rely on <typeparamref 
      ///   name="TScreen"/> because <see cref="IScreenFactory{TScreen}"/> is covariant. 
      /// </summary>
      Type ScreenType { get; }
   }
}
