//namespace Inspiring.Mvvm.ViewModels.Core.PropertyBehaviors.PropertyBehaviors {
//   using System;

//   internal sealed class RevalidationBehavior<TValue> :
//      Behavior,
//      IRevalidationBehavior {

//      private bool _isViewModelProperty;

//      public RevalidationBehavior() {
//         _isViewModelProperty = PropertyTypeHelper.IsViewModel(typeof(TValue));
//      }

//      public void Revalidate(IBehaviorContext context, ValidationMode mode) {
//         throw new NotImplementedException();
//      }

//      private void ClearPrevalidationValueCache() {
//         // TODO: Maybe better to rearrange stack?
//      }
//   }
//}
