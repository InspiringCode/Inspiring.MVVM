namespace Inspiring.Mvvm.ViewModels.Core {

   // TODO: Document and clean up the whole builder stuff a bit.
   public sealed class VMPropertyFactory<TVM> : ConfigurationProvider where TVM : IViewModel {
      public VMPropertyFactory(VMDescriptorConfiguration configuration)
         : base(configuration) {
      }

      public VMProperty<T> CreateProperty<T>(IValueAccessorBehavior<T> sourceValueAccessor, bool supportsManualUpdate) {
         BehaviorChainConfiguration config = GetPropertyConfiguration<T>(BehaviorChainTemplateKeys.Property);
         config.Enable(BehaviorKeys.SourceAccessor, sourceValueAccessor);

         if (supportsManualUpdate) {
            config.Enable(BehaviorKeys.ManualUpdateBehavior);
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

      public VMProperty<TChildVM> CreateViewModelProperty<TChildVM>(IValueAccessorBehavior<TChildVM> viewModelAccessor, IBehavior sourceAccessor = null, bool needsViewModelFactory = false, bool cachesValue = false) where TChildVM : IViewModel {
         BehaviorChainConfiguration config = GetBasicViewModelPropertyConfiguration<TChildVM>();

         config.Enable(BehaviorKeys.ViewModelAccessor, viewModelAccessor);

         if (cachesValue) {
            config.Enable(BehaviorKeys.ValueCache);
         }

         if (needsViewModelFactory) {
            config.Enable(BehaviorKeys.ViewModelFactory);
         }

         if (sourceAccessor != null) {
            config.Enable(BehaviorKeys.SourceAccessor, sourceAccessor);
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

      public VMProperty<IVMCollection<TItemVM>> CreateCollectionProperty<TItemVM>(
         BehaviorChainConfiguration collectionConfiguration,
         bool isPopulatable
      ) where TItemVM : IViewModel {
         var config = GetPropertyConfiguration<IVMCollection<TItemVM>>(BehaviorChainTemplateKeys.CollectionProperty);

         if (isPopulatable) {
            //config.Enable(BehaviorKeys.CollectionInstanceCache);
            config.Enable(BehaviorKeys.ManualUpdateBehavior, new ManualUpdateCollectionPropertyBehavior<TItemVM>());
            config.Enable(BehaviorKeys.CollectionPopulator, new CollectionPopulatorBehavior<TItemVM>());
         }

         config.Enable(
            BehaviorKeys.CollectionFactory,
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
            new DescriptorSetterCollectionBehavior<TItemVM>(itemDescriptor)
         );

         return config;
      }

      internal BehaviorChainConfiguration GetPropertyConfiguration<TValue>(BehaviorChainTemplateKey key) {
         var template = BehaviorChainTemplateRegistry.GetTemplate(key);
         var invoker = PropertyBehaviorFactory.CreateInvoker<TVM, TValue>();
         return template.CreateConfiguration(invoker);
      }

      internal VMProperty<TValue> CreateProperty<TValue>(BehaviorChainConfiguration config) {
         var property = new VMProperty<TValue>();

         Configuration
           .PropertyConfigurations
           .RegisterProperty(property, config);

         return property;
      }
   }
}
