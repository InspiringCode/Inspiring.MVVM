namespace Inspiring.Mvvm.ViewModels.Core.Builder.Properties {
   using System;
   using System.Collections.Generic;
   using System.Linq;
using Inspiring.Mvvm.ViewModels.Fluent;

   internal sealed class VMCollectionPropertyFactoryWithSource<TVM, TItemSource> : 
      IVMCollectionPropertyFactoryWithSource<TVM, TItemSource>
      where TVM : IViewModel {

      public VMProperty<IVMCollection<TItemVM>> Of<TItemVM>() where TItemVM : IViewModel, ICanInitializeFrom<TItemSource> {
         throw new NotImplementedException();
      }
   }
}
