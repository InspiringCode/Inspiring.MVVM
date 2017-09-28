namespace Inspiring.Mvvm.ViewModels.Core {
   
   internal sealed class ItemDescriptorProviderBehavior :
      Behavior,
      IItemDescriptorProviderBehavior {

      public ItemDescriptorProviderBehavior(IVMDescriptor itemDescriptor) {
         Check.NotNull(itemDescriptor, nameof(itemDescriptor));
         ItemDescriptor = itemDescriptor;
      }

      public IVMDescriptor ItemDescriptor { get; private set; }
   }
}
