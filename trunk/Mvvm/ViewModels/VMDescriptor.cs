namespace Inspiring.Mvvm.ViewModels {
   using System;
   using System.Collections.Generic;
   using System.ComponentModel;
   using System.Linq;
   using System.Reflection;
   using Inspiring.Mvvm.ViewModels.Core;

   public class VMDescriptor {
      private List<VMPropertyBase> _properties = new List<VMPropertyBase>();
      private PropertyDescriptorCollection _propertyDescriptors;

      public VMDescriptor() {
         DynamicFields = new FieldDefinitionCollection();
         Validators = new List<Action<ViewModelValidationArgs>>();
      }

      internal FieldDefinitionCollection DynamicFields { get; private set; }

      internal IEnumerable<VMPropertyBase> Properties {
         get { return _properties; }
      }

      internal List<Action<ViewModelValidationArgs>> Validators { get; private set; }

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

      internal void InitializeProperties() {
         GetType()
            .GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
            .Where(p => typeof(VMPropertyBase).IsAssignableFrom(p.PropertyType))
            .ForEach(p => {
               VMPropertyBase property = (VMPropertyBase)p.GetValue(this, null);
               if (property != null) {
                  _properties.Add(property);
                  property.Initialize(p.Name, this);
               }
            });
      }

   }
}

namespace Inspiring.Mvvm.ViewModels.Future {
   using System.Collections.Generic;
   using System.Linq;
   using System.Reflection;
   using Inspiring.Mvvm.ViewModels.Core;

   public class VMDescriptor : VMDescriptorBase {
      public VMDescriptor() {
         RegisterService(new TypeDescriptorService(this));
         RegisterService(new ViewModelValidatorHolder());
         RegisterService(new FieldDefinitionCollection());
      }

      internal void InitializePropertyNames() {
         foreach (PropertyInfo pi in GetVMPropertyDefinitions()) {
            VMPropertyBase property = GetVMProperty(pi);
            property.Initialize(pi.Name);
         }
      }

      protected override VMPropertyCollection DiscoverProperties() {
         IEnumerable<VMPropertyBase> properties = GetVMPropertyDefinitions()
            .Select(GetVMProperty);

         return new VMPropertyCollection(properties.ToArray());
      }

      private static bool IsVMPropertyDefinition(PropertyInfo pi) {
         return typeof(VMPropertyBase).IsAssignableFrom(pi.PropertyType);
      }

      private IEnumerable<PropertyInfo> GetVMPropertyDefinitions() {
         return GetType()
            .GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
            .Where(IsVMPropertyDefinition);
      }

      private VMPropertyBase GetVMProperty(PropertyInfo pi) {
         return (VMPropertyBase)pi.GetValue(this, null);
      }
   }
}
