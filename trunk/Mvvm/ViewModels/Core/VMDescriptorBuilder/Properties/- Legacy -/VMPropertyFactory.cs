using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using System.Windows.Input;
using Inspiring.Mvvm.Common;
using Inspiring.Mvvm.Screens;
using Inspiring.Mvvm.ViewModels.Fluent;
namespace Inspiring.Mvvm.ViewModels.Core.Builder.Properties {

   internal sealed class VMPropertyFactory<TVM, TSource> :
      ConfigurationProvider,
      IVMPropertyFactory<TSource>
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
      public ILocalVMPropertyFactory Local {
         get {
            return new VMPropertyFactoryLocal<TVM>(Configuration);
         }
      }

      /// <inheritdoc />
      public IVMPropertyFactoryWithSource<T> Mapped<T>(
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
      public IVMPropertyFactoryWithSource<T> Calculated<T>(
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
      public IVMCollectionPropertyFactory<TSource> Collection() {
         return new VMCollectionPropertyFactory<TVM, TSource>(_sourceObjectPath, Configuration);
      }

      ///// <inheritdoc />
      //public IVMCollectionPropertyFactoryWithSource<TVM, TItemSource> Collection<TItemSource>(
      //   Func<TSource, IEnumerable<TItemSource>> sourceCollectionSelector
      //) {
      //   var sourceCollectionAccessor = new CalculatedPropertyAccessor<TVM, TSource, IEnumerable<TItemSource>>(
      //      _sourceObjectPath,
      //      sourceCollectionSelector
      //   );

      //   return new VMCollectionPropertyFactoryWithSource<TVM, TItemSource>(
      //      Configuration,
      //      sourceCollectionAccessor
      //   );
      //}

      public VMProperty<ICommand> Command(
         Action<TSource> execute,
         Func<TSource, bool> canExecute = null
      ) {
         // TODO: Is this really nice and clean?
         var sourceValueAccessor = new CalculatedPropertyAccessor<TVM, TSource, ICommand>(
            _sourceObjectPath,
            sourceObject => DelegateCommand.For(
               () => execute(sourceObject),
               canExecute != null ? () => canExecute(sourceObject) : (Func<bool>)null
            )
         );

         var template = BehaviorChainTemplateRegistry.GetTemplate(BehaviorChainTemplateKeys.CommandProperty);
         var invoker = PropertyBehaviorFactory.CreateInvoker<TVM, ICommand>();
         var configuration = template.CreateConfiguration(invoker);

         configuration.Enable(BehaviorKeys.SourceValueAccessor, sourceValueAccessor);

         var property = new VMProperty<ICommand>();

         Configuration
            .PropertyConfigurations
            .RegisterProperty(property, configuration);

         return property;
      }
   }
}
