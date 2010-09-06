namespace Inspiring.Mvvm.ViewModels.Fluent {
   using System;
   using Inspiring.Mvvm.Common;

   /// <summary>
   /// Part of the fluent interface syntax.
   /// </summary>
   public interface IVMDescriptorBuilder<TVM> : IHideObjectMembers {
      /// <summary>
      ///   The first step is to create a new instance of a 'VMDescriptor' 
      ///   subclass and assign its 'VMProperty' properties.
      /// </summary>
      /// <typeparam name="TDescriptor">
      ///   The type of the descriptor that is created by this method. You do
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
      IVMDescriptorConfigurator<TVM, TDescriptor> CreateDescriptor<TDescriptor>(
         Func<IVMPropertyFactoryProvider<TVM>, TDescriptor> descriptorFactory
      ) where TDescriptor : VMDescriptor;
   }
}
