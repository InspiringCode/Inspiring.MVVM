using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using System.Windows.Input;
using Inspiring.Mvvm.ViewModels.Fluent;
namespace Inspiring.Mvvm.ViewModels.Core.Builder.Properties {

   internal sealed class VMPropertyFactory<TVM, TSource> :
      IVMPropertyFactory<TVM, TSource>
      where TVM : IViewModel {

      private readonly VMDescriptorConfiguration _configuration;

      public VMPropertyFactory(VMDescriptorConfiguration configuration) {
         Contract.Requires(configuration != null);

         _configuration = configuration;
      }

      public IVMPropertyFactoryWithSource<TVM, T> Mapped<T>(
         Expression<Func<TSource, T>> sourcePropertySelector
      ) {
         throw new NotImplementedException();
      }

      public IVMPropertyFactoryWithSource<TVM, T> Calculated<T>(
         Func<TSource, T> getter,
         Action<TSource, T> setter = null
      ) {
         throw new NotImplementedException();
      }

      public ILocalVMPropertyFactory<TVM> Local() {
         throw new NotImplementedException();
      }

      public IVMCollectionPropertyFactory<TVM> Collection() {
         throw new NotImplementedException();
      }

      public IVMCollectionPropertyFactoryWithSource<TVM, TItemSource> Collection<TItemSource>(
         Func<TSource, IEnumerable<TItemSource>> sourceCollectionSelector
      ) {
         throw new NotImplementedException();
      }

      public VMProperty<ICommand> Command(
         Action<TSource> execute,
         Func<TSource, bool> canExecute = null
      ) {
         throw new NotImplementedException();
      }
   }
}
