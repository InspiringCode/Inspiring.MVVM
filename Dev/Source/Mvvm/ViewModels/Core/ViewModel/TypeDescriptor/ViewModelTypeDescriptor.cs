namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.ComponentModel;
   using System.Diagnostics.Contracts;

   internal sealed class ViewModelTypeDescriptor : CustomTypeDescriptor {
      private readonly IVMDescriptor _descriptor;

      public ViewModelTypeDescriptor(IVMDescriptor descriptor) {
         Contract.Requires(descriptor != null);
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
