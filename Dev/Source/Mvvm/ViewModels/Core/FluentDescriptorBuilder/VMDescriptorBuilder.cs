namespace Inspiring.Mvvm.ViewModels.Core.VMDescriptorBuilder {
   using System;
   using System.Diagnostics.Contracts;
   using Inspiring.Mvvm.ViewModels.Core.Behaviors;
   using Inspiring.Mvvm.ViewModels.Core.VMDescriptorBuilder.Behaviors;

   /// <summary>
   ///   Fluent interface. See <see cref="VMDescriptorBuilder"/>.
   /// </summary>
   public sealed class DerivedVMDescriptorBuilder<TBaseDescriptor>
      where TBaseDescriptor : VMDescriptor {

      private readonly IVMDescriptorBuilder _baseBuilder;

      internal DerivedVMDescriptorBuilder(IVMDescriptorBuilder baseBuilder) {
         _baseBuilder = baseBuilder;
      }

      /// <summary>
      ///   Selects the descriptor type that should be created.
      /// </summary>
      public VMDescriptorBuilder<TDescriptor> OfType<TDescriptor>()
         where TDescriptor : TBaseDescriptor, new() {
         return new VMDescriptorBuilder<TDescriptor>(_baseBuilder);
      }
   }

   /// <summary>
   ///   Fluent interface. See <see cref="VMDescriptorBuilder"/>.
   /// </summary>
   public sealed class VMDescriptorBuilder<TDescriptor>
      where TDescriptor : VMDescriptor, new() {

      private readonly IVMDescriptorBuilder _baseBuilder;

      internal VMDescriptorBuilder(IVMDescriptorBuilder baseBuilder) {
         _baseBuilder = baseBuilder;
      }

      /// <summary>
      ///   <para>Specifies the <see cref="ViewModel"/> type for which this 
      ///      descriptor should be build.</para>
      ///   <para>A <see cref="VMDescriptor"/> instance is always build for one
      ///      certain VM class and all instances of a VM usually have the same
      ///      <see cref="VMDescriptor"/> object. For example: All instances of
      ///      the PersonVM CLASS are associated with the same PersonVMDescriptor
      ///      INSTANCE.</para>
      /// </summary>
      public IVMDescriptorBuilder<TDescriptor, TVM> For<TVM>()
         where TVM : IViewModel { // TODO: Better type checking? Sometimes we need in, sometimes we need out?
         return new VMDescriptorBuilder<TDescriptor, TVM>(_baseBuilder);
      }
   }

   /// <summary>
   ///   Fluent interface. See <see cref="VMDescriptorBuilder"/>.
   /// </summary>
   /// <remarks>
   ///   This is the class where the real building happens. The other builder
   ///   class merely collection generic arguments.
   /// </remarks>
   public sealed class VMDescriptorBuilder<TDescriptor, TVM> :
      IVMDescriptorBuilder,
      IVMDescriptorBuilder<TDescriptor, TVM>,
      IVMDescriptorBuilderWithProperties<TDescriptor, TVM>
      where TDescriptor : VMDescriptor, new()
      where TVM : IViewModel {

      private readonly IVMDescriptorBuilder _baseBuilder;
      private Action<TDescriptor, IVMPropertyBuilderProvider<TVM>> _propertyConfigurator;
      private Action<RootValidatorBuilder<TVM, TVM, TDescriptor>> _validatorConfigurator;
      private Action<IVMBehaviorBuilder<TVM, TDescriptor>> _behaviorConfigurator;
      private Action<ViewModelBehaviorBuilder<TVM, TDescriptor>> _viewModelBehaviorConfigurator;
      private Action<IVMDependencyConfigurator<TDescriptor>> _dependencyConfigurator;

      internal VMDescriptorBuilder(IVMDescriptorBuilder baseBuilder) {
         _baseBuilder = baseBuilder;
      }

      /// <inheritdoc />
      public IVMDescriptorBuilderWithProperties<TDescriptor, TVM> WithProperties(
         Action<TDescriptor, IVMPropertyBuilderProvider<TVM>> propertyConfigurator
      ) {
         Contract.Requires<ArgumentNullException>(propertyConfigurator != null);
         _propertyConfigurator = propertyConfigurator;
         return this;
      }

      /// <inheritdoc />
      public IVMDescriptorBuilderWithProperties<TDescriptor, TVM> WithPropertyDependencies(
         Action<IVMDependencyConfigurator<TDescriptor>> dependencyConfigurator
      ) {
         Contract.Requires<ArgumentNullException>(dependencyConfigurator != null);
         _dependencyConfigurator = dependencyConfigurator;
         return this;
      }

      /// <inheritdoc />
      public IVMDescriptorBuilderWithProperties<TDescriptor, TVM> WithValidators(
         Action<RootValidatorBuilder<TVM, TVM, TDescriptor>> validatorConfigurator
      ) {
         Contract.Requires<ArgumentNullException>(validatorConfigurator != null);
         _validatorConfigurator = validatorConfigurator;
         return this;
      }

      /// <inheritdoc />
      public IVMDescriptorBuilderWithProperties<TDescriptor, TVM> WithBehaviors(
         Action<IVMBehaviorBuilder<TVM, TDescriptor>> behaviorConfigurator
      ) {
         Contract.Requires<ArgumentNullException>(behaviorConfigurator != null);
         _behaviorConfigurator = behaviorConfigurator;
         return this;
      }

      /// <inheritdoc />
      public IVMDescriptorBuilderWithProperties<TDescriptor, TVM> WithViewModelBehaviors(
         Action<ViewModelBehaviorBuilder<TVM, TDescriptor>> behaviorConfigurator
      ) {
         Contract.Requires<ArgumentNullException>(behaviorConfigurator != null);
         _viewModelBehaviorConfigurator = behaviorConfigurator;
         return this;
      }

      /// <inheritdoc />
      public TDescriptor Build() {
         var descriptor = new TDescriptor();
         var configuration = new VMDescriptorConfiguration(GetDefaultViewModelConfiguration());

         ConfigureDescriptor(descriptor, configuration);

         descriptor.Builder = this;
         descriptor.InitializePropertyNames(); // TODO: This is propably too late!

         configuration.ApplyTo(descriptor);

         return descriptor;
      }

      /// <inheritdoc />
      void IVMDescriptorBuilder.ConfigureDescriptor(VMDescriptor descriptor, VMDescriptorConfiguration configuration) {
         ConfigureDescriptor((TDescriptor)descriptor, configuration);
      }

      private void ConfigureDescriptor(TDescriptor descriptor, VMDescriptorConfiguration configuration) {
         if (_baseBuilder != null) {
            _baseBuilder.ConfigureDescriptor(descriptor, configuration);
         }

         var propertyBuilderProvider = new VMPropertyBuilderProvider<TVM>(configuration);
         var viewModeBehaviorBulider = new ViewModelBehaviorBuilder<TVM, TDescriptor>(configuration, descriptor);
         var validatorBuilder = new RootValidatorBuilder<TVM, TVM, TDescriptor>(configuration, descriptor);
         var behaviorBuilder = new VMBehaviorBuilder<TVM, TDescriptor>(configuration, descriptor);

         _propertyConfigurator(descriptor, propertyBuilderProvider);

         if (_validatorConfigurator != null) {
            _validatorConfigurator(validatorBuilder);
            validatorBuilder.Execute();
         }

         if (_viewModelBehaviorConfigurator != null) {
            _viewModelBehaviorConfigurator(viewModeBehaviorBulider);
         }

         if (_behaviorConfigurator != null) {
            _behaviorConfigurator(behaviorBuilder);
         }
      }


      private static BehaviorChainConfiguration GetDefaultViewModelConfiguration() {
         return BehaviorChainConfiguration.GetConfiguration(
            DefaultBehaviorChainTemplateKeys.ViewModel,
            BehaviorFactoryConfigurations.ForViewModel<TVM>()
         );
      }
   }
}
