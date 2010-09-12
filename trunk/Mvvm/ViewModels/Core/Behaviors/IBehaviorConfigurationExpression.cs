namespace Inspiring.Mvvm.ViewModels.Core {
   using System;

   public interface IBehaviorConfigurationExpression {
      IBehaviorConfigurationExpression Add(
         VMBehaviorKey key,
         IBehaviorFactory behavior,
         BehaviorOrderModifier relativeTo,
         VMBehaviorKey position,
         bool addLazily = false
      );

      IBehaviorConfigurationExpression Override(
         VMBehaviorKey behavior,
         IBehaviorFactory withBehavior,
         bool addLazily = false
      );

      IBehaviorConfigurationExpression OverridePermanently(
         VMBehaviorKey behavior,
         IBehaviorFactory withBehavior,
         bool addLazily = false
      );

      IBehaviorConfigurationExpression ReplaceBehaviors(
         BehaviorConfiguration withBehaviors
      );

      IBehaviorConfigurationExpression ConfigureBehavior<TBehavior>(
         VMBehaviorKey behavior,
         Action<TBehavior> configurationAction
      );
   }

   public enum BehaviorOrderModifier {
      Before,
      After
   }
}
