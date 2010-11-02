namespace Inspiring.Mvvm.ViewModels.Core {
   using System.ComponentModel;
   using System.Diagnostics.Contracts;
   using System.Linq;
   using Inspiring.Mvvm.ViewModels.Future;

   /// <summary>
   ///   A service registered with a <see cref="VMDescriptor"/> that caches all
   ///   <see cref="VMPropertyDescriptor"/> instances of a VMDescriptor instance.
   /// </summary>
   public sealed class TypeDescriptorService {
      private PropertyDescriptorCollection _propertyDescriptors;
      private VMDescriptorBase _vmDescriptor;

      public TypeDescriptorService(VMDescriptorBase vmDescriptor) {
         Contract.Requires(vmDescriptor != null);

         _vmDescriptor = vmDescriptor;
      }

      public PropertyDescriptorCollection PropertyDescriptors {
         get {
            if (_propertyDescriptors == null) {
               _propertyDescriptors = new PropertyDescriptorCollection(
                  _vmDescriptor
                     .Properties
                     .Select(p => new VMPropertyDescriptor(p))
                     .ToArray()
               );
            }

            return _propertyDescriptors;
         }
      }
   }
}
