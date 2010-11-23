namespace Inspiring.Mvvm.ViewModels.Core {

   public static class VMPropertyExtensionMethods {
      public static ValidationState GetValidationState(
         this IVMProperty property,
         IBehaviorContext context
      ) {
         return property
            .Behaviors
            .GetNextBehavior<IValidationStateProviderBehavior>()
            .GetValidationState(context);
      }

      public static bool IsMutable(
         this IVMProperty property,
         IBehaviorContext context
      ) {
         IMutabilityCheckerBehavior mutabilityChecker;
         bool found = property.Behaviors.TryGetBehavior(out mutabilityChecker);

         return found ?
            mutabilityChecker.IsMutable(context) :
            true;
      }
   }
}
