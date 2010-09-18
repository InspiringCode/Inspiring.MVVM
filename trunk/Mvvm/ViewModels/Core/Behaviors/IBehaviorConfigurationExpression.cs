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

      IBehaviorConfigurationExpression OverrideFactory(
         VMBehaviorKey behaviorKey,
         IBehavior behavior
      );

      IBehaviorConfigurationExpression Enable(
         VMBehaviorKey behaviorKey
      );

      IBehaviorConfigurationExpression Configure<TBehavior>(
         VMBehaviorKey behaviorKey,
         Action<TBehavior> configurationAction
      );

      IBehaviorConfigurationExpression MergeFrom(
         BehaviorConfiguration additionalConfiguration
      );
   }

   public enum RelativePosition {
      Before,
      After
   }
}
