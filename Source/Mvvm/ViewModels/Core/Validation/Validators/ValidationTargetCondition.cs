namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   
   internal sealed class ValidationTargetCondition : ICondition<ValidationRequest> {
      private readonly PathDefinition _expectedTarget;

      public ValidationTargetCondition(PathDefinition expectedTarget) {
         Check.NotNull(expectedTarget, nameof(expectedTarget));
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
