namespace Inspiring.Mvvm.ViewModels {
   using Inspiring.Mvvm.ViewModels.Core;

   internal sealed class ManualUpdateSelectionPropertyBehavior<TChildVM, TChildSource> :
      Behavior,
      IManualUpdateBehavior
      where TChildVM : IViewModel {

      public void UpdatePropertyFromSource(IBehaviorContext context) {
         // Refreshes the source value cache if this is a disconnected property.
         this.UpdatePropertyFromSourceNext(context);

         TChildSource sourceValue = this.GetValueNext<TChildSource>(context);
         TChildVM vm = this.GetValueNext<TChildVM>(context);

         var initializableVM = (ICanInitializeFrom<TChildSource>)vm;
         initializableVM.InitializeFrom(sourceValue);

         vm.Kernel.UpdateFromSource();
      }

      public void UpdatePropertySource(IBehaviorContext context) {
         // Handled by the source value cache if this is a disconnected property.
         this.UpdatePropertySourceNext(context);
      }
   }
}
