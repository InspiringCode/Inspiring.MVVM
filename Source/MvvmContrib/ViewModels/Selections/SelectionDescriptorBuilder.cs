namespace Inspiring.Mvvm.ViewModels {
   using System;
   using Inspiring.Mvvm.ViewModels.Core;

   internal abstract class SelectionDescriptorBuilder<TDescriptor, TVM>
      where TDescriptor : VMDescriptor, new()
      where TVM : IViewModel {

      private IVMDescriptorBuilderWithProperties<TDescriptor, TVM> _builder;

      public SelectionDescriptorBuilder() {
         _builder = VMDescriptorBuilder
            .OfType<TDescriptor>()
            .For<TVM>()
            .WithProperties((d, b) => { });
      }

      public void WithProperties(
         Action<TDescriptor, IVMPropertyBuilderProvider<TVM>> propertyConfigurator
      ) {
         _builder = _builder.WithProperties(propertyConfigurator);
      }

      public void WithBehaviors(
         Action<IVMBehaviorBuilder<TVM, TDescriptor>> behaviorConfigurator
      ) {
         _builder = _builder.WithBehaviors(behaviorConfigurator);
      }

      public void WithViewModelBehaviors(
         Action<ViewModelBehaviorBuilder<TVM, TDescriptor>> behaviorConfigurator
      ) {
         _builder = _builder.WithViewModelBehaviors(behaviorConfigurator);
      }

      public void WithDependencies(
         Action<IVMDependencyBuilder<TVM, TDescriptor>> dependencyConfigurator
      ) {
         _builder = _builder.WithDependencies(dependencyConfigurator);
      }

      public void WithValidators(
         Action<RootValidatorBuilder<TVM, TVM, TDescriptor>> validatorConfigurator
      ) {
         _builder = _builder.WithValidators(validatorConfigurator);
      }

      public TDescriptor Build() {
         return _builder.Build();
      }
   }
}
