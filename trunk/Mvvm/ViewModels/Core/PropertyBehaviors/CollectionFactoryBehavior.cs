using System;
namespace Inspiring.Mvvm.ViewModels.Core {
   internal sealed class CollectionFactoryBehavior<TVM> :
      Behavior, IAccessPropertyBehavior<VMCollection<TVM>> {
      private VMDescriptor _itemDescriptor;

      public CollectionFactoryBehavior(VMDescriptor itemDescriptor) {
         _itemDescriptor = itemDescriptor;
      }

      public VMCollection<TVM> GetValue(IBehaviorContext vm) {
         return new VMCollection<TVM>(_itemDescriptor);
      }

      public void SetValue(IBehaviorContext vm, VMCollection<TVM> value) {
         throw new NotSupportedException();
      }
   }
}
