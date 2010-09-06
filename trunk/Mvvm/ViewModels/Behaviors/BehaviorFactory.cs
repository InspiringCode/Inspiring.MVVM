namespace Inspiring.Mvvm.ViewModels.Behaviors {
   using System;

   public abstract class VMBehaviorFactory {
      public abstract Type BehaviorType { get; }

      public abstract IBehavior Create<TValue>();
   }
}
