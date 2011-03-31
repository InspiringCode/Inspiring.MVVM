namespace Inspiring.Mvvm.ViewModels.Core {

   internal sealed class UndoSetValueBehavior<TValue> :
      Behavior,
      IValueAccessorBehavior<TValue> {

      public TValue GetValue(IBehaviorContext context) {
         return this.GetValueNext<TValue>(context);
      }

      public void SetValue(IBehaviorContext context, TValue value) {
         this.SetValueNext(context, value);
      }
   }
}
