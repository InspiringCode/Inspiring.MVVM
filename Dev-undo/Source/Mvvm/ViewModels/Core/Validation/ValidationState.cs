namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Collections.Generic;
   using System.Diagnostics.Contracts;
   using System.Linq;
   using Inspiring.Mvvm.Common;

   /// <summary>
   ///   The validation state is the result of a validation. It holds all validation
   ///   errors and the overall result (valid/invalid). Each property and the view 
   ///   model have a validation state.
   /// </summary>
   /// <remarks>
   ///   The <see cref="ValidationResult"/> is an immutable data structure. You can 
   ///   use <see cref="Join"/> to create states with more than one error.
   /// </remarks>
   public sealed class ValidationResult {
      /// <summary>
      ///   A sharable, valid default instance.
      /// </summary>
      public static readonly ValidationResult Valid = new ValidationResult(new ValidationError[0]);

      private readonly ValidationError[] _errors;

      public ValidationResult(ValidationError error)
         : this(new[] { error }) {
         Contract.Requires(error != null);
      }

      public ValidationResult(IEnumerable<ValidationError> errors)
         : this(errors.ToArray()) {
         Contract.Requires(errors != null);
      }

      private ValidationResult(ValidationError[] errors) {
         _errors = errors;
      }

      /// <summary>
      ///   Gets the validation errors that were added by the validators of the
      ///   property or view model.
      /// </summary>
      public IEnumerable<ValidationError> Errors {
         get { return _errors; }
      }

      /// <summary>
      ///   Returns true if all validators have succeeded.
      /// </summary>
      public bool IsValid {
         get { return _errors.Length == 0; }
      }

      /// <summary>
      ///   Creates a new <see cref="ValidationResult"/> that contains the errors 
      ///   of all passed in states.
      /// </summary>
      public static ValidationResult Join(IEnumerable<ValidationResult> states) {
         return states.Aggregate(seed: Valid, func: Join);
      }

      /// <summary>
      ///   Creates a new <see cref="ValidationResult"/> that contains the errors 
      ///   of the <paramref name="first"/> and <paramref name="second"/> state.
      /// </summary>
      public static ValidationResult Join(ValidationResult first, ValidationResult second) {
         if (first.IsValid) {
            return second;
         }

         if (second.IsValid) {
            return first;
         }

         var allErrors = ArrayUtils.Concat(first._errors, second._errors);
         return new ValidationResult(allErrors);
      }

      /// <summary>
      ///   Two <see cref="ValidationResult"/>s are equal, if there <see 
      ///   cref="ValidationErrorCollection"/>s are equal.
      /// </summary>
      public override bool Equals(object obj) {
         var other = obj as ValidationResult;

         if (other == null || other._errors.Length != _errors.Length) {
            return false;
         }

         for (int i = 0; i < _errors.Length; i++) {
            if (!other._errors[i].Equals(_errors[i])) {
               return false;
            }
         }

         return true;
      }

      /// <inheritdoc />
      public override int GetHashCode() {
         return HashCodeService.CalculateHashCode(
            this,
            HashCodeService.CalculateCollectionHashCode(_errors)
         );
      }

      public override string ToString() {
         return String.Format(
            "{{IsValid = {0}, Errors={1}}}",
            IsValid,
            String.Join(", ", Errors)
         );
      }
   }
}
