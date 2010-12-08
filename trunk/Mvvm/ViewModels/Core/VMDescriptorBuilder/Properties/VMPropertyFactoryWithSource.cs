namespace Inspiring.Mvvm.ViewModels.Core.Builder.Properties {
   using System;
   using System.Diagnostics.Contracts;
   using Inspiring.Mvvm.ViewModels.Fluent;

   internal sealed class VMPropertyFactoryWithSource<TVM, TSourceValue> :
      VMPropertyFactoryBase<TVM>,
      IVMPropertyFactoryWithSource<TVM, TSourceValue>
      where TVM : IViewModel {

      private readonly IValueAccessorBehavior<TSourceValue> _sourceValueAccessor;

      public VMPropertyFactoryWithSource(
         VMDescriptorConfiguration configuration,
         IValueAccessorBehavior<TSourceValue> sourceValueAccessor
      )
         : base(configuration) {
         Contract.Requires(configuration != null);
         Contract.Requires(sourceValueAccessor != null);

         _sourceValueAccessor = sourceValueAccessor;
      }

      public VMProperty<TSourceValue> Property() {
         return CreateProperty(_sourceValueAccessor);
      }

      public VMProperty<TChildVM> VM<TChildVM>() where TChildVM : IViewModel, ICanInitializeFrom<TSourceValue> {
         throw new NotImplementedException();
      }


      public IValueAccessorBehavior<TSourceValue> GetSourceAccessor() {
         throw new NotImplementedException();
      }
   }
}
