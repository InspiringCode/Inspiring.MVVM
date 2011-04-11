namespace Inspiring.Mvvm.ViewModels.Core {
   using System;

   internal sealed class RefreshableValueCacheBehavior<TValue> : ValueCacheBehaviorOld<TValue>, IManualUpdateBehavior {
      private IVMPropertyDescriptor _property;

      public RefreshableValueCacheBehavior() {
         throw new NotImplementedException("Redesign this, propably inherit from CacheBehavior");
      }

      public void UpdatePropertyFromSource(IBehaviorContext context) {
         TValue oldValue = GetValue(context);
         CopyFromSource(context);
         TValue newValue = GetValue(context);

         if (!Object.Equals(oldValue, newValue)) {
            var args = new ChangeArgs(ChangeType.PropertyChanged, context.VM, _property);
            context.NotifyChange(args);
         }
      }

      public void UpdatePropertySource(IBehaviorContext vm) {
         CopyToSource(vm);
      }

      public override void Initialize(BehaviorInitializationContext context) {
         base.Initialize(context);
         _property = context.Property;
      }
   }
}
