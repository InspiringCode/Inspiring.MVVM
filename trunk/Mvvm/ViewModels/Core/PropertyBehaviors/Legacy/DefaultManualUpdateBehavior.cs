namespace Inspiring.Mvvm.ViewModels.Core {

   internal sealed class DefaultManualUpdateBehavior<TValue> : Behavior, IManuelUpdateBehavior {
      private VMPropertyBase<TValue> _property;

      public void UpdateFromSource(IBehaviorContext vm) {
         vm.RaisePropertyChanged(_property);
      }

      public void UpdateSource(IBehaviorContext vm) {
         // Nothing to do
      }

      protected override void Initialize(BehaviorInitializationContext context) {
         base.Initialize(context);
         _property = (VMPropertyBase<TValue>)context.Property;
      }
   }
}
