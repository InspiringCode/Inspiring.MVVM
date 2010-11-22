namespace Inspiring.Mvvm.ViewModels.Core {
   using System;

   internal sealed class CollectionFactoryBehavior<TItemVM> :
      Behavior,
      IPropertyAccessorBehavior<VMCollection<TItemVM>>
      where TItemVM : IViewModel {
      private VMDescriptor _itemDescriptor;

      public CollectionFactoryBehavior(VMDescriptor itemDescriptor) {
         _itemDescriptor = itemDescriptor;
      }

      public VMCollection<TItemVM> GetValue(IBehaviorContext vm, ValueStage stage) {
         CollectionValidationBehavior<TItemVM> validationBehavior;
         return TryGetBehavior(out validationBehavior) ?
            new VMCollection<TItemVM>((IViewModel)vm.VM, _itemDescriptor, validationBehavior) :
            new VMCollection<TItemVM>((IViewModel)vm.VM, _itemDescriptor);
      }

      public void SetValue(IBehaviorContext vm, VMCollection<TItemVM> value) {
         throw new NotSupportedException();
      }
   }
}
