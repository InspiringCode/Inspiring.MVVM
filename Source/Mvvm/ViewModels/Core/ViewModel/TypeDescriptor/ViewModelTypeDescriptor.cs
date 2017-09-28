namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.ComponentModel;
   
   internal sealed class ViewModelTypeDescriptor : CustomTypeDescriptor {
      private readonly IVMDescriptor _descriptor;

      public ViewModelTypeDescriptor(IVMDescriptor descriptor) {
         Check.NotNull(descriptor, nameof(descriptor));
         _descriptor = descriptor;
      }

      public override PropertyDescriptorCollection GetProperties() {
         return GetPropertyDescriptors();
      }

      public override PropertyDescriptorCollection GetProperties(Attribute[] attributes) {
         return GetPropertyDescriptors();
      }

      private PropertyDescriptorCollection GetPropertyDescriptors() {
         return _descriptor
            .Behaviors
            .GetNextBehavior<TypeDescriptorProviderBehavior>()
            .PropertyDescriptors;
      }
   }
}
