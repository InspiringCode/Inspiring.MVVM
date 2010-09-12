using System;
namespace Inspiring.Mvvm.ViewModels.Core {
   internal sealed class CollectionFactoryBehavior<TVM> :
      Behavior, IAccessPropertyBehavior<VMCollection<TVM>> {

      public VMCollection<TVM> GetValue(IBehaviorContext vm) {
         return new VMCollection<TVM>();
      }

      public void SetValue(IBehaviorContext vm, VMCollection<TVM> value) {
         throw new NotSupportedException();
      }
   }
}
