using Inspiring.Mvvm.ViewModels.Core.Behaviors;
namespace Inspiring.Mvvm.ViewModels.Core {

   // TODO: Document and clean up the whole builder stuff a bit.
   public sealed class VMPropertyFactory<TVM> : ConfigurationProvider where TVM : IViewModel {
      public VMPropertyFactory(VMDescriptorConfiguration configuration)
         : base(configuration) {
      }

      public IVMPropertyDescriptor<T> CreateProperty<T>(IValueAccessorBehavior<T> sourceValueAccessor, bool supportsManualUpdate, bool includeRefreshBehavior) {
         BehaviorChainConfiguration config = GetPropertyConfiguration<T>(BehaviorChainTemplateKeys.Property);
         config.Enable(PropertyBehaviorKeys.SourceAccessor, sourceValueAccessor);

         if (supportsManualUpdate) {
            config.Enable(PropertyBehaviorKeys.ManualUpdateBehavior);
         }

         if (includeRefreshBehavior) {
            config.Enable(PropertyBehaviorKeys.RefreshBehavior, new RefreshBehavior.SimpleProperty<T>());
         }

         return CreateProperty<T>(config);
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

         if (manualUpdateBehavior != null) {
            config.Enable(PropertyBehaviorKeys.ManualUpdateBehavior, manualUpdateBehavior);
         }

         if (refreshBehavior != null) {
            config.Enable(PropertyBehaviorKeys.RefreshBehavior, refreshBehavior);
         }

         return CreateProperty<TChildVM>(config);
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

         var template = BehaviorChainTemplateRegistry.GetTemplate(BehaviorChainTemplateKeys.ViewModelProperty);
         var factoryConfig = new ViewModelPropertyBehaviorFactoryConfiguration<TVM, TChildVM>();
         var config = template.CreateConfiguration(factoryConfig);

         return config;
      }

      internal IVMPropertyDescriptor<IVMCollection<TItemVM>> CreateCollectionProperty<TItemVM>(
         BehaviorChainConfiguration collectionConfiguration,
         bool isPopulatable,
         IRefreshBehavior refreshBehavior = null
      ) where TItemVM : IViewModel {
         var config = GetPropertyConfiguration<IVMCollection<TItemVM>>(BehaviorChainTemplateKeys.CollectionProperty);

         if (isPopulatable) {
            //config.Enable(BehaviorKeys.CollectionInstanceCache);
            config.Enable(PropertyBehaviorKeys.ManualUpdateBehavior, new ManualUpdateCollectionPropertyBehavior<TItemVM>());
            config.Enable(PropertyBehaviorKeys.CollectionPopulator, new CollectionPopulatorBehavior<TItemVM>());
         }


         if (refreshBehavior != null) {
            config.Enable(PropertyBehaviorKeys.RefreshBehavior, refreshBehavior);
         }

         config.Enable(
            PropertyBehaviorKeys.CollectionFactory,
            new CollectionFactoryBehavior<TItemVM>(collectionConfiguration)
         );

         return CreateProperty<IVMCollection<TItemVM>>(config);
      }

      internal BehaviorChainConfiguration GetCollectionConfiguration<TItemVM>(
         VMDescriptorBase itemDescriptor
      ) where TItemVM : IViewModel {
         var template = BehaviorChainTemplateRegistry.GetTemplate(BehaviorChainTemplateKeys.DefaultCollectionBehaviors);
         var factoryConfig = new CollectionPropertyBehaviorFactoryConfiguration<TVM, TItemVM>();
         var config = template.CreateConfiguration(factoryConfig);

         config.Enable(
            CollectionBehaviorKeys.DescriptorSetter,
            new ItemDescriptorCollectionBehavior<TItemVM>(itemDescriptor)
         );

         return config;
      }

      internal BehaviorChainConfiguration GetPropertyConfiguration<TValue>(BehaviorChainTemplateKey key) {
         var template = BehaviorChainTemplateRegistry.GetTemplate(key);
         var factoryConfiguration = new PropertyBehaviorFactoryConfiguration<TVM, TValue>();
         return template.CreateConfiguration(factoryConfiguration);
      }

      internal IVMPropertyDescriptor<TValue> CreateProperty<TValue>(BehaviorChainConfiguration config) {
         var property = new VMPropertyDescriptor<TValue>();

         Configuration
           .PropertyConfigurations
           .RegisterProperty(property, config);

         return property;
      }
   }
}
