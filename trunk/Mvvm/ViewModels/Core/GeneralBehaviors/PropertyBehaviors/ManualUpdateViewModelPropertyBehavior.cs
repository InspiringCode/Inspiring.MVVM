namespace Inspiring.Mvvm.ViewModels.Core {

   // TODO: Does this really have to be public?
   public sealed class ManualUpdateViewModelPropertyBehavior<TChildVM, TChildSource> :
      Behavior,
      IManualUpdateBehavior
      where TChildVM : IViewModel, IHasSourceObject<TChildSource> {

      public void UpdatePropertyFromSource(IBehaviorContext context) {
         // Refreshes the source value cache if this is a disconnected property.
         this.UpdatePropertyFromSourceNext(context);

         TChildSource sourceValue = this.GetValueNext<TChildSource>(context);
         TChildVM vm = this.GetValueNext<TChildVM>(context);

         vm.Source = sourceValue;
         vm.Kernel.UpdateFromSource();
      }

      public void UpdatePropertySource(IBehaviorContext context) {
         // Handled by the source value cache if this is a disconnected property.
         this.UpdatePropertySourceNext(context);
      }
   }
}
