﻿namespace Inspiring.Mvvm.ViewModels.Core {
   using System.Diagnostics.Contracts;

   internal sealed class ItemDescriptorProviderBehavior :
      Behavior,
      IItemDescriptorProviderBehavior {

      private IVMDescriptor _itemDescriptor;

      public ItemDescriptorProviderBehavior(IVMDescriptor itemDescriptor) {
         Contract.Requires(itemDescriptor != null);
         ItemDescriptor = itemDescriptor;
      }

      public IVMDescriptor ItemDescriptor { get; private set; }
   }
}