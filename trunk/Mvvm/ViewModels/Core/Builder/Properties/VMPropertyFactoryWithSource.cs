namespace Inspiring.Mvvm.ViewModels.Core.Builder.Properties {
   using System;
   using System.Diagnostics.Contracts;
   using Inspiring.Mvvm.ViewModels.Fluent;

   internal sealed class VMPropertyFactoryWithSource<TVM, TSourceValue> :
      IVMPropertyFactoryWithSource<TVM, TSourceValue>
      where TVM : IViewModel {

      private readonly VMDescriptorConfiguration _configuration;
      private readonly IValueAccessorBehavior<TSourceValue> _sourceValueAccessor;

      public VMPropertyFactoryWithSource(
         VMDescriptorConfiguration configuration,
         IValueAccessorBehavior<TSourceValue> sourceValueAccessor
      ) {
         Contract.Requires(configuration != null);
         Contract.Requires(sourceValueAccessor != null);

         _configuration = configuration;
         _sourceValueAccessor = sourceValueAccessor;
      }

      public VMProperty<TSourceValue> Property() {
         var behaviorTemplate = BehaviorChainTemplateRegistry.GetTemplate(BehaviorChainTemplateKeys.Property);

         var behaviorConfiguration = behaviorTemplate.CreateConfiguration<TSourceValue>();
         behaviorConfiguration.Enable(BehaviorKeys.SourceValueAccessor, _sourceValueAccessor);

         var property = new VMProperty<TSourceValue>();
         _configuration.PropertyConfigurations.RegisterProperty(property, behaviorConfiguration);

         return property;
      }

      public VMProperty<TChildVM> VM<TChildVM>() where TChildVM : IViewModel, ICanInitializeFrom<TSourceValue> {
         throw new NotImplementedException();
      }


      public IValueAccessorBehavior<TSourceValue> GetSourceAccessor() {
         throw new NotImplementedException();
      }

      public VMDescriptorConfiguration GetConfiguration() {
         throw new NotImplementedException();
      }
   }
}
