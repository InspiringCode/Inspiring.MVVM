namespace Inspiring.Mvvm.ViewModels.Core.Validation.Validators {
   using System.Diagnostics.Contracts;
   using System.Linq;

   internal sealed class CompositeValidator : IValidator {
      private readonly IValidator[] _validators;

      public CompositeValidator(params IValidator[] validators) {
         Contract.Requires(validators != null);
         Contract.Requires(Contract.ForAll(validators, x => x != null));

         _validators = validators;
      }

      public CompositeValidator Add(IValidator validator) {
         Contract.Requires(validator != null);
         return new CompositeValidator(ArrayUtils.Append(_validators, validator));
      }

      public ValidationResult Execute(ValidationRequest request) {
         return ValidationResult.Join(
            _validators.Select(x => x.Execute(request))
         );
      }
   }
}
