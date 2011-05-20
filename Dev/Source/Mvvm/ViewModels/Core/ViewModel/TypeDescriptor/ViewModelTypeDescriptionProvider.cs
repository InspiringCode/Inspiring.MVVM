namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.ComponentModel;

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
            IVMDescriptor classDescriptor = ClassDescriptorAttribute.GetClassDescriptorOf(objectType);
            if (classDescriptor != null) {
               return new ViewModelTypeDescriptor(classDescriptor);
            }
         }

         return base.GetTypeDescriptor(objectType, instance);
      }
   }
}
