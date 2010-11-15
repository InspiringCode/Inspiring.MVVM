namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Diagnostics.Contracts;

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

      public override string ToString() {
         return String.Format(
            "IsValid = {0}, Errors={1}",
            IsValid,
            Errors
         );
      }
   }
}
