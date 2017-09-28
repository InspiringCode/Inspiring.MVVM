namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Linq.Expressions;
   using Inspiring.Mvvm.Common;

   internal sealed class VMPropertyBuilderProvider<TVM> :
      IVMPropertyBuilderProvider<TVM>
      where TVM : IViewModel {

      private readonly VMDescriptorConfiguration _configuration;

      public VMPropertyBuilderProvider(VMDescriptorConfiguration configuration) {
         Check.NotNull(configuration, nameof(configuration));
         _configuration = configuration;
      }

      public IVMPropertyBuilder<TVM> GetPropertyBuilder() {
         return new VMPropertyBuilder<TVM, TVM>(PropertyPath.Empty<TVM>(), _configuration);
      }

      public IVMPropertyBuilder<TSource> GetPropertyBuilder<TSource>(
         Expression<Func<TVM, TSource>> sourceObjectSelector
      ) {
         var path = PropertyPath.Create(sourceObjectSelector);
         return new VMPropertyBuilder<TVM, TSource>(path, _configuration);
      }
   }
}
