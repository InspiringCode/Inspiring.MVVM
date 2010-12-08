namespace Inspiring.Mvvm.ViewModels.Core.Builder.Properties {
   using System;
   using Inspiring.Mvvm.ViewModels.Fluent;
   using System.Diagnostics.Contracts;

   internal sealed class VMCollectionPropertyFactoryWithSource<TVM, TItemSource> :
      ConfigurationProvider,
      IVMCollectionPropertyFactoryWithSource<TVM, TItemSource>
      where TVM : IViewModel {

      public VMCollectionPropertyFactoryWithSource(
         VMDescriptorConfiguration configuration
      )
         : base(configuration) {
         Contract.Requires(configuration != null);
      }

      public VMProperty<IVMCollection<TItemVM>> Of<TItemVM>() where TItemVM : IViewModel, ICanInitializeFrom<TItemSource> {
         throw new NotImplementedException();
      }
   }
}
