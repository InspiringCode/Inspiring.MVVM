namespace Inspiring.Mvvm.ViewModels.Core.Collections {
   using System.Diagnostics.Contracts;

   public sealed class VMCollectionDescriptor {
      public VMCollectionDescriptor() {
         Behaviors = new Behavior();
      }

      public Behavior Behaviors { get; private set; }

      [ContractInvariantMethod]
      private void ObjectInvariant() {
         Contract.Invariant(Behaviors != null);
      }
   }
}
