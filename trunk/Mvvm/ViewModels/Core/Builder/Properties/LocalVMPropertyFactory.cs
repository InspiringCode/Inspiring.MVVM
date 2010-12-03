namespace Inspiring.Mvvm.ViewModels.Core.Builder.Properties {
   using System;
   using Inspiring.Mvvm.ViewModels.Fluent;

   internal sealed class LocalVMPropertyFactory<TVM> :
      ILocalVMPropertyFactory<TVM>
      where TVM : IViewModel {

      public VMProperty<T> Property<T>() {
         throw new NotImplementedException();
      }

      public VMProperty<TChildVM> VM<TChildVM>() where TChildVM : IViewModel {
         throw new NotImplementedException();
      }

      public VMDescriptorConfiguration GetConfiguration() {
         throw new NotImplementedException();
      }
   }
}
