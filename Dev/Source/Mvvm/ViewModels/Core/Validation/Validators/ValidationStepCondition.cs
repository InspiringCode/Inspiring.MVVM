namespace Inspiring.Mvvm.ViewModels.Core.Validation.Validators {
   using System;

   internal sealed class ValidationStepCondition : ICondition<ValidationRequest> {
      private readonly ValidationStep _expectedStep;

      public ValidationStepCondition(ValidationStep expectedStep) {
         _expectedStep = expectedStep;
      }

      public bool IsTrue(ValidationRequest operand) {
         return operand.Step == _expectedStep;
      }

      public override string ToString() {
         return String.Format("ValidationRequest.Step == {0}", _expectedStep);
      }
   }
}
