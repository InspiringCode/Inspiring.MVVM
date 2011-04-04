namespace Inspiring.Mvvm.ViewModels.Core.GeneralBehaviors.ViewModelProperty {
   using System;
   using System.Collections.Generic;
   using System.Linq;
   using Inspiring.Mvvm.ViewModels.Core.Validation;

   internal sealed class ViewModelPropertyRefreshBehavior<TValue> :
      Behavior,
      IRefreshBehavior
      where TValue : IViewModel {

      public void Refresh(IBehaviorContext context) {
         if (this.IsLoadedNext(context)) {

            // RefreshNext
            // Revalidate

            //new HierarchicalRevalidator(null, ValidationScope.FullSubtree).Revalidate();
         }
         throw new NotImplementedException();
      }
   }
}
