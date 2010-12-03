namespace Inspiring.Mvvm.ViewModels.Core.Builder.Properties {
   using System;
   using Inspiring.Mvvm.ViewModels.Fluent;

   internal sealed class VMCollectionPropertyFactory<TVM> :
      IVMCollectionPropertyFactory<TVM>
      where TVM : IViewModel {

      public VMProperty<IVMCollection<TItemVM>> Of<TItemVM>() where TItemVM : IViewModel {
         throw new NotImplementedException();
      }
   }
}
