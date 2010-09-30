namespace Inspiring.Mvvm.ViewModels {
   using System.Collections.Generic;
   using System.ComponentModel;
   using System.Linq;
   using System.Reflection;
   using Inspiring.Mvvm.ViewModels.Core;
   using System;
   using System.Diagnostics.Contracts;

   public class VMDescriptor {
      private List<VMProperty> _properties = new List<VMProperty>();
      private PropertyDescriptorCollection _propertyDescriptors;

      public VMDescriptor() {
         DynamicFields = new FieldDefinitionCollection();
      }

      internal FieldDefinitionCollection DynamicFields { get; private set; }

      internal IEnumerable<VMProperty> Properties {
         get { return _properties; }
      }

      internal VMProperty GetProperty(string propertyName) {
         Contract.Requires(propertyName != null);
         Contract.Ensures(Contract.Result<VMProperty>() != null);

         VMProperty property = _properties.Find(x => x.PropertyName == propertyName);

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
            .Where(p => typeof(VMProperty).IsAssignableFrom(p.PropertyType))
            .ForEach(p => {
               VMProperty property = (VMProperty)p.GetValue(this, null);
               if (property != null) {
                  _properties.Add(property);
                  property.Initialize(p.Name, this);
               }
            });
      }

   }
}
