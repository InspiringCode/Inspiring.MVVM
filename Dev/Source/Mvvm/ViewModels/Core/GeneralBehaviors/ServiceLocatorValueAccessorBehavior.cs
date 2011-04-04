namespace Inspiring.Mvvm.ViewModels.Core {

   internal sealed class ServiceLocatorValueAccessorBehavior<TValue> :
      Behavior,
      IValueAccessorBehavior<TValue> {

      public TValue GetValue(IBehaviorContext context) {
         return context.ServiceLocator.GetInstance<TValue>();
      }

      public void SetValue(IBehaviorContext context, TValue value) {
         this.SetValueNext(context, value);
      }
   }
}
