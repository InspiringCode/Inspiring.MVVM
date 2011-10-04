namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Linq;
   using Inspiring.Mvvm.Common;

   internal sealed class PropertySelector<TDescriptor> :
      IPropertySelector
      where TDescriptor : IVMDescriptor {

      private readonly Func<TDescriptor, IVMPropertyDescriptor> _propertySelector;
      private string _propertyName;

      public PropertySelector(Func<TDescriptor, IVMPropertyDescriptor> propertySelector) {
         _propertySelector = propertySelector;
      }

      public string PropertyName {
         get {
            if (_propertyName == null) {
               // Only parse the IL code if we really need the property name
               // because this operation may be expensive.
               var parser = new ILParser(_propertySelector.Method);
               _propertyName = parser
                  .GetAccessedProperties()
                  .Single()
                  .Name;
            }

            return _propertyName;
         }
      }

      public IVMPropertyDescriptor GetProperty(TDescriptor desriptor) {
         return _propertySelector(desriptor);
      }

      public IVMPropertyDescriptor GetProperty(IVMDescriptor descriptor) {
         return GetProperty((TDescriptor)descriptor);
      }

      public override string ToString() {
         string descriptorTypeName = TypeService.GetFriendlyName(typeof(TDescriptor));
         return String.Format("{0}.{1}", descriptorTypeName, PropertyName);
      }
   }
}