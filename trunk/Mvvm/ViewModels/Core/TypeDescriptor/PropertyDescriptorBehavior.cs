namespace Inspiring.Mvvm.ViewModels.Core {
   using System.Diagnostics.Contracts;

   internal sealed class PropertyDescriptorBehavior :
      Behavior,
      IBehaviorInitializationBehavior,
      IHandlePropertyChangedBehavior {

      private IVMProperty _property;
      private VMPropertyDescriptor _descriptor;

      public VMPropertyDescriptor PropertyDescriptor {
         get {
            if (_descriptor == null) {
               AssertInitialized();
               _descriptor = new VMPropertyDescriptor(_property);
            }

            return _descriptor;
         }
      }

      public void Initialize(BehaviorInitializationContext context) {
         _property = context.Property;
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
