namespace Inspiring.Mvvm.ViewModels.Core {

   internal sealed class UndoSetValueBehavior<TValue> :
      Behavior,
      IBehaviorInitializationBehavior,
      IValueAccessorBehavior<TValue> {

      private IVMPropertyDescriptor _property;

      public void Initialize(BehaviorInitializationContext context) {
         _property = context.Property;
         this.InitializeNext(context);
      }

      public TValue GetValue(IBehaviorContext context) {
         return this.GetValueNext<TValue>(context);
      }

      public void SetValue(IBehaviorContext context, TValue value) {
         TValue oldValue = (TValue)context.VM.Kernel.GetValue(_property);

         var action = new SetValueAction<TValue>(context.VM, _property, oldValue);
         UndoManager.GetManager(context.VM).PushAction(action);
         this.SetValueNext(context, value);
      }
   }
}
