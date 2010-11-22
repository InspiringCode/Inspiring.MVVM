namespace Inspiring.Mvvm.ViewModels.Core {
   using Inspiring.Mvvm.ViewModels.Future;

   public static class VMDescriptorBaseExtensionMethods {
      public static ValidationState GetValidationState(
         this VMDescriptorBase descriptor,
         IBehaviorContext context
      ) {
         return descriptor
            .Behaviors
            .GetNextBehavior<IValidationStateProviderBehavior>()
            .GetValidationState(context);
      }
   }
}
