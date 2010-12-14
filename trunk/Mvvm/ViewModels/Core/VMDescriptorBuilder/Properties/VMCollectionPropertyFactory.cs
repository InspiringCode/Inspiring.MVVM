namespace Inspiring.Mvvm.ViewModels.Core.Builder.Properties {
   using System.Diagnostics.Contracts;
   using Inspiring.Mvvm.ViewModels.Fluent;

   internal sealed class VMCollectionPropertyFactory<TVM> :
      ConfigurationProvider,
      IVMCollectionPropertyFactory<TVM>
      where TVM : IViewModel {

      public VMCollectionPropertyFactory(
         VMDescriptorConfiguration configuration
      )
         : base(configuration) {
         Contract.Requires(configuration != null);
      }

      public VMProperty<IVMCollection<TItemVM>> Of<TItemVM>(VMDescriptorBase itemDescriptor) where TItemVM : IViewModel {
         var sourceCollectionAccessor = new InstancePropertyBehavior<IVMCollection<TItemVM>>();

         var collectionBehaviorsTemplate = BehaviorChainTemplateRegistry.GetTemplate(
            BehaviorChainTemplateKeys.DefaultCollectionBehaviors
         );
         var collectionFactoryInvoker = CollectionBehaviorFactory.CreateInvoker<TVM, TItemVM>();
         var collectionConfiguration = collectionBehaviorsTemplate.CreateConfiguration(collectionFactoryInvoker);

         collectionConfiguration.Enable(CollectionBehaviorKeys.SourceCollectionAccessor, sourceCollectionAccessor);
         collectionConfiguration.Enable(CollectionBehaviorKeys.DescriptorSetter, new DescriptorSetterCollectionBehavior<TItemVM>(itemDescriptor));
         
         var behaviorTemplate = BehaviorChainTemplateRegistry.GetTemplate(BehaviorChainTemplateKeys.CollectionProperty);
         var factoryInvoker = PropertyBehaviorFactory.CreateInvoker<TVM, IVMCollection<TItemVM>>();
         var behaviorConfiguration = behaviorTemplate.CreateConfiguration(factoryInvoker);

         var fac = new CollectionFactoryBehavior<TItemVM>(collectionConfiguration);

         behaviorConfiguration.Enable(BehaviorKeys.CollectionFactory, fac);
         //behaviorConfiguration.Enable(BehaviorKeys.CollectionPopulator, new CollectionPopulatorBehavior<TItemVM>());
         //behaviorConfiguration.Enable(BehaviorKeys.CollectionInstanceCache);

         var property = new VMProperty<IVMCollection<TItemVM>>();

         Configuration
            .PropertyConfigurations
            .RegisterProperty(property, behaviorConfiguration);

         return property;
      }
   }
}
