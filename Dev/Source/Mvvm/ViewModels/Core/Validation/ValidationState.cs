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
   public sealed class ValidationState {
      /// <summary>
      ///   Provides a sharable default instance for a valid ValidationState. This
      ///   instance is readonly.
      /// </summary>
      public static readonly ValidationState Valid = new ValidationState(new List<ValidationError>());

      private readonly List<ValidationError> _errors;

      public ValidationState()
         : this(new List<ValidationError>()) {
      }

      private ValidationState(List<ValidationError> errors) {
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
         get { return _errors.Count == 0; }
      }

      /// <summary>
      ///   Creates a new <see cref="ValidationState"/> that contains the errors 
      ///   of all passed in states.
      /// </summary>
      public static ValidationState Join(params ValidationState[] states) {
         var state = new ValidationState();

         states
            .SelectMany(x => x.Errors)
            .ForEach(state.AddError);

         return state;
      }

      internal void AddError(ValidationError error) {
         Contract.Requires(error != null);
         //Contract.Requires<ArgumentException>(
         //   this != Empty,
         //   "The default 'ValidationErrorCollection' cannot be modified."
         //);
         _errors.Add(error);
      }

      [Obsolete]
      internal void AddError(string error) {
         Contract.Requires(error != null);
         //Contract.Requires<ArgumentException>(
         //   this != Empty,
         //   "The default 'ValidationErrorCollection' cannot be modified."
         //);
         _errors.Add(new ValidationError(error));
      }

      internal void RemoveErrorsOf(Validator validator) {
         Contract.Requires(validator != null);
         _errors.RemoveAll(x => x.Validator == validator);
      }

      /// <summary>
      ///   Two <see cref="ValidationState"/>s are equal, if there <see 
      ///   cref="ValidationErrorCollection"/>s are equal.
      /// </summary>
      public override bool Equals(object obj) {
         var other = obj as ValidationState;

         if (other == null || other._errors.Count != _errors.Count) {
            return false;
         }

         for (int i = 0; i < _errors.Count; i++) {
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
            "IsValid = {0}, Errors={1}",
            IsValid,
            Errors
         );
      }
   }
}
