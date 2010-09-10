namespace Inspiring.Mvvm.ViewModels.Fluent {
   using System;

   /// <summary>
   /// Part of the fluent interface syntax.
   /// </summary>
   public interface IVMDescriptorConfigurator<TVM, TDescriptor>
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
      IVMDescriptorConfigurator<TVM, TDescriptor> WithValidations(
         Action<TDescriptor, IVMValidationConfigurator> validationConfigurator
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
      IVMDescriptorConfigurator<TVM, TDescriptor> WithDependencies(
         Action<TDescriptor, IVMDependencyConfigurator> dependencyConfigurator
      );

      IVMDescriptorConfigurator<TVM, TDescriptor> WithBehaviors(
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
