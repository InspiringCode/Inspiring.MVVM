namespace Inspiring.Mvvm.ViewModels.Core {
   using System;


   public interface IBehaviorConfigurationExpression {
      IBehaviorConfigurationExpression Insert(
         VMBehaviorKey behaviorKey,
         RelativePosition relativeTo,
         VMBehaviorKey other
      );

      IBehaviorConfigurationExpression OverrideFactory(
         VMBehaviorKey behaviorKey,
         IBehaviorFactory factory
      );

      IBehaviorConfigurationExpression Enable(
         VMBehaviorKey behaviorKey
      );

      IBehaviorConfigurationExpression Configure<TBehavior>(
         VMBehaviorKey behaviorKey,
         Action<TBehavior> configurationAction
      );
   }

   public interface IBehaviorConfigurationExpression2 {
      IBehaviorConfigurationExpression2 Add(
         VMBehaviorKey key,
         IBehaviorFactory behavior,
         RelativePosition relativeTo,
         VMBehaviorKey position,
         bool addLazily = false
      );

      IBehaviorConfigurationExpression2 Override(
         VMBehaviorKey behavior,
         IBehaviorFactory withBehavior,
         bool addLazily = false
      );

      IBehaviorConfigurationExpression2 OverridePermanently(
         VMBehaviorKey behavior,
         IBehaviorFactory withBehavior,
         bool addLazily = false
      );

      IBehaviorConfigurationExpression2 ReplaceBehaviors(
         BehaviorConfiguration withBehaviors
      );

      IBehaviorConfigurationExpression2 ConfigureBehavior<TBehavior>(
         VMBehaviorKey behavior,
         Action<TBehavior> configurationAction
      );
   }

   public enum RelativePosition {
      Before,
      After
   }
}
