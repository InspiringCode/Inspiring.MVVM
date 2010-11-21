namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Diagnostics.Contracts;
   using Contracts;

   [ContractClass(typeof(ICollectionPopulatorBehaviorContracts<>))]
   public interface ICollectionPopulatorBehavior<TItemVM> :
      IBehavior
      where TItemVM : IViewModel {

      void Repopulate(IBehaviorContext context, IVMCollection<TItemVM> collection);
   }

   namespace Contracts {
      [ContractClassFor(typeof(ICollectionPopulatorBehavior<>))]
      internal abstract class ICollectionPopulatorBehaviorContracts<TItemVM> :
         ICollectionPopulatorBehavior<TItemVM>
         where TItemVM : IViewModel {

         public void Repopulate(IBehaviorContext context, IVMCollection<TItemVM> collection) {
            Contract.Requires<ArgumentNullException>(context != null);
            Contract.Requires<ArgumentNullException>(collection != null);
         }

         public IBehavior Successor { get; set; }

         void IBehavior.Initialize(BehaviorInitializationContext context) {
            // TODO: Remove this method...
         }
      }

   }
}
