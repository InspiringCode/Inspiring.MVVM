namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Collections.Generic;
   using System.Diagnostics.Contracts;
   using System.Linq.Expressions;
   using System.Windows.Input;
   using Inspiring.Mvvm.Common;
   using Inspiring.Mvvm.Screens;
   using Inspiring.Mvvm.ViewModels.Fluent;

   internal sealed class VMPropertyBuilder<TVM, TSourceObject> :
      VMPropertyBuilderBase<TVM>,
      IVMPropertyBuilder<TSourceObject>,
      IValuePropertyBuilder<TSourceObject>,
      IViewModelPropertyBuilder<TSourceObject>,
      ICollectionPropertyBuilder<TSourceObject>
      where TVM : IViewModel {

      private readonly PropertyPath<TVM, TSourceObject> _sourceObjectPath;

      public VMPropertyBuilder(
         PropertyPath<TVM, TSourceObject> sourceObjectPath,
         VMDescriptorConfiguration configuration
      )
         : base(configuration) {
         Contract.Requires(sourceObjectPath != null);
         Contract.Requires(configuration != null);

         _sourceObjectPath = sourceObjectPath;
      }

      /// <inheritdoc />
      public IValuePropertyBuilder<TSourceObject> Property {
         get { return this; }
      }

      /// <inheritdoc />
      public IViewModelPropertyBuilder<TSourceObject> VM {
         get { return this; }
      }

      /// <inheritdoc />
      public ICollectionPropertyBuilder<TSourceObject> Collection {
         get { return this; }
      }

      /// <inheritdoc />
      VMProperty<T> IValuePropertyBuilder<TSourceObject>.MapsTo<T>(
         Expression<Func<TSourceObject, T>> sourcePropertySelector
      ) {
         var path = PropertyPath.Concat(
            _sourceObjectPath,
            PropertyPath.CreateWithDefaultValue(sourcePropertySelector)
         );

         return CreateProperty(
            sourceValueAccessor: new MappedPropertyAccessor<TVM, T>(path)
         );
      }

      /// <inheritdoc />
      VMProperty<T> IValuePropertyBuilder<TSourceObject>.DelegatesTo<T>(
         Func<TSourceObject, T> getter,
         Action<TSourceObject, T> setter
      ) {
         var sourceValueAccessor = new CalculatedPropertyAccessor<TVM, TSourceObject, T>(
            _sourceObjectPath,
            getter,
            setter
         );

         return CreateProperty(sourceValueAccessor);
      }

      /// <inheritdoc />
      VMProperty<T> IValuePropertyBuilder<TSourceObject>.Of<T>() {
         return CreateProperty(
            sourceValueAccessor: new InstancePropertyBehavior<T>()
         );
      }

      /// <inheritdoc />
      IViewModelPropertyBuilderWithSource<TSourceValue> IViewModelPropertyBuilder<TSourceObject>.Wraps<TSourceValue>(
         Expression<Func<TSourceObject, TSourceValue>> sourceValueSelector
      ) {
         var path = PropertyPath.Concat(
            _sourceObjectPath,
            PropertyPath.CreateWithDefaultValue(sourceValueSelector)
         );

         return new ViewModelPropertyBuilderWithSource<TSourceValue>(
            Configuration,
            sourceValueAccessor: new MappedPropertyAccessor<TVM, TSourceValue>(path)
         );
      }

      /// <inheritdoc />
      IViewModelPropertyBuilderWithSource<TSourceValue> IViewModelPropertyBuilder<TSourceObject>.Wraps<TSourceValue>(
         Func<TSourceObject, TSourceValue> getter,
         Action<TSourceObject, TSourceValue> setter
      ) {
         var sourceValueAccessor = new CalculatedPropertyAccessor<TVM, TSourceObject, TSourceValue>(
            _sourceObjectPath,
            getter,
            setter
         );

         return new ViewModelPropertyBuilderWithSource<TSourceValue>(
            Configuration,
            sourceValueAccessor
         );
      }

      /// <inheritdoc />
      VMProperty<TChildVM> IViewModelPropertyBuilder<TSourceObject>.CreatedBy<TChildVM>(
         Func<TSourceObject, TChildVM> viewModelFactory
      ) {
         return CreateViewModelProperty<TChildVM, TSourceObject>(
            sourceValueAccessor: GetSourceObjectAccessor(),
            customFactory: new DelegateViewModelFactory<TSourceObject, TChildVM>(viewModelFactory)
         );
      }

      /// <inheritdoc />
      VMProperty<TChildVM> IViewModelPropertyBuilder<TSourceObject>.Of<TChildVM>() {
         return CreateViewModelProperty<TChildVM>();
      }

      /// <inheritdoc />
      ICollectionPropertyBuilderWithSource<TItemSource> ICollectionPropertyBuilder<TSourceObject>.Wraps<TItemSource>(
         Func<TSourceObject, IEnumerable<TItemSource>> sourceCollectionSelector
      ) {
         var sourceValueAccessor = new CalculatedPropertyAccessor<TVM, TSourceObject, IEnumerable<TItemSource>>(
            _sourceObjectPath,
            sourceCollectionSelector,
            setter: null
         );

         return new CollectionPropertyBuilderWithSource<TItemSource>(
            Configuration,
            sourceValueAccessor
         );
      }

      /// <inheritdoc />
      IPopulatedCollectionPropertyBuilder<TItemVM> ICollectionPropertyBuilder<TSourceObject>.PopulatedWith<TItemVM>(
         Func<TSourceObject, IEnumerable<TItemVM>> itemsProvider
      ) {
         return new PopulatedCollectionPropertyBuilder<TItemVM>(
            Configuration,
            GetSourceObjectAccessor(),
            new DelegatePopulatorCollectionBehavior<TItemVM, TSourceObject>(itemsProvider)
         );
      }

      /// <inheritdoc />
      VMProperty<IVMCollection<TItemVM>> ICollectionPropertyBuilder<TSourceObject>.Of<TItemVM>(
         VMDescriptorBase itemDescriptor
      ) {
         return CreateCollectionProperty<TItemVM>(
            GetCollectionConfiguration<TItemVM>(itemDescriptor),
            isPopulatable: false
         );
      }

      /// <inheritdoc />
      VMProperty<ICommand> IVMPropertyBuilder<TSourceObject>.Command(
         Action<TSourceObject> execute,
         Func<TSourceObject, bool> canExecute
      ) {
         // TODO: Is this really nice and clean?
         var sourceValueAccessor = new CalculatedPropertyAccessor<TVM, TSourceObject, ICommand>(
            _sourceObjectPath,
            sourceObject => DelegateCommand.For(
               () => execute(sourceObject),
               canExecute != null ? () => canExecute(sourceObject) : (Func<bool>)null
            )
         );

         var config = GetPropertyConfiguration<ICommand>(BehaviorChainTemplateKeys.CommandProperty);
         config.Enable(BehaviorKeys.SourceValueAccessor, sourceValueAccessor);
         return CreateProperty<ICommand>(config);
      }

      private MappedPropertyAccessor<TVM, TSourceObject> GetSourceObjectAccessor() {
         return new MappedPropertyAccessor<TVM, TSourceObject>(_sourceObjectPath);
      }

      private class CollectionPropertyBuilderWithSource<TItemSource> :
         VMPropertyBuilderBase<TVM>,
         ICollectionPropertyBuilderWithSource<TItemSource> {

         private IValueAccessorBehavior<IEnumerable<TItemSource>> _sourceCollectionAccessor;

         public CollectionPropertyBuilderWithSource(
            VMDescriptorConfiguration configuration,
            IValueAccessorBehavior<IEnumerable<TItemSource>> sourceCollectionAccessor
         )
            : base(configuration) {
            Contract.Requires(sourceCollectionAccessor != null);
            _sourceCollectionAccessor = sourceCollectionAccessor;
         }

         VMProperty<IVMCollection<TItemVM>> ICollectionPropertyBuilderWithSource<TItemSource>.With<TItemVM>(
            VMDescriptorBase itemDescriptor
         ) {
            var collectionConfiguration = GetCollectionConfiguration<TItemVM>(itemDescriptor);

            collectionConfiguration.Enable(CollectionBehaviorKeys.SourceAccessor, _sourceCollectionAccessor);
            collectionConfiguration.Enable(CollectionBehaviorKeys.ViewModelFactory, new ViewModelFactoryBehavior<TItemVM>());
            collectionConfiguration.Enable(
               CollectionBehaviorKeys.Populator,
               new PopulatorCollectionBehavior<TItemVM, TItemSource>()
            );

            return CreateCollectionProperty<TItemVM>(
               collectionConfiguration,
               isPopulatable: true
            );
         }
      }

      private class ViewModelPropertyBuilderWithSource<TSourceValue> :
         VMPropertyBuilderBase<TVM>,
         IViewModelPropertyBuilderWithSource<TSourceValue> {

         private IValueAccessorBehavior<TSourceValue> _sourceValueAccessor;

         public ViewModelPropertyBuilderWithSource(
            VMDescriptorConfiguration configuration,
            IValueAccessorBehavior<TSourceValue> sourceValueAccessor
         )
            : base(configuration) {
            Contract.Requires(sourceValueAccessor != null);
            _sourceValueAccessor = sourceValueAccessor;
         }

         VMProperty<TChildVM> IViewModelPropertyBuilderWithSource<TSourceValue>.With<TChildVM>() {
            return CreateViewModelProperty<TChildVM, TSourceValue>(_sourceValueAccessor);
         }
      }

      private class PopulatedCollectionPropertyBuilder<TItemVM> :
         VMPropertyBuilderBase<TVM>,
         IPopulatedCollectionPropertyBuilder<TItemVM>
         where TItemVM : IViewModel {

         private IValueAccessorBehavior<TSourceObject> _sourceObjectAccessor;
         private IPopulatorCollectionBehavior<TItemVM> _collectionPopulator;

         public PopulatedCollectionPropertyBuilder(
            VMDescriptorConfiguration configuration,
            IValueAccessorBehavior<TSourceObject> sourceObjectAccessor,
            IPopulatorCollectionBehavior<TItemVM> collectionPopulator
         )
            : base(configuration) {
            Contract.Requires(sourceObjectAccessor != null);
            Contract.Requires(collectionPopulator != null);

            _sourceObjectAccessor = sourceObjectAccessor;
            _collectionPopulator = collectionPopulator;
         }

         public VMProperty<IVMCollection<TItemVM>> With(VMDescriptorBase itemDescriptor) {
            var collectionConfiguration = GetCollectionConfiguration<TItemVM>(itemDescriptor);

            collectionConfiguration.Enable(CollectionBehaviorKeys.SourceAccessor, _sourceObjectAccessor);
            collectionConfiguration.Enable(CollectionBehaviorKeys.Populator, _collectionPopulator);

            return CreateCollectionProperty<TItemVM>(
               collectionConfiguration,
               isPopulatable: true
            );
         }
      }
   }
}
