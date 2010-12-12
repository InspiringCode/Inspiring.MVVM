namespace Inspiring.Mvvm.ViewModels.Core {
   using System;

   internal sealed class CollectionPopulatorBehavior<TItemVM> :
      Behavior,
      IValueAccessorBehavior<IVMCollection<TItemVM>>,
      IMutabilityCheckerBehavior
      where TItemVM : IViewModel {

      public IVMCollection<TItemVM> GetValue(IBehaviorContext context, ValueStage stage) {
         var fac = GetNextBehavior<ICollectionFactoryBehavior<TItemVM>>();
         var coll = fac.CreateCollection(context);
         var behavior = coll.Behaviors.GetNextBehavior<IPopulatorCollectionBehavior<TItemVM>>();

         behavior.Repopulate(context, coll);

         return coll;
      }

      public void SetValue(IBehaviorContext vm, IVMCollection<TItemVM> value) {
         throw new NotSupportedException(
            ExceptionTexts.CannotSetVMCollectionProperties
         );
      }

      public bool IsMutable(IBehaviorContext vm) {
         return false;
      }
   }
}
