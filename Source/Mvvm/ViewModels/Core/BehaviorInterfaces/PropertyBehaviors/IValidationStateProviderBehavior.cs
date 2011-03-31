﻿namespace Inspiring.Mvvm.ViewModels.Core {

   public interface IValidationStateProviderBehavior {
      ValidationState GetValidationState(IBehaviorContext context);

      ValidationState GetDescendantsValidationState(IBehaviorContext context);
   }
}