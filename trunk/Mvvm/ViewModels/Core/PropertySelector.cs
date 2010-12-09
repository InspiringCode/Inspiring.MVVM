namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Diagnostics.Contracts;

   /// <summary>
   ///   <para>Stores which VM property of a VM descriptor class was selected.</para>
   ///   <example>
   ///      <para>There are two instances of 'StatusVM' which defines the VM property
   ///         'Title'. This means there are also two VM property instances for the
   ///         'Title' property. The <see cref="PropertySelector"/> allows us to refer 
   ///         to the 'Title' property independently of VM descriptor instance.</para>
   ///      <para>'PropertySelector.Create((PersonVMDescriptor x) => x.Name)' gives
   ///         us a <see cref="PropertySelector"/> instance that returns the first VM 
   ///         property instance if we pass in the first descriptor to the <see 
   ///         cref="GetProperty"/> method and the VM property instance of 
   ///         and the second descriptor if we pass the second descriptor.</para>
   ///   </example>
   /// </summary>
   public abstract class PropertySelector {
      public static PropertySelector Create<TDescriptor>(
         Func<TDescriptor, IVMProperty> propertySelector
      ) where TDescriptor : VMDescriptorBase {
         Contract.Requires(propertySelector != null);
         return new GenericPropertySelector<TDescriptor>(propertySelector);
      }

      /// <summary>
      ///   Returns the VM property of the given <paramref name="descriptor"/>
      ///   that represents the VM property selected by this <see 
      ///   cref="PropertySelector"/>.
      /// </summary>
      /// <param name="descriptor">
      ///   The descriptor instance whos VM property instance should be returned.
      /// </param>
      public abstract IVMProperty GetProperty(VMDescriptorBase descriptor);

      /// <summary>
      ///   Encapsulates the generic argument needed to call the 'selector' 
      ///   function.
      /// </summary>
      private sealed class GenericPropertySelector<TDescriptor> :
         PropertySelector
         where TDescriptor : VMDescriptorBase {

         private readonly Func<TDescriptor, IVMProperty> _selector;

         public GenericPropertySelector(Func<TDescriptor, IVMProperty> selector) {
            Contract.Requires(selector != null);
            _selector = selector;
         }

         public override IVMProperty GetProperty(VMDescriptorBase descriptor) {
            TDescriptor concreteDescriptor = (TDescriptor)descriptor;
            return _selector(concreteDescriptor);
         }
      }
   }
}