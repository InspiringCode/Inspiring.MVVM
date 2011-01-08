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

      private readonly List<PropertySelector> _properties;

      public VMPropertyPath()
         : this(new List<PropertySelector>()) {
      }

      private VMPropertyPath(List<PropertySelector> properties) {
         _properties = properties;
      }

      public int Length {
         get { return _properties.Count; }
      }

      public VMPropertyPath AddProperty(PropertySelector propertySelector) {
         var propertiesClone = new List<PropertySelector>(_properties);
         propertiesClone.Add(propertySelector);
         return new VMPropertyPath(propertiesClone);
      }

      public IVMProperty GetProperty(int index, VMDescriptorBase declaringDescriptor) {
         Contract.Requires<ArgumentNullException>(declaringDescriptor != null);
         Contract.Requires<IndexOutOfRangeException>(0 <= index && index < Length);
         Contract.Ensures(Contract.Result<IVMProperty>() != null);

         return _properties[index].GetProperty(declaringDescriptor);
      }

      public object GetValue(int index, IViewModel viewModel) {
         Contract.Requires<IndexOutOfRangeException>(0 <= index && index < Length);
         return _properties[index].GetPropertyValue(viewModel);
      }
   }
}
