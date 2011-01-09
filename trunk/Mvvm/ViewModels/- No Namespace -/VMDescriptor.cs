namespace Inspiring.Mvvm.ViewModels {
   using System.Collections.Generic;
   using System.Linq;
   using System.Reflection;
   using Inspiring.Mvvm.ViewModels.Core;

   public class VMDescriptor : VMDescriptorBase {
      public VMDescriptor() {
         var b = new TypeDescriptorBehavior();
         b.Initialize(new BehaviorInitializationContext(this));
         Behaviors.Successor = b;
      }

      /// <summary>
      ///   Gets or sets the builder that was used to create and configure this
      ///   instance. This is esepcially important for VM inheritance.
      /// </summary>
      internal IVMDescriptorBuilder Builder {
         get;
         set;
      }

      internal void InitializePropertyNames() {
         foreach (PropertyInfo pi in GetVMPropertyDefinitions()) {
            IVMPropertyDescriptor property = GetVMProperty(pi);
            if (property != null) {
               // TODO: Is this the optimal way?
               property.Initialize(pi.Name);
            }
         }
      }

      protected override VMPropertyCollection DiscoverProperties() {
         IEnumerable<IVMPropertyDescriptor> properties = GetVMPropertyDefinitions()
            .Select(GetVMProperty);

         return new VMPropertyCollection(properties.ToArray());
      }

      private static bool IsVMPropertyDefinition(PropertyInfo pi) {
         return typeof(IVMPropertyDescriptor).IsAssignableFrom(pi.PropertyType);
      }

      private IEnumerable<PropertyInfo> GetVMPropertyDefinitions() {
         return GetType()
            .GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
            .Where(IsVMPropertyDefinition);
      }

      private IVMPropertyDescriptor GetVMProperty(PropertyInfo pi) {
         return (IVMPropertyDescriptor)pi.GetValue(this, null);
      }
   }
}
