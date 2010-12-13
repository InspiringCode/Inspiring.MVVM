namespace Inspiring.Mvvm.ViewModels.Core.Builder.Properties {
   using System;
   using System.Diagnostics.Contracts;
   using Inspiring.Mvvm.ViewModels.Fluent;

   internal sealed class VMPropertyFactoryWithSource<TVM, TSourceValue> :
      VMPropertyFactoryBase<TVM>,
      IVMPropertyFactoryWithSource<TVM, TSourceValue>
      where TVM : IViewModel {

      private readonly IValueAccessorBehavior<TSourceValue> _sourceValueAccessor;

      public VMPropertyFactoryWithSource(
         VMDescriptorConfiguration configuration,
         IValueAccessorBehavior<TSourceValue> sourceValueAccessor
      )
         : base(configuration) {
         Contract.Requires(configuration != null);
         Contract.Requires(sourceValueAccessor != null);

         _sourceValueAccessor = sourceValueAccessor;
      }

      public VMProperty<TSourceValue> Property() {
         return CreateProperty(_sourceValueAccessor);
      }

      public VMProperty<TChildVM> VM<TChildVM>() where TChildVM : IViewModel, ICanInitializeFrom<TSourceValue> {
         var template = BehaviorChainTemplateRegistry.GetTemplate(BehaviorChainTemplateKeys.ViewModelProperty);
         var invoker = PropertyBehaviorFactory.CreateInvoker<TVM, TChildVM>();
         var config = template.CreateConfiguration(invoker);

         config.Enable(BehaviorKeys.ViewModelPropertyInitializer, new ViewModelPropertyInitializerBehavior<TChildVM, TSourceValue>());
         config.Enable(BehaviorKeys.ViewModelFactory, new ViewModelFactoryBehavior<TChildVM>());
         config.Enable(BehaviorKeys.SourceValueAccessor, _sourceValueAccessor);

         VMProperty<TChildVM> property = new VMProperty<TChildVM>();

         Configuration
           .PropertyConfigurations
           .RegisterProperty(property, config);

         return property;
      }


      public IValueAccessorBehavior<TSourceValue> GetSourceAccessor() {
         throw new NotImplementedException();
      }
   }
}
