namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using Inspiring.Mvvm.Common;

   /// <summary>
   /// Part of the fluent interface syntax.
   /// </summary>
   public interface IVMDescriptorBuilder<TVM> : IHideObjectMembers where TVM : IViewModel {
      /// <summary>
      ///   The first step is to create a new instance of a 'VMDescriptor' 
      ///   subclass and assign its 'VMProperty' properties.
      /// </summary>
      /// <typeparam name="TDescriptor">
      ///   The type of the descriptor that is created by this method. You do not
      ///   have to specify it because it can be inferred. Note that this type
      ///   must be the same as the first generic parameter of the 'ViewModel'
      ///   class. Example: If 'PersonVM' derives from 'ViewModel(of 
      ///   PersonVMDescriptor)' the 'TDescriptor' must be 'PersonVMDescriptor'.
      /// </typeparam>
      /// <param name="descriptorFactory">
      ///   <para>This delegate should create a new instance of a 'VMDescriptor' 
      ///      subclass, assign its 'VMProperty' properties and return the new 
      ///      instance.</para>
      ///   <para>Use the passed in '<see cref="IVMPropertyFactoryProvider"/>' to
      ///      create 'VMProperty' instances and assign them to your 
      ///      'VMDescriptor'. Example: <code>CreateDescriptor(c => {
      ///         var v = c.GetPropertyFactory();
      ///         return new PersonVMDescriptor {
      ///            FirstName = v.Mapped(x => x.Person.FirstName);
      ///         };
      ///      })</code>.
      ///   </para>
      /// </param>
      IVMDescriptorBuilder<TVM, TDescriptor> CreateDescriptor<TDescriptor>(
         Func<IVMPropertyBuilderProvider<TVM>, TDescriptor> descriptorFactory
      ) where TDescriptor : VMDescriptor;
   }

   /// <summary>
   /// Part of the fluent interface syntax.
   /// </summary>
   public interface IVMDescriptorBuilder<TVM, TDescriptor>
      where TVM : IViewModel
      where TDescriptor : VMDescriptor {

      /// <summary>
      ///   Use this method to configure how the VM properties of the 
      ///   'VMDescriptor' created in the first step are validated.
      /// </summary>
      /// <param name="validationConfigurator">
      ///   The first argument is the 'VMDescriptor' instance created in the 
      ///   first step. The second argument is an object used to configure 
      ///   validations. Example: <code>WithValidations((d, c) => {
      ///      c.Check(d.FirstName).HasValue();
      ///   })</code>.
      /// </param>
      IVMDescriptorBuilder<TVM, TDescriptor> WithValidations(
         Action<TDescriptor, ValidatorBuilder<TVM, TDescriptor>> validationConfigurator
      );

      /// <summary>
      ///   Use this method to setup dependencies between properties. A property
      ///   'A' depends on 'B' if a change of 'B' also results in a change of 'A'.
      ///   Example: If 'Name' is calculated by 'FirstName + " "  + LastName' the
      ///   property 'Name' depends on the properties 'FirstName' and 'LastName'.
      ///   This relation is important for correct change notification.
      /// </summary>
      /// <param name="dependencyConfigurator">
      ///   The first argument is the 'VMDescriptor' instance created in the 
      ///   first step. The second argument is an object used to setup 
      ///   dependencies. Example: <code>WithDependencies((d, c) => {
      ///      c.Properties(d.Name).DependOn(d.FirstName, d.LastName);
      ///   })</code>.
      /// </param>
      IVMDescriptorBuilder<TVM, TDescriptor> WithDependencies(
         Action<TDescriptor, IVMDependencyConfigurator> dependencyConfigurator
      );

      IVMDescriptorBuilder<TVM, TDescriptor> WithBehaviors(
         Action<TDescriptor, IVMBehaviorConfigurator> behaviorConfigurator
      );

      /// <summary>
      ///   The last step is to return the fully configured 'VMDescriptor'. You
      ///   should assign the result to a public static readonly field and pass
      ///   it to the base constructor of your 'ViewModel'. See example.
      /// </summary>     
      /// <example>
      ///   <code>
      ///      TODO: Example here.
      ///   </code>
      /// </example>
      TDescriptor Build();
   }
}
