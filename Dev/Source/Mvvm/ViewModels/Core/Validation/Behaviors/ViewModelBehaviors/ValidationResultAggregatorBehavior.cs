namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Linq;

   internal sealed class ValidationResultAggregatorBehavior :
      Behavior,
      IBehaviorInitializationBehavior,
      IValidationResultAggregatorBehavior {

      private VMDescriptorBase _descriptor;

      public void Initialize(BehaviorInitializationContext context) {
         _descriptor = context.Descriptor;
         this.InitializeNext(context);
      }

      public ValidationResult GetValidationResult(IBehaviorContext context, ValidationResultScope scope) {
         ValidationResult result = GetValidationResultCore(context, scope);
         ValidationResult nextResult = this.GetValidationResultNext(context, scope);

         return ValidationResult.Join(result, nextResult);
      }

      private ValidationResult GetValidationResultCore(
         IBehaviorContext context,
         ValidationResultScope scope
       ) {
         switch (scope) {
            case ValidationResultScope.All:
               return ValidationResult.Join(
                  GetValidationResultCore(context, ValidationResultScope.Self),
                  GetValidationResultCore(context, ValidationResultScope.Descendants)
               );
            case ValidationResultScope.Self:
               return ValidationResult.Join(
                  GetValidationResultCore(context, ValidationResultScope.PropertiesOnly),
                  GetValidationResultCore(context, ValidationResultScope.ViewModelValidationsOnly)
               );
            case ValidationResultScope.Descendants:
               return ValidationResult.Join(
                  _descriptor.Properties.Select(x => x
                     .Behaviors
                     .GetDescendantsValidationResultNext(context))
               );
            case ValidationResultScope.ViewModelValidationsOnly:
               return _descriptor
                  .Behaviors
                  .GetValidationResultNext(context);
            case ValidationResultScope.PropertiesOnly:
               return ValidationResult.Join(
                  _descriptor.Properties.Select(x => x
                     .Behaviors
                     .GetValidationResultNext(context))
               );
            default:
               throw new NotSupportedException();
         }
      }
   }
}
