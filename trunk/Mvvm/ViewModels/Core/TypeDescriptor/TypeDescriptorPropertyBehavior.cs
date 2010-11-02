namespace Inspiring.Mvvm.ViewModels.Core.TypeDescriptor {
   using System;
   using System.Diagnostics.Contracts;

   [Obsolete]
   internal sealed class TypeDescriptorPropertyBehavior : Behavior, IHandlePropertyChangedBehavior {
      private VMPropertyDescriptor _propertyDescriptor;

      public TypeDescriptorPropertyBehavior(VMPropertyDescriptor propertyDescriptor) {
         Contract.Requires(propertyDescriptor != null);
         _propertyDescriptor = propertyDescriptor;
      }

      public void HandlePropertyChanged(IBehaviorContext vm) {
         _propertyDescriptor.RaiseValueChanged(vm);

         IHandlePropertyChangedBehavior next;
         if (TryGetBehavior(out next)) {
            next.HandlePropertyChanged(vm);
         }
      }
   }
}
