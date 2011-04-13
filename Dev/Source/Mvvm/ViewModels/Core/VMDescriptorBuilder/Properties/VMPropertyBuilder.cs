namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Collections.Generic;
   using System.Diagnostics.Contracts;
   using System.Linq.Expressions;
   using System.Windows.Input;
   using Inspiring.Mvvm.Common;

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
         Factory = new VMPropertyFactory<TVM, TSourceObject>(configuration);
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

      private VMPropertyFactory<TVM, TSourceObject> Factory { get; set; }

      /// <inheritdoc />
      IVMPropertyDescriptor<T> IValuePropertyBuilder<TSourceObject>.MapsTo<T>(
         Expression<Func<TSourceObject, T>> sourcePropertySelector
      ) {
         var path = PropertyPath.Concat(
            _sourceObjectPath, 
            PropertyPath.Create(sourcePropertySelector)
         );

         path = path.ReturnDefaultIfNull();

         return Factory.CreatePropertyWithSource(new MappedValueAccessorBehavior<TVM, T>(path));
      }

      /// <inheritdoc />
      IVMPropertyDescriptor<T> IValuePropertyBuilder<TSourceObject>.DelegatesTo<T>(
         Func<TSourceObject, T> getter,
         Action<TSourceObject, T> setter
      ) {
         var sourceValueAccessor = new DelegateValueAccessor<TVM, TSourceObject, T>(
            _sourceObjectPath,
            getter,
            setter
         );

         return Factory.CreatePropertyWithSource<T>(sourceValueAccessor);
      }

      /// <inheritdoc />
      IVMPropertyDescriptor<T> IValuePropertyBuilder<TSourceObject>.Of<T>() {
         return Factory.CreateProperty<T>(new StoredValueAccessorBehavior<T>());
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
            sourceValueAccessor: new MappedValueAccessorBehavior<TVM, TSourceValue>(path)
         );
      }

      /// <inheritdoc />
      IViewModelPropertyBuilderWithSource<TSourceValue> IViewModelPropertyBuilder<TSourceObject>.Wraps<TSourceValue>(
         Func<TSourceObject, TSourceValue> getter,
         Action<TSourceObject, TSourceValue> setter
      ) {
         var sourceValueAccessor = new DelegateValueAccessor<TVM, TSourceObject, TSourceValue>(
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
            viewModelAccessor: new DelegateViewModelAccessorBehavior<TChildVM>(),
            sourceAccessor: new DelegateValueAccessor<TVM, TSourceObject, TChildVM>(
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
            viewModelAccessor: new StoredValueAccessorBehavior<TChildVM>(),
            needsViewModelFactory: false,
            cachesValue: false,
            refreshBehavior: new RefreshBehavior.ViewModelProperty<TChildVM>()
         );
      }



      /// <inheritdoc />
      ICollectionPropertyBuilderWithSource<TItemSource> ICollectionPropertyBuilder<TSourceObject>.Wraps<TItemSource>(
         Func<TSourceObject, IEnumerable<TItemSource>> sourceCollectionSelector
      ) {
         var sourceValueAccessor = new DelegateValueAccessor<TVM, TSourceObject, IEnumerable<TItemSource>>(
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
         throw new NotImplementedException();

         //var commandConfig = BehaviorChainConfiguration.GetConfiguration(
         //   BehaviorChainTemplateKeys.CommandBehaviors,
         //   FactoryConfigurations.ForSimpleProperty<TVM, ICommand, TSourceObject>()
         //);

         //commandConfig.Enable(
         //   PropertyBehaviorKeys.CommandExecutor,
         //   new DelegatingCommandBehavior<TSourceObject>(execute, canExecute)
         //);

         //commandConfig.Enable(
         //   PropertyBehaviorKeys.SourceAccessor,
         //   new MappedPropertyAccessor<TVM, TSourceObject>(_sourceObjectPath)
         //);

         //var config = Factory.GetPropertyConfiguration<ICommand>(BehaviorChainTemplateKeys.CommandProperty);

         //config.Enable(
         //   PropertyBehaviorKeys.CommandFactory,
         //   new CommandFactoryBehavior(commandConfig)
         //);

         //return Factory.CreateAndRegisterProperty<ICommand>(config);
      }

      private MappedValueAccessorBehavior<TVM, TSourceObject> GetSourceObjectAccessor() {
         return new MappedValueAccessorBehavior<TVM, TSourceObject>(_sourceObjectPath);
      }

      private class CollectionPropertyBuilderWithSource<TItemSource> :
         ICollectionPropertyBuilderWithSource<TItemSource> {

         private IValueAccessorBehavior<IEnumerable<TItemSource>> _sourceCollectionAccessor;

         public CollectionPropertyBuilderWithSource(
            VMPropertyFactory<TVM, TSourceObject> factory,
            IValueAccessorBehavior<IEnumerable<TItemSource>> sourceCollectionAccessor
         ) {
            Contract.Requires(sourceCollectionAccessor != null);
            _sourceCollectionAccessor = sourceCollectionAccessor;
            Factory = factory;
         }

         private VMPropertyFactory<TVM, TSourceObject> Factory { get; set; }

         IVMPropertyDescriptor<IVMCollection<TItemVM>> ICollectionPropertyBuilderWithSource<TItemSource>.With<TItemVM>(
            VMDescriptorBase itemDescriptor
         ) {
            var collectionConfiguration = Factory.GetCollectionConfiguration<TItemVM>(itemDescriptor);

            collectionConfiguration.Enable(CollectionBehaviorKeys.SourceSynchronizer, new SynchronizerCollectionBehavior<TItemVM, TItemSource>());
            collectionConfiguration.Enable(CollectionBehaviorKeys.SourceAccessor, _sourceCollectionAccessor);
            collectionConfiguration.Enable(CollectionBehaviorKeys.ViewModelFactory, new ServiceLocatorValueFactoryBehavior<TItemVM>());
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
            VMPropertyFactory<TVM, TSourceObject> factory,
            IValueAccessorBehavior<TSourceValue> sourceValueAccessor
         ) {
            Contract.Requires(sourceValueAccessor != null);
            _sourceValueAccessor = sourceValueAccessor;
            Factory = factory;
         }

         private VMPropertyFactory<TVM, TSourceObject> Factory { get; set; }

         IVMPropertyDescriptor<TChildVM> IViewModelPropertyBuilderWithSource<TSourceValue>.With<TChildVM>() {
            return Factory.CreateViewModelProperty(
               viewModelAccessor: new WrapperViewModelAccessorBehavior<TChildVM, TSourceValue>(),
               manualUpdateBehavior: new ManualUpdateViewModelPropertyBehavior<TChildVM, TSourceValue>(),
               sourceAccessor: _sourceValueAccessor,
               //sourceSetter: new ViewModelSourceSetterBehavior<TChildVM, TSourceValue>(),
               needsViewModelFactory: false,
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
            VMPropertyFactory<TVM, TSourceObject> factory,
            IValueAccessorBehavior<TSourceObject> sourceObjectAccessor,
            IPopulatorCollectionBehavior<TItemVM> collectionPopulator
         ) {
            Contract.Requires(sourceObjectAccessor != null);
            Contract.Requires(collectionPopulator != null);

            _sourceObjectAccessor = sourceObjectAccessor;
            _collectionPopulator = collectionPopulator;
            Factory = factory;
         }

         private VMPropertyFactory<TVM, TSourceObject> Factory { get; set; }

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
            needsViewModelFactory: false,
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
