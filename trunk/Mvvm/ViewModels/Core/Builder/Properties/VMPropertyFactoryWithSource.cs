namespace Inspiring.Mvvm.ViewModels.Core.Builder.Properties {
   using System;
   using System.Collections.Generic;
   using System.Linq;
using Inspiring.Mvvm.ViewModels.Fluent;

   internal sealed class VMPropertyFactoryWithSource<TVM, TSourceObject> : 
      IVMPropertyFactoryWithSource<TVM, TSourceObject> {

      public VMProperty<TSourceObject> Property() {
         throw new NotImplementedException();
      }

      public VMProperty<TChildVM> VM<TChildVM>() where TChildVM : IViewModel, ICanInitializeFrom<TSourceObject> {
         throw new NotImplementedException();
      }
   }
}
