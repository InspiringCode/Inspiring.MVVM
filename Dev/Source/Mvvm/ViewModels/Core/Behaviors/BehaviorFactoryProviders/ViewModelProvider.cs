﻿namespace Inspiring.Mvvm.ViewModels.Core.Behaviors {
   using Inspiring.Mvvm.Common;

   public class ViewModelProvider : IViewModelProvider {
      public virtual BehaviorFactory GetFactory<TVM>() where TVM : IViewModel {
         return new BehaviorFactory()
            .RegisterBehavior<LoadOrderBehavior>(ViewModelBehaviorKeys.LoadOrderController)
            .RegisterBehavior<ViewModelValidationSourceBehavior>(ViewModelBehaviorKeys.ViewModelValidationSource)
            .RegisterBehavior<ValidatorExecutorBehavior>(ViewModelBehaviorKeys.ValidationExecutor)
            .RegisterBehavior<TypeDescriptorBehavior>(ViewModelBehaviorKeys.TypeDescriptorProvider)
            .RegisterBehavior<UndoRootBehavior>(ViewModelBehaviorKeys.UndoRoot);
      }
   }
}
