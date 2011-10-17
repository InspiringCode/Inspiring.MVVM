﻿namespace Inspiring.Mvvm.ViewModels.Core {
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

      public void Refresh(IBehaviorContext context, bool executeRefreshDependencies) {
         var collection = GetValue(context);

         foreach (TItemVM item in collection) {
            item.Kernel.RefreshWithoutValidation();
         }

         this.RefreshNext(context, executeRefreshDependencies);
      }

      protected override IVMCollection<TItemVM> ProvideValue(IBehaviorContext context) {
         return this.CreateValueNext<IVMCollection<TItemVM>>(context);
      }
   }
}