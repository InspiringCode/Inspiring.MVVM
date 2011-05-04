namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Collections.Generic;
   using System.Linq.Expressions;
   using System.Windows.Input;
   using Inspiring.Mvvm.Common;

   internal sealed class VMPropertyFactory<TOwnerVM, TSourceObject> :
      ConfigurationProvider,
      ICustomPropertyFactory<TSourceObject> where TOwnerVM : IViewModel {

      private readonly PropertyPath<TOwnerVM, TSourceObject> _sourceObjectPath;

      internal VMPropertyFactory(
         VMDescriptorConfiguration configuration,
         PropertyPath<TOwnerVM, TSourceObject> sourceObjectPath
      )
         : base(configuration) {
         _sourceObjectPath = sourceObjectPath;
      }

      public IVMPropertyDescriptor<TValue> Property<TValue>(IValueAccessorBehavior<TValue> valueAccessor) {
         return CustomProperty<TValue>(
            DefaultBehaviorChainTemplateKeys.Property,
            BehaviorFactoryConfigurations.ForSimpleProperty<TOwnerVM, TValue, TSourceObject>(false),
            config => config.Enable(PropertyBehaviorKeys.ValueAccessor, valueAccessor)
         );
      }

      public IVMPropertyDescriptor<TValue> PropertyWithSource<TValue>(
         IValueAccessorBehavior<TValue> valueAccessor
      ) {
         return CustomProperty<TValue>(
            DefaultBehaviorChainTemplateKeys.PropertyWithSource,
            BehaviorFactoryConfigurations.ForSimpleProperty<TOwnerVM, TValue, TSourceObject>(true),
            config => config.Enable(PropertyBehaviorKeys.ValueAccessor, valueAccessor)
         );
      }

      public IVMPropertyDescriptor<TChildVM> ViewModelProperty<TChildVM>(
         IValueAccessorBehavior<TChildVM> valueAccessor,
         IBehavior sourceAccessor = null
      ) where TChildVM : IViewModel {
         return CustomProperty<TChildVM>(
            DefaultBehaviorChainTemplateKeys.ViewModelProperty,
            BehaviorFactoryConfigurations.ForChildProperty<TOwnerVM, TChildVM, TSourceObject>(),
            config => {
               config.Enable(PropertyBehaviorKeys.ValueAccessor, valueAccessor);

               if (sourceAccessor != null) {
                  config.Enable(PropertyBehaviorKeys.SourceAccessor, sourceAccessor);
               }
            }
         );
      }

      public IVMPropertyDescriptor<TChildVM> ViewModelPropertyWithSource<TChildVM, TChildSource>(
         IValueAccessorBehavior<TChildVM> valueAccessor,
         IValueAccessorBehavior<TChildSource> sourceAccessor = null
      ) where TChildVM : IViewModel, IHasSourceObject<TChildSource> {
         return CustomProperty<TChildVM>(
            DefaultBehaviorChainTemplateKeys.ViewModelPropertyWithSource,
            BehaviorFactoryConfigurations.ForChildProperty<TOwnerVM, TChildVM, TSourceObject, TChildSource>(),
            config => {
               config.Enable(PropertyBehaviorKeys.ValueAccessor, valueAccessor);

               if (sourceAccessor != null) {
                  config.Enable(PropertyBehaviorKeys.SourceAccessor, sourceAccessor);
               }
            }
         );
      }

      public IVMPropertyDescriptor<IVMCollection<TChildVM>> CollectionProperty<TChildVM>(
         IVMDescriptor itemDescriptor,
         IValueAccessorBehavior<IVMCollection<TChildVM>> valueAccessor,
         IValueAccessorBehavior<IEnumerable<TChildVM>> sourceAccessor = null
      ) where TChildVM : IViewModel {
         return CustomProperty<IVMCollection<TChildVM>>(
            DefaultBehaviorChainTemplateKeys.CollectionProperty,
            BehaviorFactoryConfigurations.ForChildProperty<TOwnerVM, TChildVM, TSourceObject>(),
            config => {
               config.Enable(PropertyBehaviorKeys.ValueAccessor, valueAccessor);

               config.Enable(
                  CollectionPropertyBehaviorKeys.ItemDescriptorProvider,
                  new ItemDescriptorProviderBehavior(itemDescriptor)
               );

               if (sourceAccessor != null) {
                  config.Enable(PropertyBehaviorKeys.SourceAccessor, sourceAccessor);
               }
            }
         );
      }

      public IVMPropertyDescriptor<IVMCollection<TChildVM>> CollectionPropertyWithSource<TChildVM, TChildSource>(
         IVMDescriptor itemDescriptor,
         IValueAccessorBehavior<IVMCollection<TChildVM>> valueAccessor,
         IValueAccessorBehavior<IEnumerable<TChildSource>> sourceAccessor = null
      ) where TChildVM : IViewModel, IHasSourceObject<TChildSource> {
         return CustomProperty<IVMCollection<TChildVM>>(
            DefaultBehaviorChainTemplateKeys.CollectionPropertyWithSource,
            BehaviorFactoryConfigurations.ForChildProperty<TOwnerVM, TChildVM, TSourceObject, TChildSource>(),
            config => {
               config.Enable(PropertyBehaviorKeys.ValueAccessor, valueAccessor);

               config.Enable(
                  CollectionPropertyBehaviorKeys.ItemDescriptorProvider,
                  new ItemDescriptorProviderBehavior(itemDescriptor)
               );

               if (sourceAccessor != null) {
                  config.Enable(PropertyBehaviorKeys.SourceAccessor, sourceAccessor);
               }
            }
         );
      }

      public IVMPropertyDescriptor<ICommand> CommandProperty(
         IValueAccessorBehavior<TSourceObject> sourceObjectAccessor,
         IBehavior commandExecutor
      ) {
         return CustomProperty<ICommand>(
            DefaultBehaviorChainTemplateKeys.CommandProperty,
            BehaviorFactoryConfigurations.ForSimpleProperty<TOwnerVM, ICommand, TSourceObject>(false),
            config => {
               config.Enable(PropertyBehaviorKeys.SourceAccessor, sourceObjectAccessor);
               config.Enable(CommandPropertyBehaviorKeys.CommandExecutor, commandExecutor);
            }
         );
      }

      public IVMPropertyDescriptor<TValue> CustomProperty<TValue>(
         BehaviorChainTemplateKey templateKey,
         IBehaviorFactoryConfiguration factoryConfiguration,
         Action<BehaviorChainConfiguration> chainConfigurationAction
      ) {
         var config = BehaviorChainConfiguration.GetConfiguration(templateKey, factoryConfiguration);
         chainConfigurationAction(config);
         return CustomProperty<TValue>(config);
      }

      public IVMPropertyDescriptor<TValue> CustomProperty<TValue>(BehaviorChainConfiguration behaviorChain) {
         var property = new VMPropertyDescriptor<TValue>();

         Configuration
           .PropertyConfigurations
           .RegisterProperty(property, behaviorChain);

         return property;
      }

      public IValueAccessorBehavior<TValue> CreateDelegateAccessor<TValue>(
         Func<TSourceObject, TValue> getter,
         Action<TSourceObject, TValue> setter = null
      ) {
         return new DelegateValueAccessor<TOwnerVM, TSourceObject, TValue>(
            _sourceObjectPath,
            getter,
            setter
         );
      }

      public IValueAccessorBehavior<TValue> CreateMappedAccessor<TValue>(
         Expression<Func<TSourceObject, TValue>> valueSelector
      ) {
         var path = PropertyPath.Concat(
            _sourceObjectPath,
            PropertyPath.CreateWithDefaultValue(valueSelector)
         );

         return new MappedValueAccessorBehavior<TOwnerVM, TValue>(path);
      }

      public IValueAccessorBehavior<TSourceObject> CreateSourceObjectAccessor() {
         return new MappedValueAccessorBehavior<TOwnerVM, TSourceObject>(_sourceObjectPath);
      }
   }
}
