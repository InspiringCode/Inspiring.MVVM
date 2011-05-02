namespace Inspiring.Mvvm.ViewModels.Core {
   using System;

   internal class PropertyChangedNotifierBehavior<TValue> :
      InitializableBehavior,
      IBehaviorInitializationBehavior,
      IValueAccessorBehavior<TValue> {

      private IVMPropertyDescriptor _property;

      public virtual void Initialize(BehaviorInitializationContext context) {
         _property = context.Property;
         this.InitializeNext(context);
         SetInitialized();
      }

      public TValue GetValue(IBehaviorContext context) {
         RequireInitialized();
         return this.GetValueNext<TValue>(context);
      }

      public void SetValue(IBehaviorContext context, TValue value) {
         RequireInitialized();

         TValue oldValue = GetValue(context);
         this.SetValueNext(context, value);

         if (!Object.Equals(value, oldValue)) {
            context.NotifyChange(ChangeArgs.PropertyChanged(_property));
         }
      }
   }
}
