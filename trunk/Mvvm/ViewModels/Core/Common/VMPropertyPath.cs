namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Collections.Generic;
   using System.Diagnostics.Contracts;

   /// <summary>
   ///   A class that describes a list of VM proprerties that need to be get in
   ///   the order specified by the VMPropertyPath to get from one VM to an 
   ///   descendant VM in a VM hierarchy.
   /// </summary>
   public sealed class VMPropertyPath {
      public static readonly VMPropertyPath Empty = new VMPropertyPath();

      private readonly List<IPropertyProvider> _properties;

      public VMPropertyPath()
         : this(new List<IPropertyProvider>()) {
      }

      private VMPropertyPath(List<IPropertyProvider> properties) {
         _properties = properties;
      }

      public int Length {
         get { return _properties.Count; }
      }

      public VMPropertyPath AddProperty<TDescriptor>(
         Func<TDescriptor, IVMProperty> propertySelector
      ) where TDescriptor : VMDescriptorBase {
         var propertiesClone = new List<IPropertyProvider>(_properties);

         propertiesClone.Add(
            new PropertyProvider<TDescriptor>(propertySelector)
         );

         return new VMPropertyPath(propertiesClone);
      }

      public IVMProperty GetProperty(int index, VMDescriptorBase declaringDescriptor) {
         Contract.Requires<ArgumentNullException>(declaringDescriptor != null);
         Contract.Requires<IndexOutOfRangeException>(0 <= index && index < Length);
         Contract.Ensures(Contract.Result<IVMProperty>() != null);


         return _properties[index].GetProperty(declaringDescriptor);
      }

      private interface IPropertyProvider {
         IVMProperty GetProperty(VMDescriptorBase descriptor);
      }

      private sealed class PropertyProvider<TDescriptor> :
         IPropertyProvider
         where TDescriptor : VMDescriptorBase {

         private readonly Func<TDescriptor, IVMProperty> _propertySelector;

         public PropertyProvider(Func<TDescriptor, IVMProperty> propertySelector) {
            Contract.Requires(propertySelector != null);
            _propertySelector = propertySelector;
         }

         public IVMProperty GetProperty(VMDescriptorBase descriptor) {
            TDescriptor concreteDescriptor = (TDescriptor)descriptor;
            return _propertySelector(concreteDescriptor);
         }
      }

   }
}
