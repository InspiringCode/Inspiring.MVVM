namespace Inspiring.Mvvm.ViewModels.Core.Builder.Properties {
   using System;
   using System.Collections.Generic;
   using System.Diagnostics.Contracts;
   using Inspiring.Mvvm.Common;
   using Inspiring.Mvvm.ViewModels.Fluent;

   internal sealed class VMCollectionPropertyFactory<TVM, TSource> :
      ConfigurationProvider,
      IVMCollectionPropertyFactory<TVM, TSource>
      where TVM : IViewModel {

      private PropertyPath<TVM, TSource> _sourceObjectPath;

      public VMCollectionPropertyFactory(
         PropertyPath<TVM, TSource> sourceObjectPath,
         VMDescriptorConfiguration configuration
      )
         : base(configuration) {
         Contract.Requires(configuration != null);

         _sourceObjectPath = sourceObjectPath;
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

      /// <inheritdoc />
      public IVMCollectionPropertyFactoryWithSource<TVM, TItemSource> Wraps<TItemSource>(
         Func<TSource, IEnumerable<TItemSource>> sourceCollectionSelector
      ) {
         var sourceCollectionAccessor = new CalculatedPropertyAccessor<TVM, TSource, IEnumerable<TItemSource>>(
            _sourceObjectPath,
            sourceCollectionSelector
         );

         return new VMCollectionPropertyFactoryWithSource<TVM, TItemSource>(
            Configuration,
            sourceCollectionAccessor
         );
      }

      public VMProperty<IVMCollection<TItemVM>> InitializedBy<TItemVM>(
         Func<TSource, IEnumerable<TItemVM>> itemSelector
      ) where TItemVM : IViewModel {
         throw new NotImplementedException();
      }
   }
}
