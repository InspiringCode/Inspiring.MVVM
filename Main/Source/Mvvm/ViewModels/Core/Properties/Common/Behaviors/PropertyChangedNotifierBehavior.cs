namespace Inspiring.Mvvm.ViewModels.Core {
   using System;

   internal class PropertyChangedNotifierBehavior<TValue> :
      InitializableBehavior,
      IBehaviorInitializationBehavior,
      IChangeNotifierBehavior<TValue>,
      IValueAccessorBehavior<TValue> {

      protected IVMPropertyDescriptor _property;

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
            NotifyPropertyChanged(context, ValueStage.ValidatedValue, oldValue, value);
         }
      }

      public virtual void NotifyPropertyChanged(IBehaviorContext context, ValueStage stage, TValue oldValue, TValue newValue) {
         var args = ChangeArgs.PropertyChanged(
            _property, stage,
            reason: null
         );

         context.NotifyChange(args);

         this.NotifyPropertyChangedNext(context, stage, oldValue, newValue);
      }
   }
}
