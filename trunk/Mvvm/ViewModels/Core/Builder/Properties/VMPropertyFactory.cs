using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using System.Windows.Input;
using Inspiring.Mvvm.Common;
using Inspiring.Mvvm.ViewModels.Fluent;
namespace Inspiring.Mvvm.ViewModels.Core.Builder.Properties {

   internal sealed class VMPropertyFactory<TVM, TSource> :
      IVMPropertyFactory<TVM, TSource>
      where TVM : IViewModel {

      private readonly PropertyPath<TVM, TSource> _sourceObjectPath;
      private readonly VMDescriptorConfiguration _configuration;

      public VMPropertyFactory(
         PropertyPath<TVM, TSource> sourceObjectPath,
         VMDescriptorConfiguration configuration
      ) {
         Contract.Requires(sourceObjectPath != null);
         Contract.Requires(configuration != null);

         _sourceObjectPath = sourceObjectPath;
         _configuration = configuration;
      }

      /// <inheritdoc />
      public IVMPropertyFactoryWithSource<TVM, T> Mapped<T>(
         Expression<Func<TSource, T>> sourcePropertySelector
      ) {
         var path = PropertyPath.Concat(
            _sourceObjectPath,
            PropertyPath.CreateWithDefaultValue(sourcePropertySelector)
         );

         var sourceValueAccessor = new MappedPropertyAccessor<TVM, T>(path);

         return new VMPropertyFactoryWithSource<TVM, T>(
            _configuration,
            sourceValueAccessor
         );
      }

      public IVMPropertyFactoryWithSource<TVM, T> Calculated<T>(
         Func<TSource, T> getter,
         Action<TSource, T> setter = null
      ) {
         //new CalculatedPropertyBehavior<,
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

      public VMDescriptorConfiguration GetConfiguration() {
         throw new NotImplementedException();
      }
   }
}
