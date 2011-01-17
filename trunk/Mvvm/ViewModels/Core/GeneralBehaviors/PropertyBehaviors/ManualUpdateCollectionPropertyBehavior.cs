namespace Inspiring.Mvvm.ViewModels.Core {

   internal sealed class ManualUpdateCollectionPropertyBehavior<TItemVM> :
      Behavior,
      IManualUpdateBehavior
      where TItemVM : IViewModel {

      public void UpdatePropertyFromSource(IBehaviorContext context) {
         this.UpdatePropertyFromSourceNext(context);

         //var collection = this.GetValueNext<IVMCollection<TItemVM>>(context);
         var repopulationBehavior = this.GetNextBehavior<CollectionPopulatorBehavior<TItemVM>>();

         repopulationBehavior.Populate(context);
      }

      public void UpdatePropertySource(IBehaviorContext context) {
         this.UpdatePropertySourceNext(context);
      }
   }
}
