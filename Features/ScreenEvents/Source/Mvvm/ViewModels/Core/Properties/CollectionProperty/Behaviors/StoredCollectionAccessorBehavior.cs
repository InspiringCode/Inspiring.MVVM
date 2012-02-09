namespace Inspiring.Mvvm.ViewModels.Core {
   using System;

   internal sealed class StoredCollectionAccessorBehavior<TItemVM> :
      CachedAccessorBehavior<IVMCollection<TItemVM>>,
      IRefreshBehavior
      where TItemVM : IViewModel {

      public override void SetValue(IBehaviorContext context, IVMCollection<TItemVM> value) {
         throw new NotSupportedException(
            ExceptionTexts.CannotSetVMCollectionProperties
         );
      }

      public void Refresh(IBehaviorContext context, RefreshOptions options) {
         var collection = GetValue(context);

         foreach (TItemVM item in collection) {
            item.Kernel.RefreshWithoutValidation(options.ExecuteRefreshDependencies);
         }

         this.RefreshNext(context, options);
      }

      protected override IVMCollection<TItemVM> ProvideValue(IBehaviorContext context) {
         return this.CreateValueNext<IVMCollection<TItemVM>>(context);
      }
   }
}
