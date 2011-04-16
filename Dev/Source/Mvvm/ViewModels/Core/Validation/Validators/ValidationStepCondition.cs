namespace Inspiring.Mvvm.ViewModels.Core {
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
         return String.Format("Step == {0}", _expectedStep);
      }
   }
}
