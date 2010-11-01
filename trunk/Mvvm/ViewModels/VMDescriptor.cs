namespace Inspiring.Mvvm.ViewModels {
   using System;
   using System.Collections.Generic;
   using System.ComponentModel;
   using System.Diagnostics.Contracts;
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

      internal VMPropertyBase GetProperty(string propertyName) {
         Contract.Requires(propertyName != null);
         Contract.Ensures(Contract.Result<VMPropertyBase>() != null);

         VMPropertyBase property = _properties.Find(x => x.PropertyName == propertyName);

         if (property == null) {
            throw new InvalidOperationException(
               ExceptionTexts.PropertyNotFound.FormatWith(propertyName)
            );
         }

         return property;
      }

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
