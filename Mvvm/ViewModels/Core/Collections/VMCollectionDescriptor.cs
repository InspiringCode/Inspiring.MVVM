namespace Inspiring.Mvvm.ViewModels.Core.Collections {
   using System.Diagnostics.Contracts;
   using Inspiring.Mvvm.ViewModels;

   public sealed class VMCollectionDescriptor {
      public VMCollectionDescriptor(VMDescriptorBase itemDescriptor) {
         Contract.Requires(itemDescriptor != null);
         ItemDescriptor = itemDescriptor;
         Behaviors = new Behavior();
      }

      public Behavior Behaviors { get; private set; }

      public VMDescriptorBase ItemDescriptor { get; private set; }

      [ContractInvariantMethod]
      private void ObjectInvariant() {
         Contract.Invariant(Behaviors != null);
      }
   }
}