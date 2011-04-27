﻿namespace Inspiring.Mvvm.ViewModels.Core {
   using System;

   internal sealed class ViewModelValidationSourceBehavior :
      InitializableBehavior,
      IBehaviorInitializationBehavior,
      IChangeHandlerBehavior,
      IValidationResultProviderBehavior,
      IPropertyRevalidationBehavior, // TODO: Replace with IViewModelRevalidationBehavior.
      IRefreshControllerBehavior {

      private ValidationResultManager _resultManager;

      public void Initialize(BehaviorInitializationContext context) {
         _resultManager = new ValidationResultManager(context, ViewModel.GeneralFieldGroup);
         SetInitialized();

         this.InitializeNext(context);
      }

      public void Revalidate(IBehaviorContext context, CollectionResultCache cache) {
         RequireInitialized();

         var result = ValidationOperation.PerformViewModelValidation(cache, context.VM);
         _resultManager.UpdateValidationResult(context, result);

         this.PropertyRevalidateNext(context, cache);
      }

      public void Refresh(IBehaviorContext context) {
         this.ViewModelRefreshNext(context);
         Revalidator.RevalidateViewModelValidations(context.VM);
      }

      public void Refresh(IBehaviorContext context, IVMPropertyDescriptor property) {
         this.ViewModelRefreshNext(context, property);
      }

      public ValidationResult GetValidationResult(IBehaviorContext context) {
         RequireInitialized();

         return ValidationResult.Join(
            _resultManager.GetValidationResult(context),
            this.GetValidationResultNext(context)
         );
      }

      public void HandleChange(IBehaviorContext context, ChangeArgs args) {
         this.HandleChangedNext(context, args);
         Revalidator.RevalidateViewModelValidations(context.VM);
      }

      public void BeginValidation(IBehaviorContext context, ValidationController controller) {
         throw new NotImplementedException(); // TODO: Remove this implementation
      }

      public void EndValidation(IBehaviorContext context) {
         throw new NotImplementedException(); // TODO: Remove this implementation
      }
   }
}