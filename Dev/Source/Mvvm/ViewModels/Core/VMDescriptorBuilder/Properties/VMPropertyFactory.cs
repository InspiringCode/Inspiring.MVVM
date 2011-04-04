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

      //public VMProperty<TChildVM> CreateViewModelProperty<TChildVM>(
      //   IValueAccessorBehavior<TChildVM> sourceValueAccessor
      //) where TChildVM : IViewModel {
      //   BehaviorChainConfiguration config = GetPropertyConfiguration<TChildVM>(BehaviorChainTemplateKeys.ViewModelProperty);

      //   config.Enable(BehaviorKeys.ParentSetter, new ParentSetterBehavior<TChildVM>(setParentOnGetValue: true, setParentOnSetValue: false));
      //   config.Enable(BehaviorKeys.SourceAccessor, sourceValueAccessor);

      //   return CreateProperty<TChildVM>(config);
      //}

      // TODO: Recheck all that ManualUpdate stuff.
      //[Obsolete]
      //public VMProperty<TChildVM> CreateViewModelProperty<TChildVM, TSourceValue>(
      //   IValueAccessorBehavior<TSourceValue> sourceValueAccessor
      //) where TChildVM : IViewModel, ICanInitializeFrom<TSourceValue> {
      //   BehaviorChainConfiguration config = GetPropertyConfiguration<TChildVM>(BehaviorChainTemplateKeys.ViewModelProperty);

      //   config.Enable(BehaviorKeys.ManualUpdateBehavior, new ManualUpdateViewModelPropertyBehavior<TChildVM, TSourceValue>());
      //   config.Enable(BehaviorKeys.ValueCache);
      //   config.Enable(BehaviorKeys.ParentSetter, new ParentSetterBehavior<TChildVM>(setParentOnGetValue: true, setParentOnSetValue: false));
      //   config.Enable(BehaviorKeys.ViewModelAccessor, new ViewModelFactoryAccessorBehavior<TChildVM>());
      //   config.Enable(BehaviorKeys.ViewModelFactory, new ViewModelFactoryBehavior<TChildVM>());
      //   config.Enable(BehaviorKeys.SourceAccessor, sourceValueAccessor);

      //   return CreateProperty<TChildVM>(config);
      //}

      //[Obsolete]
      //public VMProperty<TChildVM> CreateViewModelProperty<TChildVM, TSourceObject>(
      //   IValueAccessorBehavior<TSourceObject> sourceObjectAccessor,
      //   IViewModelFactoryBehavior<TChildVM> customFactory
      //) where TChildVM : IViewModel {
      //   BehaviorChainConfiguration config = GetPropertyConfiguration<TChildVM>(BehaviorChainTemplateKeys.ViewModelProperty);

      //   config.Enable(BehaviorKeys.ValueCache);
      //   config.Enable(BehaviorKeys.ParentSetter, new ParentSetterBehavior<TChildVM>(setParentOnGetValue: true, setParentOnSetValue: false));
      //   config.Enable(BehaviorKeys.ViewModelAccessor, new ViewModelFactoryAccessorBehavior<TChildVM>());
      //   config.Enable(BehaviorKeys.ViewModelFactory, customFactory);
      //   config.Enable(BehaviorKeys.SourceAccessor, sourceObjectAccessor);

      //   return CreateProperty<TChildVM>(config);
      //}

      //[Obsolete]
      //public VMProperty<TChildVM> CreateViewModelProperty<TChildVM, TSourceObject>(
      //   IValueAccessorBehavior<TSourceObject> sourceObjectAccessor,
      //   IValueAccessorBehavior<TChildVM> customAccessor
      //) where TChildVM : IViewModel {
      //   BehaviorChainConfiguration config = GetPropertyConfiguration<TChildVM>(BehaviorChainTemplateKeys.ViewModelProperty);

      //   config.Enable(BehaviorKeys.ValueCache);
      //   config.Enable(BehaviorKeys.ParentSetter, new ParentSetterBehavior<TChildVM>(setParentOnGetValue: true, setParentOnSetValue: false));
      //   config.Enable(BehaviorKeys.ViewModelAccessor, customAccessor);
      //   config.Enable(BehaviorKeys.ViewModelFactory, new ViewModelFactoryBehavior<TChildVM>());
      //   config.Enable(BehaviorKeys.SourceAccessor, sourceObjectAccessor);

      //   return CreateProperty<TChildVM>(config);
      //}

      //[Obsolete]
      //public VMProperty<TChildVM> CreateViewModelProperty<TChildVM, TSourceObject>(
      //   //IValueAccessorBehavior<TSourceObject> sourceObjectAccessor,
      //   IValueAccessorBehavior<TChildVM> viewModelAccessor
      //) where TChildVM : IViewModel {
      //   BehaviorChainConfiguration config = GetPropertyConfiguration<TChildVM>(BehaviorChainTemplateKeys.ViewModelProperty);

      //   // TODO: What is the right behavior here?????? Important!
      //   config.Enable(BehaviorKeys.ValueCache, new RefreshableValueCacheBehavior<TChildVM>());
      //   config.Enable(BehaviorKeys.ParentSetter, new ParentSetterBehavior<TChildVM>(setParentOnGetValue: true, setParentOnSetValue: false));
      //   config.Enable(BehaviorKeys.ViewModelAccessor, viewModelAccessor);
      //   //config.Enable(BehaviorKeys.ViewModelFactory, customFactory);
      //   //config.Enable(BehaviorKeys.SourceValueAccessor, sourceObjectAccessor);

      //   return CreateProperty<TChildVM>(config);
      //}

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
         var invoker = ViewModelPropertyBehaviorFactory.CreateInvoker<TVM, TChildVM>();
         var config = template.CreateConfiguration(invoker);

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
         var invoker = CollectionBehaviorFactory.CreateInvoker<TVM, TItemVM>();
         var config = template.CreateConfiguration(invoker);

         config.Enable(
            CollectionBehaviorKeys.DescriptorSetter,
            new ItemDescriptorCollectionBehavior<TItemVM>(itemDescriptor)
         );

         return config;
      }

      internal BehaviorChainConfiguration GetPropertyConfiguration<TValue>(BehaviorChainTemplateKey key) {
         var template = BehaviorChainTemplateRegistry.GetTemplate(key);
         var invoker = PropertyBehaviorFactory.CreateInvoker<TVM, TValue>();
         return template.CreateConfiguration(invoker);
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
