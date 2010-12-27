namespace Inspiring.Mvvm.ViewModels.Core {

   internal sealed class ManualUpdateViewModelPropertyBehavior<TChildVM, TChildSource> :
      Behavior,
      IManualUpdateBehavior
      where TChildVM : IViewModel, ICanInitializeFrom<TChildSource> {

      public void UpdateFromSource(IBehaviorContext context) {
         // Refreshes the source value cache if this is a disconnected property.
         this.UpdateFromSourceNext(context);

         TChildSource sourceValue = this.GetValueNext<TChildSource>(context);
         TChildVM vm = this.GetValueNext<TChildVM>(context);

         vm.InitializeFrom(sourceValue);
         vm.Kernel.UpdateFromSource();
      }

      public void UpdateSource(IBehaviorContext context) {
         // Handled by the source value cache if this is a disconnected property.
         this.UpdateSourceNext(context);
      }
   }
}
