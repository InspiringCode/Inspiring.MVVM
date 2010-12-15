namespace Inspiring.Mvvm.ViewModels.Core.Builder.Properties {
   using System.Diagnostics.Contracts;
   using Inspiring.Mvvm.ViewModels.Fluent;

   internal sealed class LocalVMPropertyLocal<TVM> :
      VMPropertyFactoryBase<TVM>,
      ILocalVMPropertyFactory
      where TVM : IViewModel {

      public LocalVMPropertyLocal(
         VMDescriptorConfiguration configuration
      )
         : base(configuration) {
         Contract.Requires(configuration != null);
      }

      public VMProperty<T> Property<T>() {
         var sourceValueAccessor = new InstancePropertyBehavior<T>();
         return CreateProperty(sourceValueAccessor);
      }

      public VMProperty<TChildVM> VM<TChildVM>() where TChildVM : IViewModel {
         var template = BehaviorChainTemplateRegistry.GetTemplate(BehaviorChainTemplateKeys.ViewModelProperty);
         var invoker = PropertyBehaviorFactory.CreateInvoker<TVM, TChildVM>();
         var config = template.CreateConfiguration(invoker);

         var sourceValueAccessor = new InstancePropertyBehavior<TChildVM>();

         config.Enable(BehaviorKeys.ParentSetter, new ParentSetterBehavior<TChildVM>());
         config.Enable(BehaviorKeys.SourceValueAccessor, sourceValueAccessor);

         VMProperty<TChildVM> property = new VMProperty<TChildVM>();

         Configuration
           .PropertyConfigurations
           .RegisterProperty(property, config);

         return property;
      }
   }
}
