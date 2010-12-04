namespace Inspiring.Mvvm.ViewModels.Core.Builder.Properties {
   using System;
   using System.Diagnostics.Contracts;
   using Inspiring.Mvvm.ViewModels.Fluent;

   internal sealed class LocalVMPropertyLocal<TVM> :
      VMPropertyFactoryBase,
      ILocalVMPropertyFactory<TVM>
      where TVM : IViewModel {

      public LocalVMPropertyLocal(
         VMDescriptorConfiguration configuration
      )
         : base(configuration) {
         Contract.Requires(configuration != null);
      }

      public VMProperty<T> Property<T>() {
         var sourceValueAccessor = new InstancePropertyBehavior<T>();
         return CreateProperty(sourceValueAccessor);
      }

      public VMProperty<TChildVM> VM<TChildVM>() where TChildVM : IViewModel {
         throw new NotImplementedException();
      }
   }
}
