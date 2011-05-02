namespace Inspiring.Mvvm.ViewModels.Core {

   internal sealed class SimplePropertyChangeNotifierBehavior :
      Behavior,
      IBehaviorInitializationBehavior,
      IRefreshBehavior {

      private IVMPropertyDescriptor _property;

      public void Initialize(BehaviorInitializationContext context) {
         _property = context.Property;
         this.InitializeNext(context);
      }

      public void Refresh(IBehaviorContext context) {
         context.NotifyChange(ChangeArgs.PropertyChanged(_property));
         this.RefreshNext(context);
      }
   }
}
