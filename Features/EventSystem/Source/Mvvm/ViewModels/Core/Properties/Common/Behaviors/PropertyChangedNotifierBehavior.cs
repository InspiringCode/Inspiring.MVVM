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
            var args = CreateChangeArgs(_property, oldValue, value);
            context.NotifyChange(args);
         }
      }

      protected virtual ChangeArgs CreateChangeArgs(
         IVMPropertyDescriptor property,
         TValue oldValue,
         TValue newValue
      ) {
         return ChangeArgs.PropertyChanged(_property);
      }
   }
}
