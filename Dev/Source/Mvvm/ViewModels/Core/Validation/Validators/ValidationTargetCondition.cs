namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Diagnostics.Contracts;

   internal sealed class ValidationTargetCondition : ICondition<ValidationRequest> {
      private readonly PathDefinition _expectedTarget;

      public ValidationTargetCondition(PathDefinition expectedTarget) {
         Contract.Requires(expectedTarget != null);
         _expectedTarget = expectedTarget;
      }

      public bool IsTrue(ValidationRequest operand) {
         PathMatch match = _expectedTarget.Matches(operand.TargetPath);
         return match.Success;
      }

      public override string ToString() {
         return String.Format(
            "ValidationTarget matches {0}",
            _expectedTarget
         );
      }
   }
}
