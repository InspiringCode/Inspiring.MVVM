namespace Inspiring.Mvvm.ViewModels.Core {
   using System;

   internal sealed class CollectionFactoryBehavior<TItemVM> :
      Behavior,
      IAccessPropertyBehavior<VMCollection<TItemVM>>
      where TItemVM : ViewModel {
      private VMDescriptor _itemDescriptor;

      public CollectionFactoryBehavior(VMDescriptor itemDescriptor) {
         _itemDescriptor = itemDescriptor;
      }

      public VMCollection<TItemVM> GetValue(IBehaviorContext vm) {
         return new VMCollection<TItemVM>(_itemDescriptor);
      }

      public void SetValue(IBehaviorContext vm, VMCollection<TItemVM> value) {
         throw new NotSupportedException();
      }
   }
}
