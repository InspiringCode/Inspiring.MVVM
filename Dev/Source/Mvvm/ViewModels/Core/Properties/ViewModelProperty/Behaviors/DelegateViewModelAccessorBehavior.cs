using System;
namespace Inspiring.Mvvm.ViewModels.Core {

   internal sealed class DelegateViewModelAccessorBehavior<TValue> :
      CachedAccessorBehavior<TValue>,
      IRefreshBehavior {

      private IVMPropertyDescriptor _property;

      public override void Initialize(BehaviorInitializationContext context) {
         _property = context.Property;
         base.Initialize(context);
      }

      public void Refresh(IBehaviorContext context) {
         RequireInitialized();

         var previousValue = GetValue(context);
         RefreshCache(context);
         var newValue = GetValue(context);

         if (!Object.Equals(newValue, previousValue)) {
            context.NotifyChange(ChangeArgs.PropertyChanged(_property));
         }

         this.RefreshNext(context);
      }

      protected override TValue ProvideValue(IBehaviorContext context) {
         return this.GetValueNext<TValue>(context);
      }
   }
}
