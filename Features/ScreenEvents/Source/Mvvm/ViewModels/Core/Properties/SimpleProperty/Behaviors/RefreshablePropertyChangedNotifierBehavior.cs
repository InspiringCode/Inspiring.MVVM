namespace Inspiring.Mvvm.ViewModels.Core {

   internal sealed class RefreshablePropertyChangedNotifierBehavior<TValue> :
      PropertyChangedNotifierBehavior<TValue>,
      IBehaviorInitializationBehavior,
      IRefreshBehavior {

      private IVMPropertyDescriptor _property;

      public override void Initialize(BehaviorInitializationContext context) {
         _property = context.Property;
         base.Initialize(context);
      }

      public void Refresh(IBehaviorContext context, RefreshOptions options) {
         context.NotifyChange(
            ChangeArgs.PropertyChanged(
               _property,
               ValueStage.ValidatedValue,
               RefreshReason.Create(options.ExecuteRefreshDependencies)
            )
         );

         this.RefreshNext(context, options);
      }
   }
}
