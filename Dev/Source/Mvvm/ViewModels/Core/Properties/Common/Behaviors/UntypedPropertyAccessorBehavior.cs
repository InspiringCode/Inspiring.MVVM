namespace Inspiring.Mvvm.ViewModels.Core {

   internal sealed class UntypedPropertyAccessorBehavior<TValue> :
      Behavior,
      IUntypedValueGetterBehavior,
      IUntypedValueSetterBehavior {

      public object GetValue(IBehaviorContext context) {
         return this.GetValueNext<TValue>(context);
      }

      public void SetValue(IBehaviorContext context, object value) {
         this.SetValueNext(context, (TValue)value);
      }
   }
}
