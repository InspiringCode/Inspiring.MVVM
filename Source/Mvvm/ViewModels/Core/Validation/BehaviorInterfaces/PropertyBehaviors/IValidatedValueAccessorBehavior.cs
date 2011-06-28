namespace Inspiring.Mvvm.ViewModels.Core {

   public interface IValidatedValueAccessorBehavior<TValue> : IBehavior {
      TValue GetValidatedValue(IBehaviorContext context);
   }
}
