namespace Inspiring.Mvvm.ViewModels.Core {
   using Inspiring.Mvvm.ViewModels.Core.Builder;

   // TODO: Document and clean up the whole builder stuff a bit.
   internal abstract class VMPropertyBuilderBase<TVM> : ConfigurationProvider where TVM : IViewModel {
      public VMPropertyBuilderBase(VMDescriptorConfiguration configuration)
         : base(configuration) {
      }

      protected VMProperty<T> CreateProperty<T>(IValueAccessorBehavior<T> sourceValueAccessor) {
         BehaviorChainConfiguration config = GetPropertyConfiguration<T>(BehaviorChainTemplateKeys.Property);
         config.Enable(BehaviorKeys.SourceValueAccessor, sourceValueAccessor);
         return CreateProperty<T>(config);
      }

      protected VMProperty<TChildVM> CreateViewModelProperty<TChildVM>() where TChildVM : IViewModel {
         BehaviorChainConfiguration config = GetPropertyConfiguration<TChildVM>(BehaviorChainTemplateKeys.ViewModelProperty);

         config.Enable(BehaviorKeys.ParentSetter, new ParentSetterBehavior<TChildVM>());
         config.Enable(BehaviorKeys.SourceValueAccessor, new InstancePropertyBehavior<TChildVM>());

         return CreateProperty<TChildVM>(config);
      }

      protected VMProperty<TChildVM> CreateViewModelProperty<TChildVM, TSourceValue>(
         IValueAccessorBehavior<TSourceValue> sourceValueAccessor
      ) where TChildVM : IViewModel, ICanInitializeFrom<TSourceValue> {
         BehaviorChainConfiguration config = GetPropertyConfiguration<TChildVM>(BehaviorChainTemplateKeys.ViewModelProperty);

         config.Enable(BehaviorKeys.ValueCache);
         config.Enable(BehaviorKeys.ParentSetter, new ParentSetterBehavior<TChildVM>());
         config.Enable(BehaviorKeys.ViewModelPropertyInitializer, new ViewModelPropertyInitializerBehavior<TChildVM, TSourceValue>());
         config.Enable(BehaviorKeys.ViewModelFactory, new ViewModelFactoryBehavior<TChildVM>());
         config.Enable(BehaviorKeys.SourceValueAccessor, sourceValueAccessor);

         return CreateProperty<TChildVM>(config);
      }

      protected VMProperty<TChildVM> CreateViewModelProperty<TChildVM, TSourceObject>(
         IValueAccessorBehavior<TSourceObject> sourceValueAccessor,
         IViewModelFactoryBehavior<TChildVM> customFactory
      ) where TChildVM : IViewModel {
         BehaviorChainConfiguration config = GetPropertyConfiguration<TChildVM>(BehaviorChainTemplateKeys.ViewModelProperty);

         config.Enable(BehaviorKeys.ValueCache);
         config.Enable(BehaviorKeys.ParentSetter, new ParentSetterBehavior<TChildVM>());
         config.Enable(BehaviorKeys.ViewModelFactory, customFactory);
         config.Enable(BehaviorKeys.SourceValueAccessor, sourceValueAccessor);

         return CreateProperty<TChildVM>(config);
      }

      protected VMProperty<IVMCollection<TItemVM>> CreateCollectionProperty<TItemVM>(
         BehaviorChainConfiguration collectionConfiguration,
         bool isPopulatable
      ) where TItemVM : IViewModel {
         var config = GetPropertyConfiguration<IVMCollection<TItemVM>>(BehaviorChainTemplateKeys.CollectionProperty);

         if (isPopulatable) {
            config.Enable(BehaviorKeys.CollectionInstanceCache);
            config.Enable(BehaviorKeys.CollectionPopulator, new CollectionPopulatorBehavior<TItemVM>());
         }

         config.Enable(
            BehaviorKeys.CollectionFactory,
            new CollectionFactoryBehavior<TItemVM>(collectionConfiguration)
         );

         return CreateProperty<IVMCollection<TItemVM>>(config);
      }

      protected BehaviorChainConfiguration GetCollectionConfiguration<TItemVM>(
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

      protected BehaviorChainConfiguration GetPropertyConfiguration<TValue>(BehaviorChainTemplateKey key) {
         var template = BehaviorChainTemplateRegistry.GetTemplate(key);
         var invoker = PropertyBehaviorFactory.CreateInvoker<TVM, TValue>();
         return template.CreateConfiguration(invoker);
      }

      protected VMProperty<TValue> CreateProperty<TValue>(BehaviorChainConfiguration config) {
         var property = new VMProperty<TValue>();

         Configuration
           .PropertyConfigurations
           .RegisterProperty(property, config);

         return property;
      }
   }
}
