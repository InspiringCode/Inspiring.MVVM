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

      public void Refresh(IBehaviorContext context, RefreshOptions options) {
         RequireInitialized();

         var previousValue = GetValue(context);
         RefreshCache(context);
         var newValue = GetValue(context);

         if (!Object.Equals(newValue, previousValue)) {
            var args = ChangeArgs.ViewModelPropertyChanged(
               _property,
               ValueStage.ValidatedValue,
               previousValue,
               newValue,
               RefreshReason.Create(options.ExecuteRefreshDependencies)
            );

            context.NotifyChange(args);
         }

         this.RefreshNext(context, options);
      }

      protected override TValue ProvideValue(IBehaviorContext context) {
         return this.GetValueNext<TValue>(context);
      }
   }
}
