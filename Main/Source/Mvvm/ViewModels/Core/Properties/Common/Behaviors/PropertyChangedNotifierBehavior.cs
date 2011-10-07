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

         // Note the use of 'value' instead of a second 'GetValue' call. There
         // may be some sort of cache after this behavior. Calling 'GetValue'
         // would access the real source getter once again which may not be
         // wanted.
         // The only case in which a change would not be detected this way is
         // when the value of the source property changes if it is set to the
         // same value that it returned last time.
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
         return ChangeArgs.PropertyChanged(_property, null);
      }
   }
}
