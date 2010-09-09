using System;
namespace Inspiring.Mvvm.ViewModels.Behaviors {
   internal sealed class CollectionFactoryBehavior<TVM> :
      VMPropertyBehavior, IAccessPropertyBehavior<VMCollection<TVM>> {

      public override BehaviorPosition Position {
         get { return BehaviorPosition.CollectionFactory; }
      }

      public VMCollection<TVM> GetValue(IBehaviorContext vm) {
         return new VMCollection<TVM>();
      }

      public void SetValue(IBehaviorContext vm, VMCollection<TVM> value) {
         throw new NotSupportedException();
      }
   }
}
