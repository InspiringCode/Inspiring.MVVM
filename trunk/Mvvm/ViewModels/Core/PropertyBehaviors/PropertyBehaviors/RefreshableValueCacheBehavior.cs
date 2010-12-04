namespace Inspiring.Mvvm.ViewModels.Core {
   using System;

   internal sealed class RefreshableValueCacheBehavior<TValue> : ValueCacheBehavior<TValue>, IManuelUpdateBehavior {
      private IVMProperty _property;

      public void UpdateFromSource(IBehaviorContext context) {
         TValue oldValue = GetValue(context, ValueStage.PostValidation);
         CopyFromSource(context);
         TValue newValue = GetValue(context, ValueStage.PostValidation);

         if (!Object.Equals(oldValue, newValue)) {
            var args = new ChangeArgs(ChangeType.PropertyChanged, context.VM, _property);
            context.NotifyChange(args);
         }
      }

      public void UpdateSource(IBehaviorContext vm) {
         CopyToSource(vm);
      }

      public override void Initialize(BehaviorInitializationContext context) {
         base.Initialize(context);
         _property = context.Property;
      }
   }
}
