namespace Inspiring.Mvvm.ViewModels.Core {
   using System.Diagnostics.Contracts;

   internal sealed class ItemDescriptorProviderBehavior :
      Behavior,
      IItemDescriptorProviderBehavior {

      private VMDescriptorBase _itemDescriptor;

      public ItemDescriptorProviderBehavior(VMDescriptorBase itemDescriptor) {
         Contract.Requires(itemDescriptor != null);
         ItemDescriptor = itemDescriptor;
      }

      public VMDescriptorBase ItemDescriptor { get; private set; }
   }
}
