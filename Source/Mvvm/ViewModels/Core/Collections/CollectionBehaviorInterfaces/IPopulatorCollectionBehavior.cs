namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Diagnostics.Contracts;
   using Contracts;

   [ContractClass(typeof(IPopulatorCollectionBehaviorContracts<>))]
   public interface IPopulatorCollectionBehavior<TItemVM> :
      IBehavior
      where TItemVM : IViewModel {

      void Repopulate(IBehaviorContext context, IVMCollection<TItemVM> collection);
   }

   namespace Contracts {
      [ContractClassFor(typeof(IPopulatorCollectionBehavior<>))]
      internal abstract class IPopulatorCollectionBehaviorContracts<TItemVM> :
         IPopulatorCollectionBehavior<TItemVM>
         where TItemVM : IViewModel {

         public void Repopulate(IBehaviorContext context, IVMCollection<TItemVM> collection) {
            Contract.Requires<ArgumentNullException>(context != null);
            Contract.Requires<ArgumentNullException>(collection != null);
         }

         public IBehavior Successor { get; set; }
      }

   }
}
