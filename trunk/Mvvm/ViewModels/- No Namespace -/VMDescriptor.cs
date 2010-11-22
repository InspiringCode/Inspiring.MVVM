namespace Inspiring.Mvvm.ViewModels {
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
