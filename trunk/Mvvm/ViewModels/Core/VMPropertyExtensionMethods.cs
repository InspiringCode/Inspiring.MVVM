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
   }
}
