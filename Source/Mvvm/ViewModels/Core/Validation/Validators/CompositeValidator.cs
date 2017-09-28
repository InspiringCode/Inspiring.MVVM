namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Collections.Generic;
   using System.Linq;

   internal sealed class CompositeValidator : IValidator {
      private readonly IValidator[] _validators;

      public CompositeValidator(params IValidator[] validators) {
         Check.NotNull(validators, nameof(validators));
         Check.Requires(validators.All(x => x != null));

         _validators = validators;
      }

      public CompositeValidator Add(IValidator validator) {
         Check.NotNull(validator, nameof(validator));
         return new CompositeValidator(ArrayUtils.Append(_validators, validator));
      }

      public ValidationResult Execute(ValidationRequest request) {
         return ValidationResult.Join(
            _validators.Select(x => x.Execute(request))
         );
      }

      public override string ToString() {
         IEnumerable<IValidator> validators = _validators;

         return String.Format(
            "Composite({0})",
            String.Join(", ", validators)
         );
      }
   }
}
