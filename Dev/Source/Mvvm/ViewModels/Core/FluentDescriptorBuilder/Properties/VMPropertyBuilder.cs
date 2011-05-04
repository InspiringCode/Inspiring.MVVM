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

      public VMPropertyBuilder(
         PropertyPath<TVM, TSourceObject> sourceObjectPath,
         VMDescriptorConfiguration configuration
      )
         : base(configuration) {

         Contract.Requires(sourceObjectPath != null);
         Contract.Requires(configuration != null);

         Custom = new VMPropertyFactory<TVM, TSourceObject>(configuration, sourceObjectPath);
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
      public ICustomPropertyFactory<TSourceObject> Custom {
         get;
         private set;
      }


      //
      //   V A L U E   P R O P E R T I E S
      //

      /// <inheritdoc />
      IVMPropertyDescriptor<T> IValuePropertyBuilder<TSourceObject>.MapsTo<T>(
         Expression<Func<TSourceObject, T>> sourcePropertySelector
      ) {
         return Custom.PropertyWithSource(
            Custom.CreateMappedAccessor(sourcePropertySelector)
         );
      }

      /// <inheritdoc />
      IVMPropertyDescriptor<T> IValuePropertyBuilder<TSourceObject>.DelegatesTo<T>(
         Func<TSourceObject, T> getter,
         Action<TSourceObject, T> setter
      ) {
         return Custom.PropertyWithSource<T>(
            Custom.CreateDelegateAccessor(getter, setter)
         );
      }

      /// <inheritdoc />
      IVMPropertyDescriptor<T> IValuePropertyBuilder<TSourceObject>.Of<T>() {
         return Custom.Property<T>(new StoredValueAccessorBehavior<T>());
      }


      //
      //   V I E W   M O D E L   P R O P E R T I E S
      //

      /// <inheritdoc />
      IViewModelPropertyBuilderWithSource<TSourceValue> IViewModelPropertyBuilder<TSourceObject>.Wraps<TSourceValue>(
         Expression<Func<TSourceObject, TSourceValue>> sourceValueSelector
      ) {
         return new ViewModelPropertyBuilderWithSource<TSourceValue>(
            Custom,
            sourceValueAccessor: Custom.CreateMappedAccessor(sourceValueSelector)
         );
      }

      /// <inheritdoc />
      IViewModelPropertyBuilderWithSource<TSourceValue> IViewModelPropertyBuilder<TSourceObject>.Wraps<TSourceValue>(
         Func<TSourceObject, TSourceValue> getter,
         Action<TSourceObject, TSourceValue> setter
      ) {
         return new ViewModelPropertyBuilderWithSource<TSourceValue>(
            Custom,
            sourceValueAccessor: Custom.CreateDelegateAccessor(getter, setter)
         );
      }

      /// <inheritdoc />
      IVMPropertyDescriptor<TChildVM> IViewModelPropertyBuilder<TSourceObject>.DelegatesTo<TChildVM>(
         Func<TSourceObject, TChildVM> getter,
         Action<TSourceObject, TChildVM> setter
      ) {
         return Custom.ViewModelProperty(
            valueAccessor: new DelegateViewModelAccessorBehavior<TChildVM>(),
            sourceAccessor: Custom.CreateDelegateAccessor(getter, setter)
         );
      }

      /// <inheritdoc />
      IVMPropertyDescriptor<TChildVM> IViewModelPropertyBuilder<TSourceObject>.Of<TChildVM>() {
         return Custom.ViewModelProperty(
            valueAccessor: new StoredViewModelAccessorBehavior<TChildVM>()
         );
      }


      //
      //   C O L L E C T I O N   P R O P E R T I E S
      //

      /// <inheritdoc />
      ICollectionPropertyBuilderWithSource<TItemSource> ICollectionPropertyBuilder<TSourceObject>.Wraps<TItemSource>(
         Func<TSourceObject, IEnumerable<TItemSource>> sourceCollectionSelector
      ) {
         return new CollectionPropertyBuilderWithSource<TItemSource>(
            Custom,
            sourceCollectionAccessor: Custom.CreateDelegateAccessor(sourceCollectionSelector)
         );
      }

      /// <inheritdoc />
      IPopulatedCollectionPropertyBuilder<TItemVM> ICollectionPropertyBuilder<TSourceObject>.PopulatedWith<TItemVM>(
         Func<TSourceObject, IEnumerable<TItemVM>> itemsProvider
      ) {
         return new PopulatedCollectionPropertyBuilder<TItemVM>(
            Custom,
            sourceCollectionAccessor: Custom.CreateDelegateAccessor(itemsProvider)
         );
      }

      /// <inheritdoc />
      IVMPropertyDescriptor<IVMCollection<TItemVM>> ICollectionPropertyBuilder<TSourceObject>.Of<TItemVM>(
         IVMDescriptor itemDescriptor
      ) {
         return Custom.CollectionProperty<TItemVM>(
            itemDescriptor,
            new StoredCollectionAccessorBehavior<TItemVM>()
         );
      }


      //
      //   C O M M A N D   P R O P E R T I E S
      //

      /// <inheritdoc />
      public IVMPropertyDescriptor<ICommand> Command(
         Action<TSourceObject> executeAction,
         Func<TSourceObject, bool> canExecutePredicate = null
      ) {
         return Custom.CommandProperty(
            sourceObjectAccessor: Custom.CreateSourceObjectAccessor(),
            commandExecutor: new DelegateCommandExecutorBehavior<TSourceObject>(
               executeAction,
               canExecutePredicate
            )
         );
      }


      //
      //   N E S T E D   B U I L D E R   C L A S S E S
      //

      private class ViewModelPropertyBuilderWithSource<TSourceValue> :
         IViewModelPropertyBuilderWithSource<TSourceValue> {

         private readonly IValueAccessorBehavior<TSourceValue> _sourceValueAccessor;
         private readonly ICustomPropertyFactory<TSourceObject> _factory;

         public ViewModelPropertyBuilderWithSource(
            ICustomPropertyFactory<TSourceObject> factory,
            IValueAccessorBehavior<TSourceValue> sourceValueAccessor
         ) {
            _sourceValueAccessor = sourceValueAccessor;
            _factory = factory;
         }

         IVMPropertyDescriptor<TChildVM> IViewModelPropertyBuilderWithSource<TSourceValue>.With<TChildVM>() {
            return _factory.ViewModelPropertyWithSource(
               valueAccessor: new WrapperViewModelAccessorBehavior<TChildVM, TSourceValue>(),
               sourceAccessor: _sourceValueAccessor
            );
         }
      }

      private class PopulatedCollectionPropertyBuilder<TItemVM> :
         IPopulatedCollectionPropertyBuilder<TItemVM>
         where TItemVM : IViewModel {

         private readonly ICustomPropertyFactory<TSourceObject> _factory;
         private readonly IValueAccessorBehavior<IEnumerable<TItemVM>> _sourceCollectionAccessor;

         public PopulatedCollectionPropertyBuilder(
            ICustomPropertyFactory<TSourceObject> factory,
            IValueAccessorBehavior<IEnumerable<TItemVM>> sourceCollectionAccessor
         ) {
            _factory = factory;
            _sourceCollectionAccessor = sourceCollectionAccessor;
         }

         public IVMPropertyDescriptor<IVMCollection<TItemVM>> With(IVMDescriptor itemDescriptor) {
            return _factory.CollectionProperty(
               itemDescriptor,
               valueAccessor: new PopulatedCollectionAccessorBehavior<TItemVM>(),
               sourceAccessor: _sourceCollectionAccessor
            );
         }
      }

      private class CollectionPropertyBuilderWithSource<TItemSource> :
         ICollectionPropertyBuilderWithSource<TItemSource> {

         private readonly ICustomPropertyFactory<TSourceObject> _factory;
         private readonly IValueAccessorBehavior<IEnumerable<TItemSource>> _sourceCollectionAccessor;

         public CollectionPropertyBuilderWithSource(
            ICustomPropertyFactory<TSourceObject> factory,
            IValueAccessorBehavior<IEnumerable<TItemSource>> sourceCollectionAccessor
         ) {
            _sourceCollectionAccessor = sourceCollectionAccessor;
            _factory = factory;
         }

         IVMPropertyDescriptor<IVMCollection<TItemVM>> ICollectionPropertyBuilderWithSource<TItemSource>.With<TItemVM>(
            IVMDescriptor itemDescriptor
         ) {
            return _factory.CollectionPropertyWithSource(
               itemDescriptor,
               valueAccessor: new WrapperCollectionAccessorBehavior<TItemVM, TItemSource>(),
               sourceAccessor: _sourceCollectionAccessor
            );
         }
      }
   }
}
