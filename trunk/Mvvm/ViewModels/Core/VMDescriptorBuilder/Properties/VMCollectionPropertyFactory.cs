namespace Inspiring.Mvvm.ViewModels.Core.Builder.Properties {
   using System;
   using Inspiring.Mvvm.ViewModels.Fluent;
using System.Diagnostics.Contracts;

   internal sealed class VMCollectionPropertyFactory<TVM> :
      ConfigurationProvider,
      IVMCollectionPropertyFactory<TVM>
      where TVM : IViewModel {

      public VMCollectionPropertyFactory(
         VMDescriptorConfiguration configuration
      )
         : base(configuration) {
         Contract.Requires(configuration != null);
      }

      public VMProperty<IVMCollection<TItemVM>> Of<TItemVM>() where TItemVM : IViewModel {
         throw new NotImplementedException();
      }
   }
}
