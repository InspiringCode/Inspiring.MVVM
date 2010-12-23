//namespace Inspiring.Mvvm.ViewModels {
//   using System;
//   using System.Diagnostics.Contracts;
//   using System.Linq.Expressions;
//   using Inspiring.Mvvm.Common;
//   using Inspiring.Mvvm.ViewModels.Core;
//   using Inspiring.Mvvm.ViewModels.Fluent;

//   /// <summary>
//   ///   Use this builder to create and initialize 'VMDescriptor' objects. The
//   ///   builder implements the "fluent interface" pattern.
//   /// </summary>
//   public static class VMDescriptorBuilder {
//      /// <summary>
//      ///   <para>Returns a builder expression that creates and configure a new 
//      ///      concrete <see cref="VMDescriptor"/> for the given 
//      ///      <see cref="ViewModel"/> type.</para>
//      ///   <para>A 'VMDescriptor' instance is associated with exaclty one concrete
//      ///      'ViewModel' class (all instances of a 'ViewModel' have the same 
//      ///      'VMDescriptor' object). Example: The 'PersonVM' CLASS is associated
//      ///      with one 'PersonVMDescriptor' INSTANCE.</para>
//      /// </summary>
//      /// <typeparam name="TVM">
//      ///   The 'ViewModel' class for which a descriptor should be created.
//      /// </typeparam>
//      public static IVMDescriptorBuilder<TVM> For<TVM>() where TVM : IViewModel {
//         return new BuilderExpression<TVM>();
//      }

//      private class BuilderExpression<TVM> : IVMDescriptorBuilder<TVM>, IVMPropertyBuilderProvider<TVM>
//         where TVM : IViewModel {
//         private VMDescriptorConfiguration _configuration;

//         public BuilderExpression() {
//            var template = BehaviorChainTemplateRegistry.GetTemplate(BehaviorChainTemplateKeys.ViewModel);
//            var viewModelConfiguration = template.CreateConfiguration(ViewModelBehaviorFactory.CreateInvoker<TVM>());
//            _configuration = new VMDescriptorConfiguration(viewModelConfiguration);
//         }

//         /// <inheritdoc />
//         public IVMDescriptorBuilder<TVM, TDescriptor> CreateDescriptor<TDescriptor>(
//            Func<IVMPropertyBuilderProvider<TVM>, TDescriptor> descriptorFactory
//         ) where TDescriptor : VMDescriptor {
//            TDescriptor descriptor = descriptorFactory(this);
//            descriptor.InitializePropertyNames();
//            return new BuilderExpression<TVM, TDescriptor>(descriptor, _configuration);
//         }

//         /// <inheritdoc />
//         public IVMPropertyBuilder<TVM> GetPropertyBuilder() {
//            return new VMPropertyBuilder<TVM, TVM>(PropertyPath.Empty<TVM>(), _configuration);
//         }

//         /// <inheritdoc />
//         public IVMPropertyBuilder<TSource> GetPropertyBuilder<TSource>(Expression<Func<TVM, TSource>> sourceObjectSelector) {
//            var path = PropertyPath.Create(sourceObjectSelector);
//            return new VMPropertyBuilder<TVM, TSource>(path, _configuration);
//         }
//      }

//      private class BuilderExpression<TVM, TDescriptor> :
//         ConfigurationProvider,
//         IVMDescriptorBuilder<TVM, TDescriptor>
//         where TVM : IViewModel
//         where TDescriptor : VMDescriptor {

//         private TDescriptor _descriptor;

//         public BuilderExpression(TDescriptor descriptor, VMDescriptorConfiguration configuration)
//            : base(configuration) {
//            _descriptor = descriptor;
//         }

//         public IVMDescriptorBuilder<TVM, TDescriptor> WithValidations(
//            Action<TDescriptor, ValidatorBuilder<TVM, TDescriptor>> validatorConfigurator
//         ) {
//            var validatorBuilder = new ValidatorBuilder<TVM, TDescriptor>(Configuration, _descriptor);
//            validatorConfigurator(_descriptor, validatorBuilder);

//            //throw new NotImplementedException();
//            //validationConfigurator(
//            //   _descriptor,
//            //   new ValidationBuilder<TVM>(_configuration, _descriptor)
//            //);

//            return this;
//         }

//         public IVMDescriptorBuilder<TVM, TDescriptor> WithDependencies(
//            Action<TDescriptor, IVMDependencyConfigurator<TDescriptor>> dependencyConfigurator
//         ) {
//            return this;
//         }

//         public IVMDescriptorBuilder<TVM, TDescriptor> WithBehaviors(
//            Action<IVMBehaviorBuilder<TDescriptor>> behaviorBuildAction
//         ) {
//            Contract.Requires<ArgumentNullException>(behaviorBuildAction != null);
//            var builder = new VMBehaviorBuilder<TDescriptor>(Configuration, _descriptor);
//            behaviorBuildAction(builder);
//            return this;
//         }

//         public TDescriptor Build() {
//            Configuration.ApplyTo(_descriptor);
//            return _descriptor;
//         }
//      }
//   }
//}
