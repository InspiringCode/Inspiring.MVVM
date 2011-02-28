namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Collections.ObjectModel;
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
      public static readonly ValidationState Valid = new ValidationState(ValidationErrorCollection.Empty);

      private ValidationErrorCollection _errors;

      public ValidationState()
         : this(new ValidationErrorCollection()) {
      }

      private ValidationState(ValidationErrorCollection errors) {
         _errors = errors;
      }

      /// <summary>
      ///   Gets the validation errors that were added by the validators of the
      ///   property or view model.
      /// </summary>
      public ValidationErrorCollection Errors {
         get {
            Contract.Ensures(Contract.Result<ValidationErrorCollection>() != null);
            return _errors;
         }
      }

      /// <summary>
      ///   Returns true if all validators have succeeded.
      /// </summary>
      public bool IsValid {
         get {
            return Errors.Count == 0;
         }
      }

      /// <summary>
      ///   Creates a new <see cref="ValidationState"/> that contains the errors 
      ///   of all passed in states.
      /// </summary>
      public static ValidationState Join(params ValidationState[] states) {
         var state = new ValidationState();

         states
            .SelectMany(x => x.Errors)
            .ForEach(state.Errors.Add);

         return state;
      }

      //internal void AddValidationError(Validator validator, ValidationError error) {

      //}


      //internal void RemoveValidationErrors(Validator validator) {

      //}


      public ReadOnlyCollection<ValidationError> Errors_ {
         get;
         set;
      }

      /// <summary>
      ///   Two <see cref="ValidationState"/>s are equal, if there <see 
      ///   cref="ValidationErrorCollection"/>s are equal.
      /// </summary>
      public override bool Equals(object obj) {
         var other = obj as ValidationState;
         return other != null && other.Errors.Equals(Errors);
      }

      /// <inheritdoc />
      public override int GetHashCode() {
         return HashCodeService.CalculateHashCode(this, Errors);
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
