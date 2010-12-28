//namespace Inspiring.Mvvm.ViewModels.Core {
//   internal sealed class DiscardingRevalidationBehavior :
//      Behavior,
//      IRevalidationBehavior {

//      public void Revalidate(IBehaviorContext context, ValidationMode mode) {
//         if (mode == ValidationMode.DiscardInvalidValues) {
//            var displayValueAccessor = GetNextBehavior<IDisplayValueAccessorBehavior>();
//            object preConversionValue = displayValueAccessor.GetDisplayValue(context);
//            displayValueAccessor.SetDisplayValue(context, preConversionValue);
//         }
//      }
//   }
//}
