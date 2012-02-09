namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Diagnostics.Contracts;

   public abstract class InitializableBehavior : Behavior {
      /// <summary>
      ///   Gets whether the behavior was already initialized. If a behavior does
      ///   not require initialization this property has no meaning.
      /// </summary>
      /// <remarks>
      ///   The concrete form of initialization is specific to the behavior. Call
      ///   <see cref="SetInitialized"/> when you have initialized your behavior
      ///   and call <see cref="RequireInitialized"/> before every non-initialization
      ///   method of your behavior.
      /// </remarks>
      public bool IsInitialized { get; private set; }

      /// <summary>
      ///   Marks the behavior as initialized.
      /// </summary>
      protected void SetInitialized() {
         Contract.Ensures(IsInitialized);
         IsInitialized = true;
      }

      /// <summary>
      ///   Throws an exception if the behavior is not initialized.
      /// </summary>
      protected void RequireInitialized() {
         Contract.Requires<InvalidOperationException>(
            IsInitialized,
            ExceptionTexts.BehaviorNotInitialized
         );
      }
   }
}
