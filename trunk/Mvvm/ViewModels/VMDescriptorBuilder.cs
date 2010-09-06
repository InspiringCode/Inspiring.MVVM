namespace Inspiring.Mvvm.ViewModels {
   using System;
   using System.Linq.Expressions;
   using Inspiring.Mvvm.Common;
   using Inspiring.Mvvm.ViewModels.Fluent;

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
         return new VMDescriptorBuilder<TVM>();
      }


   }

   internal class VMDescriptorBuilder<TVM> : IVMDescriptorBuilder<TVM>, IVMPropertyFactoryProvider<TVM> where TVM : ViewModel {
      public IVMDescriptorConfigurator<TVM, TDescriptor> CreateDescriptor<TDescriptor>(
         Func<IVMPropertyFactoryProvider<TVM>, TDescriptor> descriptorFactory
      ) where TDescriptor : VMDescriptor {
         TDescriptor descriptor = descriptorFactory(this);
         descriptor.Initialize();
         return new VMDescriptorConfigurator<TVM, TDescriptor>(descriptor);
      }

      public IRootVMPropertyFactory<TVM> GetPropertyFactory() {
         return new VMPropertyFactory<TVM, TVM>(PropertyPath.Empty<TVM>());
      }

      public IVMPropertyFactory<TSource> GetPropertyFactory<TSource>(Expression<Func<TVM, TSource>> sourceObjectSelector) {
         return new VMPropertyFactory<TVM, TSource>(PropertyPath.Create(sourceObjectSelector));
      }
   }

   internal class VMDescriptorConfigurator<TVM, TDescriptor> : IVMDescriptorConfigurator<TVM, TDescriptor>
      where TDescriptor : VMDescriptor {

      private TDescriptor _descriptor;

      public VMDescriptorConfigurator(TDescriptor descriptor) {
         _descriptor = descriptor;
      }

      public IVMDescriptorConfigurator<TVM, TDescriptor> WithValidations(Action<TDescriptor, IVMValidationConfigurator> validationConfigurator) {
         return this;
      }

      public IVMDescriptorConfigurator<TVM, TDescriptor> WithDependencies(Action<TDescriptor, IVMDependencyConfigurator> dependencyConfigurator) {
         return this;
      }

      public IVMDescriptorConfigurator<TVM, TDescriptor> WithBehaviors(Action<TDescriptor, IVMBehaviorConfigurator> behaviorConfigurator) {
         return this;
      }

      public TDescriptor Build() {
         return _descriptor;
      }
   }
}
