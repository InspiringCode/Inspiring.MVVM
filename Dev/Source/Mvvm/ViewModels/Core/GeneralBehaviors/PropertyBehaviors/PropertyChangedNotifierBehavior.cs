namespace Inspiring.Mvvm.ViewModels.Core {
   using System;

   internal sealed class PropertyChangedNotifierBehavior<TValue> :
      InitializableBehavior,
      IBehaviorInitializationBehavior,
      IValueAccessorBehavior<TValue>,
      IRefreshBehavior {

      private IVMPropertyDescriptor _property;

      public void Initialize(BehaviorInitializationContext context) {
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

      public void Refresh(IBehaviorContext context) {
         this.RefreshNext(context);
         context.NotifyChange(ChangeArgs.PropertyChanged(_property));
      }
   }
}
