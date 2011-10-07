using System;
namespace Inspiring.Mvvm.ViewModels.Core {

   internal sealed class DelegateViewModelAccessorBehavior<TValue> :
      CachedAccessorBehavior<TValue>,
      IRefreshBehavior
      where TValue : IViewModel {

      private IVMPropertyDescriptor _property;

      public override void Initialize(BehaviorInitializationContext context) {
         _property = context.Property;
         base.Initialize(context);
      }

      public void Refresh(IBehaviorContext context, bool executeRefreshDependencies) {
         RequireInitialized();

         var previousValue = GetValue(context);
         RefreshCache(context);
         var newValue = GetValue(context);

         if (!Object.Equals(newValue, previousValue)) {
            var args = ChangeArgs.ViewModelPropertyChanged(
               _property,
               previousValue,
               newValue
            );

            context.NotifyChange(args);
         }

         this.RefreshNext(context, executeRefreshDependencies);
      }

      protected override TValue ProvideValue(IBehaviorContext context) {
         return this.GetValueNext<TValue>(context);
      }
   }
}
