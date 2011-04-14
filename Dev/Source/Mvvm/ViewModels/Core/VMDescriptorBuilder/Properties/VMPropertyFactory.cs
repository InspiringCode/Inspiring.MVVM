using System;
using System.Windows.Input;
using Inspiring.Mvvm.ViewModels.Core.Behaviors;
namespace Inspiring.Mvvm.ViewModels.Core {

   // TODO: Document and clean up the whole builder stuff a bit.
   public sealed class VMPropertyFactory<TOwnerVM, TSourceObject> : ConfigurationProvider where TOwnerVM : IViewModel {
      public VMPropertyFactory(VMDescriptorConfiguration configuration)
         : base(configuration) {
      }

      public IVMPropertyDescriptor<TValue> CreateProperty<TValue>(IValueAccessorBehavior<TValue> valueAccessor) {
         return CreateCustomProperty<TValue>(
            BehaviorChainTemplateKeys.Property,
            FactoryConfigurations.ForSimpleProperty<TOwnerVM, TValue, TSourceObject>(false),
            config => config.Enable(PropertyBehaviorKeys.ValueAccessor, valueAccessor)
         );
      }

      public IVMPropertyDescriptor<TValue> CreatePropertyWithSource<TValue>(
         IValueAccessorBehavior<TValue> valueAccessor
      ) {
         return CreateCustomProperty<TValue>(
            BehaviorChainTemplateKeys.Property,
            FactoryConfigurations.ForSimpleProperty<TOwnerVM, TValue, TSourceObject>(true),
            config => config.Enable(PropertyBehaviorKeys.ValueAccessor, valueAccessor)
         );
      }

      public IVMPropertyDescriptor<TChildVM> CreateViewModelProperty<TChildVM>(
         IValueAccessorBehavior<TChildVM> valueAccessor,
         IValueAccessorBehavior<TChildVM> sourceAccessor = null
      ) where TChildVM : IViewModel {
         return CreateCustomProperty<TChildVM>(
            BehaviorChainTemplateKeys.ViewModelProperty,
            FactoryConfigurations.ForChildProperty<TOwnerVM, TChildVM, TSourceObject>(),
            config => {
               config.Enable(PropertyBehaviorKeys.ValueAccessor, valueAccessor);

               if (sourceAccessor != null) {
                  config.Enable(PropertyBehaviorKeys.SourceAccessor, sourceAccessor);
               }
            }
         );
      }

      public IVMPropertyDescriptor<TChildVM> CreateViewModelProperty<TChildVM, TChildSource>(
         IValueAccessorBehavior<TChildVM> valueAccessor,
         IValueAccessorBehavior<TChildSource> sourceAccessor = null
      ) where TChildVM : IViewModel, IHasSourceObject<TChildSource> {
         return CreateCustomProperty<TChildVM>(
            BehaviorChainTemplateKeys.ViewModelProperty,
            FactoryConfigurations.ForChildProperty<TOwnerVM, TChildVM, TSourceObject, TChildSource>(),
            config => {
               config.Enable(PropertyBehaviorKeys.ValueAccessor, valueAccessor);

               if (sourceAccessor != null) {
                  config.Enable(PropertyBehaviorKeys.SourceAccessor, sourceAccessor);
               }
            }
         );
      }


      public IVMPropertyDescriptor<ICommand> CreateCommandProperty(
         IValueAccessorBehavior<TSourceObject> sourceObjectAccessor,
         IBehavior commandExecutor
      ) {
         return CreateCustomProperty<ICommand>(
            BehaviorChainTemplateKeys.CommandProperty,
            FactoryConfigurations.ForSimpleProperty<TOwnerVM, ICommand, TSourceObject>(false),
            config => {
               config.Enable(PropertyBehaviorKeys.SourceAccessor, sourceObjectAccessor);
               config.Enable(CommandPropertyBehaviorKeys.CommandExecutor, commandExecutor);
            }
         );
      }

      public IVMPropertyDescriptor<TValue> CreateCustomProperty<TValue>(
         BehaviorChainTemplateKey templateKey,
         IBehaviorFactoryConfiguration factoryConfiguration,
         Action<BehaviorChainConfiguration> chainConfigurationAction
      ) {
         var config = BehaviorChainConfiguration.GetConfiguration(templateKey, factoryConfiguration);
         chainConfigurationAction(config);
         return CreateAndRegisterProperty<TValue>(config);
      }



      public IVMPropertyDescriptor<T> CreateProperty<T>(IValueAccessorBehavior<T> sourceValueAccessor, bool supportsManualUpdate, bool includeRefreshBehavior) {
         BehaviorChainConfiguration config = GetPropertyConfiguration<T>(BehaviorChainTemplateKeys.Property);
         config.Enable(PropertyBehaviorKeys.SourceAccessor, sourceValueAccessor);

         //if (supportsManualUpdate) {
         //   config.Enable(PropertyBehaviorKeys.ManualUpdateBehavior);
         //}

         if (includeRefreshBehavior) {
            config.Enable(PropertyBehaviorKeys.RefreshBehavior, new RefreshBehavior.SimpleProperty<T>());
         }

         return CreateAndRegisterProperty<T>(config);
      }

      public IVMPropertyDescriptor<TChildVM> CreateViewModelProperty<TChildVM>(
         IValueAccessorBehavior<TChildVM> viewModelAccessor,
         IValueFactoryBehavior<TChildVM> viewModelFactory = null,
         IBehavior sourceAccessor = null,
         IBehavior sourceSetter = null,
         IBehavior manualUpdateBehavior = null,
         IBehavior refreshBehavior = null,
         bool needsViewModelFactory = false,
         bool cachesValue = false
      ) where TChildVM : IViewModel {
         BehaviorChainConfiguration config = GetBasicViewModelPropertyConfiguration<TChildVM>();

         config.Enable(PropertyBehaviorKeys.ViewModelAccessor, viewModelAccessor);

         //if (cachesValue) {
         //   config.Enable(PropertyBehaviorKeys.ValueCache);
         //}

         if (needsViewModelFactory) {
            config.Enable(PropertyBehaviorKeys.ViewModelFactory);
         }

         if (sourceAccessor != null) {
            config.Enable(PropertyBehaviorKeys.SourceAccessor, sourceAccessor);
         }

         //if (sourceSetter != null) {
         //   config.Enable(PropertyBehaviorKeys.ViewModelSourceSetter, sourceSetter);
         //}

         //if (manualUpdateBehavior != null) {
         //   config.Enable(PropertyBehaviorKeys.ManualUpdateBehavior, manualUpdateBehavior);
         //}

         if (refreshBehavior != null) {
            config.Enable(PropertyBehaviorKeys.RefreshBehavior, refreshBehavior);
         }

         return CreateAndRegisterProperty<TChildVM>(config);
      }

      //public VMProperty<TChildVM> CreateViewModelProperty<TChildVM>(
      //   IValueAccessorBehavior<TChildVM> viewModelAccessor,
      //   bool withViewModelFactory = false
      //) where TChildVM : IViewModel {
      //   BehaviorChainConfiguration config = GetBasicViewModelPropertyConfiguration<TChildVM>();

      //   config.Enable(BehaviorKeys.ValueCache);
      //   config.Enable(BehaviorKeys.ViewModelAccessor, viewModelAccessor);

      //   if (withViewModelFactory) {
      //      config.Enable(BehaviorKeys.ViewModelFactory);
      //   }

      //   return CreateProperty<TChildVM>(config);
      //}

      private BehaviorChainConfiguration GetBasicViewModelPropertyConfiguration<TChildVM>()
         where TChildVM : IViewModel {

         return BehaviorChainConfiguration.GetConfiguration(
            BehaviorChainTemplateKeys.ViewModelProperty,
            FactoryConfigurations.ForChildProperty<TOwnerVM, TChildVM, TSourceObject>()
         );
      }

      internal IVMPropertyDescriptor<IVMCollection<TItemVM>> CreateCollectionProperty<TItemVM>(
         BehaviorChainConfiguration collectionConfiguration,
         bool isPopulatable,
         IRefreshBehavior refreshBehavior = null
      ) where TItemVM : IViewModel {
         var config = GetPropertyConfiguration<IVMCollection<TItemVM>>(BehaviorChainTemplateKeys.CollectionProperty);

         if (isPopulatable) {
            //config.Enable(BehaviorKeys.CollectionInstanceCache);
            //config.Enable(PropertyBehaviorKeys.ManualUpdateBehavior, new ManualUpdateCollectionPropertyBehavior<TItemVM>());
            //config.Enable(PropertyBehaviorKeys.CollectionPopulator, new CollectionPopulatorBehavior<TItemVM>());
         }


         if (refreshBehavior != null) {
            config.Enable(PropertyBehaviorKeys.RefreshBehavior, refreshBehavior);
         }

         //config.Enable(
         //   PropertyBehaviorKeys.CollectionFactory,
         //   //new CollectionFactoryBehavior<TItemVM>(collectionConfiguration)
         //);

         return CreateAndRegisterProperty<IVMCollection<TItemVM>>(config);
      }

      internal BehaviorChainConfiguration GetCollectionConfiguration<TItemVM>(
         VMDescriptorBase itemDescriptor
      ) where TItemVM : IViewModel {
         var config = BehaviorChainConfiguration.GetConfiguration(
            BehaviorChainTemplateKeys.DefaultCollectionBehaviors,
            FactoryConfigurations.ForChildProperty<TOwnerVM, TItemVM, TSourceObject>()
         );

         config.Enable(
            CollectionBehaviorKeys.DescriptorSetter,
            new ItemDescriptorProviderBehavior(itemDescriptor)
         );

         return config;
      }

      internal BehaviorChainConfiguration GetPropertyConfiguration<TValue>(BehaviorChainTemplateKey key) {
         return BehaviorChainConfiguration.GetConfiguration(
            key,
            FactoryConfigurations.ForSimpleProperty<TOwnerVM, TValue, TSourceObject>(true) // TODO??
         );
      }

      internal IVMPropertyDescriptor<TValue> CreateAndRegisterProperty<TValue>(BehaviorChainConfiguration config) {
         var property = new VMPropertyDescriptor<TValue>();

         Configuration
           .PropertyConfigurations
           .RegisterProperty(property, config);

         return property;
      }
   }
}
