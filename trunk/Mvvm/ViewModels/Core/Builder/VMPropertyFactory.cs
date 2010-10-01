namespace Inspiring.Mvvm.ViewModels {
   using System;
   using System.Collections.Generic;
   using System.Linq.Expressions;
   using System.Windows.Input;
   using Inspiring.Mvvm.Common;
   using Inspiring.Mvvm.ViewModels.Core;

   /// <inheritdoc/>
   internal class VMPropertyFactory<TVM, TSource> : IVMPropertyFactory<TVM, TSource>, IRootVMPropertyFactory<TSource>, IBehaviorConfigurationDictionaryProvider where TVM : ViewModel {
      private PropertyPath<TVM, TSource> _sourceObjectPropertyPath;
      private BehaviorConfigurationDictionary _configurations;
      private BehaviorConfiguration _additionalConfiguration;

      /// <param name="sourceObjectPath">
      ///   Pass 'PropertyPath.Empty{TVM}' if you want to create a root factory
      ///   for the VM.
      /// </param>
      public VMPropertyFactory(
         PropertyPath<TVM, TSource> sourceObjectPath,
         BehaviorConfigurationDictionary configurations,
         BehaviorConfiguration additionalConfiguration = null
      ) {
         _sourceObjectPropertyPath = sourceObjectPath;
         _configurations = configurations;
         _additionalConfiguration = additionalConfiguration;
      }

      /// <inheritdoc/>
      public VMProperty<T> Mapped<T>(Expression<Func<TSource, T>> sourcePropertySelector) {
         var property = new VMProperty<T>();
         var config = BehaviorConfigurationFactory.CreateConfiguration();

         var propertyPath = PropertyPath.Concat(
            _sourceObjectPropertyPath,
            PropertyPath.CreateWithDefaultValue(sourcePropertySelector)
         );
         config.OverrideFactory(
            VMBehaviorKey.PropertyValueAcessor,
            new ConstantBehaviorFactory(
               new MappedPropertyBehavior<TVM, T>(propertyPath)
            )
         );

         AddToDictionary(property, config);
         return property;
      }

      /// <inheritdoc/>
      public VMProperty<T> Calculated<T>(Func<TSource, T> getter, Action<TSource, T> setter = null) {
         return Calculated<VMProperty<T>, T>(getter, setter);
         //var property = new VMProperty<T>();
         //var config = BehaviorConfigurationFactory.CreateConfiguration();

         //if (!_sourceObjectPropertyPath.IsEmpty) {
         //   config.OverrideFactory(
         //      VMBehaviorKey.SourceValueAccessor,
         //      new ConstantBehaviorFactory(
         //         new MappedPropertyBehavior<TVM, TSource>(_sourceObjectPropertyPath)
         //      )
         //   ).Enable(VMBehaviorKey.SourceValueAccessor);
         //}

         //config.OverrideFactory(
         //   VMBehaviorKey.PropertyValueAcessor,
         //   new ConstantBehaviorFactory(
         //      new CalculatedPropertyBehavior<TSource, T>(getter, setter)
         //   )
         //);

         //AddToDictionary(property, config);
         //return property;
      }

      public TProperty Calculated<TProperty, T>(Func<TSource, T> getter, Action<TSource, T> setter = null)
         where TProperty : VMPropertyBase<T>, new() {

         var property = new TProperty();
         var config = BehaviorConfigurationFactory.CreateConfiguration();

         if (!_sourceObjectPropertyPath.IsEmpty) {
            config.OverrideFactory(
               VMBehaviorKey.SourceValueAccessor,
               new ConstantBehaviorFactory(
                  new MappedPropertyBehavior<TVM, TSource>(_sourceObjectPropertyPath)
               )
            ).Enable(VMBehaviorKey.SourceValueAccessor);
         }

         config.OverrideFactory(
            VMBehaviorKey.PropertyValueAcessor,
            new ConstantBehaviorFactory(
               new CalculatedPropertyBehavior<TSource, T>(getter, setter)
            )
         );

         AddToDictionary(property, config);
         return property;
      }

      /// <inheritdoc/>
      public VMProperty<TValue> Local<TValue>() {
         VMProperty<TValue> property = new VMProperty<TValue>();
         BehaviorConfiguration config = BehaviorConfigurationFactory.CreateConfiguration();

         config.OverrideFactory(
            VMBehaviorKey.PropertyValueAcessor,
            new ConstantBehaviorFactory(
               new InstancePropertyBehavior<TValue>()
            )
         );

         AddToDictionary(property, config);
         return property;
      }

      /// <inheritdoc/>
      public IVMCollectionPropertyFactoryExpression<TVM, TItem> MappedCollection<TItem>(
         Expression<Func<TSource, IEnumerable<TItem>>> sourceCollectionSelector
      ) {
         var config = BehaviorConfigurationFactory.CreateCollectionConfiguration();

         PropertyPath<TVM, IEnumerable<TItem>> sourceCollectionPropertyPath = PropertyPath.Concat(
            _sourceObjectPropertyPath,
            PropertyPath.CreateWithDefaultValue(sourceCollectionSelector)
         );

         config.OverrideFactory(
            VMBehaviorKey.SourceValueAccessor,
            new ConstantBehaviorFactory(
               new MappedPropertyBehavior<TVM, IEnumerable<TItem>>(sourceCollectionPropertyPath)
            )
         );

         return new VMCollectionPropertyFactoryExpression<TVM, TItem>(
            new ConfiguredProperty(_configurations, _additionalConfiguration) { Configuration = config }
         );
      }

      IVMCollectionPropertyFactoryExpression<TSource, TItem> IRootVMPropertyFactory<TSource>.MappedCollection<TItem>(Expression<Func<TSource, IEnumerable<TItem>>> sourceCollectionSelector) {
         return (IVMCollectionPropertyFactoryExpression<TSource, TItem>)MappedCollection(sourceCollectionSelector);
      }

      public IVMViewModelPropertyFactoryExpression<TVMSource> MappedVM<TVMSource>(
         Expression<Func<TSource, TVMSource>> viewModelSourceSelector
      ) {
         var config = BehaviorConfigurationFactory.CreateViewModelPropertyConfiguration();

         PropertyPath<TVM, TVMSource> sourcePropertyPath = PropertyPath.Concat(
            _sourceObjectPropertyPath,
            PropertyPath.CreateWithDefaultValue(viewModelSourceSelector)
         );

         config.OverrideFactory(
            VMBehaviorKey.SourceValueAccessor,
            new ConstantBehaviorFactory(
               new MappedPropertyBehavior<TVM, TVMSource>(sourcePropertyPath)
            )
         );

         return new VMViewModelPropertyFactoryExpression<TVMSource>(
            new ConfiguredProperty(_configurations, _additionalConfiguration) { Configuration = config }
         );
      }

      public VMProperty<ICommand> Command(Action<TSource> execute, Func<TSource, bool> canExecute = null) {
         var config = BehaviorConfigurationFactory.CreateCommandPropertyConfiguration();

         config.OverrideFactory(
            VMBehaviorKey.PropertyValueAcessor,
            new ConstantBehaviorFactory(
               new DelegateCommandBehavior<TSource>(execute, canExecute)
            )
         );

         if (!_sourceObjectPropertyPath.IsEmpty) {
            config.OverrideFactory(
               VMBehaviorKey.SourceValueAccessor,
               new ConstantBehaviorFactory(
                  new MappedPropertyBehavior<TVM, TSource>(_sourceObjectPropertyPath)
               )
            ).Enable(VMBehaviorKey.SourceValueAccessor);
         }

         var property = new VMProperty<ICommand>();
         AddToDictionary(property, config);
         return property;
      }

      public VMPropertyFactory<TVM, TSource> WithConfiguration(BehaviorConfiguration additionalConfiguration) {
         BehaviorConfiguration newConfiguration;

         if (_additionalConfiguration != null) {
            newConfiguration = _additionalConfiguration.Clone();
            newConfiguration.MergeFrom(additionalConfiguration);
         } else {
            newConfiguration = additionalConfiguration;
         }
         return new VMPropertyFactory<TVM, TSource>(
            _sourceObjectPropertyPath,
            _configurations,
            newConfiguration
         );
      }

      BehaviorConfigurationDictionary IBehaviorConfigurationDictionaryProvider.Provide() {
         return _configurations;
      }

      private void AddToDictionary(VMProperty property, BehaviorConfiguration config) {
         new ConfiguredProperty(_configurations, _additionalConfiguration) {
            Property = property,
            Configuration = config
         }.AddToDictionary();
      }

      private class VMCollectionPropertyFactoryExpression<TParentVM, TItem> :
         IVMCollectionPropertyFactoryExpression<TParentVM, TItem>
         where TParentVM : ViewModel {

         private ConfiguredProperty _config;

         public VMCollectionPropertyFactoryExpression(ConfiguredProperty config) {
            _config = config;
         }

         // TODO: Make this more type safe?
         public VMCollectionProperty<TItemVM> Of<TItemVM>(VMDescriptor itemDescriptor) where TItemVM : ViewModel, ICanInitializeFrom<TItem> {
            _config.Configuration.OverrideFactory(
               VMBehaviorKey.CollectionPopulator,
               new ConstantBehaviorFactory(
                  new CollectionPopulatorBehavior<TParentVM, TItemVM, TItem>()
               )
            );

            _config.Configuration.OverrideFactory(
               VMBehaviorKey.CollectionFactory,
               new ConstantBehaviorFactory(
                  new CollectionFactoryBehavior<TItemVM>(itemDescriptor)
               )
            );

            _config.Configuration.OverrideFactory(
               VMBehaviorKey.ViewModelFactory,
               new ConstantBehaviorFactory(
                  new ViewModelFactoryBehavior<TItemVM>()
               )
            );

            var property = new VMCollectionProperty<TItemVM>();
            _config.Property = property;
            _config.AddToDictionary();
            return property;
         }


         public VMCollectionProperty<TItemVM> OfParentAware<TItemVM>(VMDescriptor itemDescriptor) where TItemVM : ViewModel, ICanInitializeFrom<SourceWithParent<TParentVM, TItem>> {
            _config.Configuration.OverrideFactory(
               VMBehaviorKey.CollectionPopulator,
               new ConstantBehaviorFactory(
                  new CollectionPopulatorBehavior<TParentVM, TItemVM, TItem>()
               )
            );

            _config.Configuration.OverrideFactory(
               VMBehaviorKey.CollectionFactory,
               new ConstantBehaviorFactory(
                  new CollectionFactoryBehavior<TItemVM>(itemDescriptor)
               )
            );

            _config.Configuration.OverrideFactory(
               VMBehaviorKey.ViewModelFactory,
               new ConstantBehaviorFactory(
                  new ViewModelFactoryBehavior<TItemVM>()
               )
            );

            var property = new VMCollectionProperty<TItemVM>();
            _config.Property = property;
            _config.AddToDictionary();
            return property;
         }
      }

      private class VMViewModelPropertyFactoryExpression<TVMSource> : IVMViewModelPropertyFactoryExpression<TVMSource> {
         private ConfiguredProperty _config;

         public VMViewModelPropertyFactoryExpression(ConfiguredProperty config) {
            _config = config;
         }

         public VMProperty<TVM> Of<TVM>() where TVM : ViewModel, ICanInitializeFrom<TVMSource> {
            _config.Configuration.OverrideFactory(
               VMBehaviorKey.ViewModelPropertyInitializer,
               new ConstantBehaviorFactory(
                  new ViewModelPropertyInitializerBehavior<TVM, TVMSource>()
               )
            );

            _config.Configuration.OverrideFactory(
               VMBehaviorKey.ViewModelFactory,
               new ConstantBehaviorFactory(
                  new ViewModelFactoryBehavior<TVM>()
               )
            );

            VMProperty<TVM> property = new VMProperty<TVM>();
            _config.Property = property;
            _config.AddToDictionary();
            return property;
         }
      }

      private class ConfiguredProperty {
         private BehaviorConfigurationDictionary _configurations;
         private BehaviorConfiguration _additionalConfiguration;

         public ConfiguredProperty(
            BehaviorConfigurationDictionary configurations,
            BehaviorConfiguration additionalConfiguration
         ) {
            _configurations = configurations;
            _additionalConfiguration = additionalConfiguration;
         }

         public BehaviorConfiguration Configuration { get; set; }

         public VMProperty Property { get; set; }

         public void AddToDictionary() {
            if (_additionalConfiguration != null) {
               Configuration.MergeFrom(_additionalConfiguration);
            }
            _configurations.Add(Property, Configuration);
         }
      }
   }
}
