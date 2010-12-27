namespace Inspiring.Mvvm.ViewModels.Core {

   internal sealed class ManualUpdateCollectionPropertyBehavior<TItemVM> :
      Behavior,
      IManualUpdateBehavior
      where TItemVM : IViewModel {

      public void UpdateFromSource(IBehaviorContext context) {
         this.UpdateFromSourceNext(context);

         var collection = this.GetValueNext<IVMCollection<TItemVM>>(context);
         var repopulationBehavior = this.GetNextBehavior<CollectionPopulatorBehavior<TItemVM>>();

         repopulationBehavior.Repopulate(context, collection);
      }

      public void UpdateSource(IBehaviorContext context) {
         this.UpdateSourceNext(context);
      }
   }
}
