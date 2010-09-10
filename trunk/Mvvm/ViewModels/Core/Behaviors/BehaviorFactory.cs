namespace Inspiring.Mvvm.ViewModels.Behaviors {
   using System;

   public abstract class VMBehaviorFactory {
      private Type _behaviorType;

      protected VMBehaviorFactory(Type behaviorType) {
         _behaviorType = behaviorType;
      }

      public abstract IBehavior Create<TValue>();

      protected internal virtual bool Matches(IBehavior behavior) {
         Type behaviorType = behavior.GetType();

         Type registeredType = behaviorType.IsGenericType ?
            behaviorType.GetGenericTypeDefinition() :
            behaviorType;

         return _behaviorType == behaviorType;
      }
   }
}
