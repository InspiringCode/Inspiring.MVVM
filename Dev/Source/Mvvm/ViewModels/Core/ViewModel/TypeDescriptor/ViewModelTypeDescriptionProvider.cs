namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.ComponentModel;
   using System.Linq;
   using System.Reflection;

   /// <summary>
   ///   Returns a <see cref="ViewModelTypeDescriptor"/> for the <see cref="VMDescriptor"/>
   ///   defined by a static field marked with <see cref="ClassDescriptorAttribute"/>.
   /// </summary>
   internal sealed class ViewModelTypeDescriptionProvider : TypeDescriptionProvider {
      public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance) {
         if (instance != null) {
            // Interface is implemented by ViewModelWithTypeDescriptor
            return (ICustomTypeDescriptor)instance;
         }

         if (objectType != null) {
            FieldInfo classDescriptorField = objectType
               .GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static)
               .Where(field => Attribute.IsDefined(field, typeof(ClassDescriptorAttribute)))
               .FirstOrDefault();

            if (classDescriptorField != null) {
               var d = (IVMDescriptor)classDescriptorField.GetValue(null);
               return new ViewModelTypeDescriptor(d);
            }
         }

         return base.GetTypeDescriptor(objectType, instance);
      }
   }
}
