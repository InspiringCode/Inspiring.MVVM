namespace Inspiring.Mvvm.ViewModels.Core {
   using System;

   internal sealed class CollectionPopulatorBehavior<TItemVM> :
      Behavior,
      IValueAccessorBehavior<IVMCollection<TItemVM>>,
      IMutabilityCheckerBehavior
      where TItemVM : IViewModel {

      public IVMCollection<TItemVM> GetValue(IBehaviorContext context, ValueStage stage) {
         var coll = CreateCollection(context);
         Repopulate(context, coll);
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

      public void Repopulate(IBehaviorContext context, IVMCollection<TItemVM> collection) {
         var behavior = collection
            .Behaviors
            .GetNextBehavior<IPopulatorCollectionBehavior<TItemVM>>();

         behavior.Repopulate(context, collection);
      }

      private IVMCollection<TItemVM> CreateCollection(IBehaviorContext context) {
         return this.GetValueNext<IVMCollection<TItemVM>>(context, ValueStage.PostValidation);
      }
   }
}
