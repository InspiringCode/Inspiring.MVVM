namespace Inspiring.Mvvm.ViewModels.Core {

   public static class VMPropertyExtensionMethods {
      public static ValidationState GetValidationState(
         this IVMProperty property,
         IBehaviorContext context
      ) {
         IValidationStateProviderBehavior stateProvider;
         bool found = property.Behaviors.TryGetBehavior(out stateProvider);

         return found ?
            stateProvider.GetValidationState(context) :
            ValidationState.Valid;
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
