﻿namespace Inspiring.Mvvm.ViewModels.Core {

   public interface IRefreshBehavior : IBehavior {
      void Refresh(IBehaviorContext context, bool executeRefreshDependencies);
   }
}
