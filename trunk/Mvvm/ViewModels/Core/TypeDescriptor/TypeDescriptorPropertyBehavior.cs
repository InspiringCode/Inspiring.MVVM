namespace Inspiring.Mvvm.ViewModels.Core {
   using System.Diagnostics.Contracts;

   internal sealed class TypeDescriptorPropertyBehavior :
      Behavior,
      IBehaviorInitializationBehavior,
      IHandlePropertyChangedBehavior {

      private VMPropertyDescriptor _propertyDescriptor;

      public void Initialize(BehaviorInitializationContext context) {
         _propertyDescriptor = new VMPropertyDescriptor(context.Property);
      }

      public void HandlePropertyChanged(IBehaviorContext vm) {
         Contract.Assert(
            _propertyDescriptor != null,
            "Behavior not initialized."
         );

         _propertyDescriptor.RaiseValueChanged(vm);

         IHandlePropertyChangedBehavior next;
         if (TryGetBehavior(out next)) {
            next.HandlePropertyChanged(vm);
         }
      }
   }
}
