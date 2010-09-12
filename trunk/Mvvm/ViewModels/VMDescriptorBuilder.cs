namespace Inspiring.Mvvm.ViewModels {
   using System;
   using System.Linq.Expressions;
   using Inspiring.Mvvm.Common;
   using Inspiring.Mvvm.ViewModels.Core;

   /// <summary>
   ///   Use this builder to create and initialize 'VMDescriptor' objects. The
   ///   builder implements the "fluent interface" pattern.
   /// </summary>
   public static class VMDescriptorBuilder {
      /// <summary>
      ///   <para>Returns a builder expression that creates and configure a new 
      ///      concrete <see cref="VMDescriptor"/> for the given 
      ///      <see cref="ViewModel"/> type.</para>
      ///   <para>A 'VMDescriptor' instance is associated with exaclty one concrete
      ///      'ViewModel' class (all instances of a 'ViewModel' have the same 
      ///      'VMDescriptor' object). Example: The 'PersonVM' CLASS is associated
      ///      with one 'PersonVMDescriptor' INSTANCE.</para>
      /// </summary>
      /// <typeparam name="TVM">
      ///   The 'ViewModel' class for which a descriptor should be created.
      /// </typeparam>
      public static IVMDescriptorBuilder<TVM> For<TVM>() where TVM : ViewModel {
         return new BuilderExpression<TVM>();
      }

      private class BuilderExpression<TVM> : IVMDescriptorBuilder<TVM>, IVMPropertyFactoryProvider<TVM>
         where TVM : ViewModel {
         private BehaviorConfigurationDictionary _configurations = new BehaviorConfigurationDictionary();

         public IVMDescriptorBuilder<TVM, TDescriptor> CreateDescriptor<TDescriptor>(
            Func<IVMPropertyFactoryProvider<TVM>, TDescriptor> descriptorFactory
         ) where TDescriptor : VMDescriptor {
            TDescriptor descriptor = descriptorFactory(this);
            descriptor.InitializeProperties();
            return new BuilderExpression<TVM, TDescriptor>(descriptor, _configurations);
         }

         public IRootVMPropertyFactory<TVM> GetPropertyFactory() {
            return new VMPropertyFactory<TVM, TVM>(
               PropertyPath.Empty<TVM>(),
               _configurations
            );
         }

         public IVMPropertyFactory<TSource> GetPropertyFactory<TSource>(
            Expression<Func<TVM, TSource>> sourceObjectSelector
         ) {
            return new VMPropertyFactory<TVM, TSource>(
               PropertyPath.Create(sourceObjectSelector),
               _configurations
            );
         }
      }

      private class BuilderExpression<TVM, TDescriptor> : IVMDescriptorBuilder<TVM, TDescriptor>
         where TDescriptor : VMDescriptor {

         private TDescriptor _descriptor;
         private BehaviorConfigurationDictionary _configurations;

         public BuilderExpression(TDescriptor descriptor, BehaviorConfigurationDictionary configurations) {
            _descriptor = descriptor;
            _configurations = configurations;
         }

         public IVMDescriptorBuilder<TVM, TDescriptor> WithValidations(
            Action<TDescriptor, IVMValidationConfigurator> validationConfigurator
         ) {
            return this;
         }

         public IVMDescriptorBuilder<TVM, TDescriptor> WithDependencies(
            Action<TDescriptor, IVMDependencyConfigurator> dependencyConfigurator
         ) {
            return this;
         }

         public IVMDescriptorBuilder<TVM, TDescriptor> WithBehaviors(
            Action<TDescriptor, IVMBehaviorConfigurator> behaviorConfigurator
         ) {
            return this;
         }

         public TDescriptor Build() {
            _configurations.ApplyToProperties();
            return _descriptor;
         }
      }
   }
}
