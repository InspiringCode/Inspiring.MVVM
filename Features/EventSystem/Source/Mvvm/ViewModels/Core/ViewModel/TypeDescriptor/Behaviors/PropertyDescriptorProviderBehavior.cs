namespace Inspiring.Mvvm.ViewModels.Core {
   using System.Diagnostics.Contracts;
   using System.ComponentModel;

   internal sealed class PropertyDescriptorProviderBehavior<TValue> :
      Behavior,
      IBehaviorInitializationBehavior,
      IPropertyDescriptorProviderBehavior,
      IHandlePropertyChangedBehavior {

      private IVMPropertyDescriptor<TValue> _property;
      private ViewModelPropertyDescriptor<TValue> _descriptor;

      public PropertyDescriptor PropertyDescriptor {
         get {
            if (_descriptor == null) {
               AssertInitialized();
               _descriptor = new ViewModelPropertyDescriptor<TValue>(_property);
            }

            return _descriptor;
         }
      }

      public void Initialize(BehaviorInitializationContext context) {
         _property = (IVMPropertyDescriptor<TValue>)context.Property;
         this.InitializeNext(context);
      }

      public void HandlePropertyChanged(IBehaviorContext context) {
         AssertInitialized();

         if (_descriptor != null) {
            _descriptor.RaiseValueChanged(context.VM);
         }

         IHandlePropertyChangedBehavior next;
         if (TryGetBehavior(out next)) {
            next.HandlePropertyChanged(context);
         }
      }

      private void AssertInitialized() {
         Contract.Assert(_property != null, "Behavior not initialized.");
      }
   }
}
