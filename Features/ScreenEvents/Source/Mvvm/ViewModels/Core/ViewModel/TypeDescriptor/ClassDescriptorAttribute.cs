namespace Inspiring.Mvvm.ViewModels {
   using System;
   using System.Linq;
   using System.Reflection;

   /// <summary>
   ///   Marks a static field of type <see cref="VMDescriptor"/> that holds the
   ///   descriptor that should be used by the <see cref="System.ComponentModel.TypeDescriptionProvider"/>
   ///   if the view model is used in a designer for example.
   /// </summary>
   [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
   public sealed class ClassDescriptorAttribute : Attribute {
      public static IVMDescriptor GetClassDescriptorOf(Type viewModelType) {
         if (!typeof(IViewModel).IsAssignableFrom(viewModelType)) {
            return null;
         }

         FieldInfo classDescriptorField = viewModelType
            .GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static)
            .Where(field => Attribute.IsDefined(field, typeof(ClassDescriptorAttribute)))
            .FirstOrDefault();

         if (classDescriptorField != null) {
            return (IVMDescriptor)classDescriptorField.GetValue(null);
         }

         return GetClassDescriptorOf(viewModelType.BaseType);
      }
   }
}
