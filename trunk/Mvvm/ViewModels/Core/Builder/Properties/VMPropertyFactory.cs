using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using System.Windows.Input;
using Inspiring.Mvvm.Common;
using Inspiring.Mvvm.ViewModels.Fluent;
namespace Inspiring.Mvvm.ViewModels.Core.Builder.Properties {

   internal sealed class VMPropertyFactory<TVM, TSource> :
      ConfigurationProvider,
      IVMPropertyFactory<TVM, TSource>
      where TVM : IViewModel {

      private readonly PropertyPath<TVM, TSource> _sourceObjectPath;

      public VMPropertyFactory(
         PropertyPath<TVM, TSource> sourceObjectPath,
         VMDescriptorConfiguration configuration
      )
         : base(configuration) {
         Contract.Requires(sourceObjectPath != null);
         Contract.Requires(configuration != null);

         _sourceObjectPath = sourceObjectPath;
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
            Configuration,
            sourceValueAccessor
         );
      }

      /// <inheritdoc />
      public IVMPropertyFactoryWithSource<TVM, T> Calculated<T>(
         Func<TSource, T> getter,
         Action<TSource, T> setter = null
      ) {
         var sourceValueAccessor = new CalculatedPropertyAccessor<TVM, TSource, T>(
            _sourceObjectPath,
            getter,
            setter
         );

         return new VMPropertyFactoryWithSource<TVM, T>(
            Configuration,
            sourceValueAccessor
         );
      }

      /// <inheritdoc />
      public ILocalVMPropertyFactory<TVM> Local() {
         return new LocalVMPropertyLocal<TVM>(Configuration);
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
