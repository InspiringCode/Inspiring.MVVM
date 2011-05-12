namespace Inspiring.Mvvm.ViewModels.Core {
   using System;

   internal sealed class PropertySelector<TDescriptor> :
      IPropertySelector
      where TDescriptor : IVMDescriptor {
      private readonly Func<TDescriptor, IVMPropertyDescriptor> _propertySelector;

      public PropertySelector(Func<TDescriptor, IVMPropertyDescriptor> propertySelector) {
         _propertySelector = propertySelector;
      }

      public IVMPropertyDescriptor GetProperty(TDescriptor desriptor) {
         return _propertySelector(desriptor);
      }

      public IVMPropertyDescriptor GetProperty(IVMDescriptor descriptor) {
         return GetProperty((TDescriptor)descriptor);
      }
   }
}