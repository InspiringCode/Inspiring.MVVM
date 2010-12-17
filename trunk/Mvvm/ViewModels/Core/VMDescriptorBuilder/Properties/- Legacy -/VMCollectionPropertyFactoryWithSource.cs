namespace Inspiring.Mvvm.ViewModels.Core.Builder.Properties {
   using System.Collections.Generic;
   using System.Diagnostics.Contracts;
   using Inspiring.Mvvm.ViewModels.Fluent;

   internal sealed class VMCollectionPropertyFactoryWithSource<TVM, TItemSource> :
      ConfigurationProvider,
      IVMCollectionPropertyFactoryWithSource<TItemSource>
      where TVM : IViewModel {

      private IValueAccessorBehavior<IEnumerable<TItemSource>> _sourceCollectionAccessor;

      public VMCollectionPropertyFactoryWithSource(
         VMDescriptorConfiguration configuration,
         IValueAccessorBehavior<IEnumerable<TItemSource>> sourceCollectionAccessor
      )
         : base(configuration) {
         Contract.Requires(configuration != null);
         Contract.Requires(sourceCollectionAccessor != null);

         _sourceCollectionAccessor = sourceCollectionAccessor;
      }

      public VMProperty<IVMCollection<TItemVM>> Of<TItemVM>(VMDescriptorBase itemDescriptor)
         where TItemVM : IViewModel, ICanInitializeFrom<TItemSource> {

         var collectionBehaviorsTemplate = BehaviorChainTemplateRegistry.GetTemplate(
            BehaviorChainTemplateKeys.DefaultCollectionBehaviors
         );
         var collectionFactoryInvoker = CollectionBehaviorFactory.CreateInvoker<TVM, TItemVM>();
         var collectionConfiguration = collectionBehaviorsTemplate.CreateConfiguration(collectionFactoryInvoker);

         collectionConfiguration.Enable(CollectionBehaviorKeys.SourceCollectionAccessor, _sourceCollectionAccessor);
         collectionConfiguration.Enable(CollectionBehaviorKeys.DescriptorSetter, new DescriptorSetterCollectionBehavior<TItemVM>(itemDescriptor));
         collectionConfiguration.Enable(CollectionBehaviorKeys.Populator, new PopulatorCollectionBehavior<TItemVM, TItemSource>());
         collectionConfiguration.Enable(CollectionBehaviorKeys.ViewModelFactory);

         var behaviorTemplate = BehaviorChainTemplateRegistry.GetTemplate(BehaviorChainTemplateKeys.CollectionProperty);
         var factoryInvoker = PropertyBehaviorFactory.CreateInvoker<TVM, IVMCollection<TItemVM>>();
         var behaviorConfiguration = behaviorTemplate.CreateConfiguration(factoryInvoker);

         var fac = new CollectionFactoryBehavior<TItemVM>(collectionConfiguration);

         behaviorConfiguration.Enable(BehaviorKeys.CollectionFactory, fac);
         behaviorConfiguration.Enable(BehaviorKeys.CollectionPopulator, new CollectionPopulatorBehavior<TItemVM>());
         behaviorConfiguration.Enable(BehaviorKeys.CollectionInstanceCache);

         var property = new VMProperty<IVMCollection<TItemVM>>();

         Configuration
            .PropertyConfigurations
            .RegisterProperty(property, behaviorConfiguration);

         return property;
      }
   }
}
