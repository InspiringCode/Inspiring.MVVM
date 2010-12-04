namespace Inspiring.Mvvm.ViewModels.Core.Builder.Properties {

   internal abstract class VMPropertyFactoryBase<TVM> :
      ConfigurationProvider
      where TVM : IViewModel {

      public VMPropertyFactoryBase(VMDescriptorConfiguration configuration)
         : base(configuration) {
      }

      protected VMProperty<T> CreateProperty<T>(IValueAccessorBehavior<T> sourceValueAccessor) {
         var behaviorTemplate = BehaviorChainTemplateRegistry.GetTemplate(BehaviorChainTemplateKeys.Property);

         var factoryInvoker = PropertyBehaviorFactory.CreateInvoker<TVM, T>();
         var behaviorConfiguration = behaviorTemplate.CreateConfiguration(factoryInvoker);
         behaviorConfiguration.Enable(BehaviorKeys.SourceValueAccessor, sourceValueAccessor);

         var property = new VMProperty<T>();

         Configuration
            .PropertyConfigurations
            .RegisterProperty(property, behaviorConfiguration);

         return property;
      }
   }
}
