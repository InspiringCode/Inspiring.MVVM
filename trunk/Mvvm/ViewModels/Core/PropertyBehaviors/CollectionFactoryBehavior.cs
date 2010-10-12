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
         CollectionValidationBehavior<TItemVM> validationBehavior;
         return TryGetBehavior(out validationBehavior) ?
            new VMCollection<TItemVM>(vm.VM, _itemDescriptor, validationBehavior) :
            new VMCollection<TItemVM>(vm.VM, _itemDescriptor);
      }

      public void SetValue(IBehaviorContext vm, VMCollection<TItemVM> value) {
         throw new NotSupportedException();
      }
   }
}
