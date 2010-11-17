namespace Inspiring.Mvvm.ViewModels.Core {

   public interface IValidationStatePropertyBehavior : IBehavior {
      ValidationState GetValidationState(IBehaviorContext_ context);
   }
}
