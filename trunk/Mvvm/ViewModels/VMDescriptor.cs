namespace Inspiring.Mvvm.ViewModels {
   using System.Collections.Generic;
   using System.ComponentModel;
   using System.Linq;
   using System.Reflection;
   using Inspiring.Mvvm.ViewModels.Behaviors;

   public class VMDescriptor {
      private List<VMProperty> _properties = new List<VMProperty>();
      private PropertyDescriptorCollection _propertyDescriptors;

      public VMDescriptor() {
         DynamicFields = new FieldDefinitionCollection();
      }

      internal FieldDefinitionCollection DynamicFields { get; private set; }

      internal PropertyDescriptorCollection PropertyDescriptors {
         get {
            if (_propertyDescriptors == null) {
               PropertyDescriptor[] descriptors = _properties
                  .Select(p => p.PropertyDescriptor)
                  .ToArray();

               _propertyDescriptors = new PropertyDescriptorCollection(descriptors);
            }

            return _propertyDescriptors;
         }
      }

      internal void Initialize() {
         GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => typeof(VMProperty).IsAssignableFrom(p.PropertyType))
            .ForEach(p => {
               VMProperty property = (VMProperty)p.GetValue(this, null);
               _properties.Add(property);
               if (property != null) {
                  property.Initialize(p.Name, this);
               }
            });
      }

   }
}
