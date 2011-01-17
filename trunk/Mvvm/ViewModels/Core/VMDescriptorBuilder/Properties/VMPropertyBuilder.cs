namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Collections.Generic;
   using System.Diagnostics.Contracts;
   using System.Linq.Expressions;
   using System.Windows.Input;
   using Inspiring.Mvvm.Common;
   using Inspiring.Mvvm.Screens;

   internal sealed class VMPropertyBuilder<TVM, TSourceObject> :
      ConfigurationProvider,
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
         Factory = new VMPropertyFactory<TVM>(configuration);
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

      private VMPropertyFactory<TVM> Factory { get; set; }

      /// <inheritdoc />
      IVMPropertyDescriptor<T> IValuePropertyBuilder<TSourceObject>.MapsTo<T>(
         Expression<Func<TSourceObject, T>> sourcePropertySelector
      ) {
         var path = PropertyPath.Concat(
            _sourceObjectPath,
            PropertyPath.CreateWithDefaultValue(sourcePropertySelector)
         );

         return Factory.CreateProperty(
            sourceValueAccessor: new MappedPropertyAccessor<TVM, T>(path),
            supportsManualUpdate: true,
            includeRefreshBehavior: true
         );
      }

      /// <inheritdoc />
      IVMPropertyDescriptor<T> IValuePropertyBuilder<TSourceObject>.DelegatesTo<T>(
         Func<TSourceObject, T> getter,
         Action<TSourceObject, T> setter
      ) {
         var sourceValueAccessor = new CalculatedPropertyAccessor<TVM, TSourceObject, T>(
            _sourceObjectPath,
            getter,
            setter
         );

         return Factory.CreateProperty(
            sourceValueAccessor,
            supportsManualUpdate: true,
            includeRefreshBehavior: true
         );
      }

      /// <inheritdoc />
      IVMPropertyDescriptor<T> IValuePropertyBuilder<TSourceObject>.Of<T>() {
         return Factory.CreateProperty(
            sourceValueAccessor: new InstancePropertyBehavior<T>(),
            supportsManualUpdate: false,
            includeRefreshBehavior: false
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
            Factory,
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
            Factory,
            sourceValueAccessor
         );
      }

      /// <inheritdoc />
      IVMPropertyDescriptor<TChildVM> IViewModelPropertyBuilder<TSourceObject>.DelegatesTo<TChildVM>(
         Func<TSourceObject, TChildVM> getter,
         Action<TSourceObject, TChildVM> setter
      ) {
         return Factory.CreateViewModelProperty(
            viewModelAccessor: new CalculatedPropertyAccessor<TVM, TSourceObject, TChildVM>(
               _sourceObjectPath,
               getter,
               setter
            ),
            cachesValue: true,
            refreshBehavior: new RefreshBehavior.ViewModelProperty<TChildVM>()
         );
      }

      /// <inheritdoc />
      IVMPropertyDescriptor<TChildVM> IViewModelPropertyBuilder<TSourceObject>.Of<TChildVM>() {
         return Factory.CreateViewModelProperty(
            viewModelAccessor: new InstancePropertyBehavior<TChildVM>(),
            needsViewModelFactory: false,
            cachesValue: false,
            refreshBehavior: new RefreshBehavior.ViewModelInstanceProperty<TChildVM>()
         );
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
            Factory,
            sourceValueAccessor
         );
      }

      /// <inheritdoc />
      IPopulatedCollectionPropertyBuilder<TItemVM> ICollectionPropertyBuilder<TSourceObject>.PopulatedWith<TItemVM>(
         Func<TSourceObject, IEnumerable<TItemVM>> itemsProvider
      ) {
         return new PopulatedCollectionPropertyBuilder<TItemVM>(
            Factory,
            GetSourceObjectAccessor(),
            new DelegatePopulatorCollectionBehavior<TItemVM, TSourceObject>(itemsProvider)
         );
      }

      /// <inheritdoc />
      IVMPropertyDescriptor<IVMCollection<TItemVM>> ICollectionPropertyBuilder<TSourceObject>.Of<TItemVM>(
         VMDescriptorBase itemDescriptor
      ) {
         return Factory.CreateCollectionProperty<TItemVM>(
            Factory.GetCollectionConfiguration<TItemVM>(itemDescriptor),
            isPopulatable: false,
            refreshBehavior: new RefreshBehavior.CollectionInstanceProperty<TItemVM>()
         );
      }

      /// <inheritdoc />
      IVMPropertyDescriptor<ICommand> IVMPropertyBuilder<TSourceObject>.Command(
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

         var config = Factory.GetPropertyConfiguration<ICommand>(BehaviorChainTemplateKeys.CommandProperty);
         config.Enable(BehaviorKeys.SourceAccessor, sourceValueAccessor);
         return Factory.CreateProperty<ICommand>(config);
      }

      private MappedPropertyAccessor<TVM, TSourceObject> GetSourceObjectAccessor() {
         return new MappedPropertyAccessor<TVM, TSourceObject>(_sourceObjectPath);
      }

      private class CollectionPropertyBuilderWithSource<TItemSource> :
         ICollectionPropertyBuilderWithSource<TItemSource> {

         private IValueAccessorBehavior<IEnumerable<TItemSource>> _sourceCollectionAccessor;

         public CollectionPropertyBuilderWithSource(
            VMPropertyFactory<TVM> factory,
            IValueAccessorBehavior<IEnumerable<TItemSource>> sourceCollectionAccessor
         ) {
            Contract.Requires(sourceCollectionAccessor != null);
            _sourceCollectionAccessor = sourceCollectionAccessor;
            Factory = factory;
         }

         private VMPropertyFactory<TVM> Factory { get; set; }

         IVMPropertyDescriptor<IVMCollection<TItemVM>> ICollectionPropertyBuilderWithSource<TItemSource>.With<TItemVM>(
            VMDescriptorBase itemDescriptor
         ) {
            var collectionConfiguration = Factory.GetCollectionConfiguration<TItemVM>(itemDescriptor);

            collectionConfiguration.Enable(CollectionBehaviorKeys.SourceSynchronizer, new SynchronizerCollectionBehavior<TItemVM, TItemSource>());
            collectionConfiguration.Enable(CollectionBehaviorKeys.SourceAccessor, _sourceCollectionAccessor);
            collectionConfiguration.Enable(CollectionBehaviorKeys.ViewModelFactory, new ViewModelFactoryBehavior<TItemVM>());
            collectionConfiguration.Enable(
               CollectionBehaviorKeys.Populator,
               new PopulatorCollectionBehavior<TItemVM, TItemSource>()
            );

            return Factory.CreateCollectionProperty<TItemVM>(
               collectionConfiguration,
               isPopulatable: true,
               refreshBehavior: new RefreshBehavior.PopulatedCollectionProperty<TItemVM>()
            );
         }
      }

      private class ViewModelPropertyBuilderWithSource<TSourceValue> :
         IViewModelPropertyBuilderWithSource<TSourceValue> {

         private IValueAccessorBehavior<TSourceValue> _sourceValueAccessor;

         public ViewModelPropertyBuilderWithSource(
            VMPropertyFactory<TVM> factory,
            IValueAccessorBehavior<TSourceValue> sourceValueAccessor
         ) {
            Contract.Requires(sourceValueAccessor != null);
            _sourceValueAccessor = sourceValueAccessor;
            Factory = factory;
         }

         private VMPropertyFactory<TVM> Factory { get; set; }

         IVMPropertyDescriptor<TChildVM> IViewModelPropertyBuilderWithSource<TSourceValue>.With<TChildVM>() {
            return Factory.CreateViewModelProperty(
               viewModelAccessor: new ViewModelWithSourceAcessorBehavior<TChildVM, TSourceValue>(),
               manualUpdateBehavior: new ManualUpdateViewModelPropertyBehavior<TChildVM, TSourceValue>(),
               sourceAccessor: _sourceValueAccessor,
               needsViewModelFactory: true,
               cachesValue: true,
               refreshBehavior: new RefreshBehavior.ViewModelProperty<TChildVM>()
            );
         }
      }

      private class PopulatedCollectionPropertyBuilder<TItemVM> :
         IPopulatedCollectionPropertyBuilder<TItemVM>
         where TItemVM : IViewModel {

         private IValueAccessorBehavior<TSourceObject> _sourceObjectAccessor;
         private IPopulatorCollectionBehavior<TItemVM> _collectionPopulator;

         public PopulatedCollectionPropertyBuilder(
            VMPropertyFactory<TVM> factory,
            IValueAccessorBehavior<TSourceObject> sourceObjectAccessor,
            IPopulatorCollectionBehavior<TItemVM> collectionPopulator
         ) {
            Contract.Requires(sourceObjectAccessor != null);
            Contract.Requires(collectionPopulator != null);

            _sourceObjectAccessor = sourceObjectAccessor;
            _collectionPopulator = collectionPopulator;
            Factory = factory;
         }

         private VMPropertyFactory<TVM> Factory { get; set; }

         public IVMPropertyDescriptor<IVMCollection<TItemVM>> With(VMDescriptorBase itemDescriptor) {
            var collectionConfiguration = Factory.GetCollectionConfiguration<TItemVM>(itemDescriptor);

            collectionConfiguration.Enable(CollectionBehaviorKeys.SourceAccessor, _sourceObjectAccessor);
            collectionConfiguration.Enable(CollectionBehaviorKeys.Populator, _collectionPopulator);

            return Factory.CreateCollectionProperty<TItemVM>(
               collectionConfiguration,
               isPopulatable: true,
               refreshBehavior: new RefreshBehavior.PopulatedCollectionProperty<TItemVM>()
            );
         }
      }


      public IVMPropertyDescriptor<TChildVM> Custom<TChildVM>(IValueAccessorBehavior<TChildVM> viewModelAccessor) where TChildVM : IViewModel {
         return Factory.CreateViewModelProperty(
            viewModelAccessor: viewModelAccessor,
            sourceAccessor: GetSourceObjectAccessor(),
            needsViewModelFactory: true,
            cachesValue: true,
            refreshBehavior: new RefreshBehavior.ViewModelProperty<TChildVM>()
         );
      }

      public IVMPropertyDescriptor<TChildVM> Custom<TChildVM, TChildSource>(
         IValueAccessorBehavior<TChildVM> viewModelAccessor
      ) where TChildVM : IViewModel, IHasSourceObject<TChildSource> {
         return Factory.CreateViewModelProperty(
            viewModelAccessor: viewModelAccessor,
            sourceAccessor: GetSourceObjectAccessor(),
            manualUpdateBehavior: new ManualUpdateViewModelPropertyBehavior<TChildVM, TChildSource>(),
            needsViewModelFactory: true,
            cachesValue: true,
            refreshBehavior: new RefreshBehavior.ViewModelProperty<TChildVM>()
         );
      }

      IVMPropertyDescriptor<T> IValuePropertyBuilder<TSourceObject>.Custom<T>(IValueAccessorBehavior<T> sourceValueAccessor) {
         return Factory.CreateProperty(
            sourceValueAccessor,
            supportsManualUpdate: true,
            includeRefreshBehavior: true
         );
      }
   }
}
