namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   
   internal sealed class ConditionalValidator : IValidator {
      private readonly ICondition<ValidationRequest> _condition;
      private readonly IValidator _inner;

      public ConditionalValidator(
         ICondition<ValidationRequest> condition,
         IValidator inner
      ) {
         Check.NotNull(condition, nameof(condition));
         Check.NotNull(inner, nameof(inner));

         _condition = condition;
         _inner = inner;
      }

      public ValidationResult Execute(ValidationRequest request) {
         return _condition.IsTrue(request) ?
            _inner.Execute(request) :
            ValidationResult.Valid;
      }

      public override string ToString() {
         return String.Format(
            "if ({0}) then ({1})",
            _condition,
            _inner
         );
      }
   }
}
