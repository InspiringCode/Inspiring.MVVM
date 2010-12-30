namespace Inspiring.Mvvm.ViewModels.Core {
   using Inspiring.Mvvm.ViewModels;

   public static class VMDescriptorBaseExtensionMethods {
      public static ValidationState GetValidationState(
         this VMDescriptorBase descriptor,
         IBehaviorContext context
      ) {
         ViewModelValidationBehavior behavior; // TODO: Interface?

         return descriptor.Behaviors.TryGetBehavior(out behavior) ?
            behavior.GetValidationState(context) :
            ValidationState.Valid;
      }
   }
}
