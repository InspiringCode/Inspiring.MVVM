namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using Inspiring.Mvvm.Common;

   /// <summary>
   ///   A <see cref="ValidationError"/> is the result of a failed validation
   ///   performed by an <see cref="IValidator"/>.
   /// </summary>
   public sealed class ValidationError {
      private IValidator _validator;

      public ValidationError(
         IValidator validator,
         IValidationErrorTarget target,
         string message,
         object details = null
      ) {
         Check.NotNull(validator, nameof(validator));
         Check.NotNull(target, nameof(target));
         Check.NotNull(message, nameof(message));

         _validator = validator;

         Target = target;
         Message = message;
         Details = details;
      }

      public IValidationErrorTarget Target { get; private set; }

      /// <summary>
      ///   The error message that should be displayed to the user.
      /// </summary>
      public string Message { get; private set; }

      public object Details { get; private set; }

      /// <summary>
      ///   Two <see cref="ValidationError"/>s are equal if their <see cref="Message"/>
      ///   is equal.
      /// </summary>
      public override bool Equals(object obj) {
         var other = obj as ValidationError;
         return
            other != null &&
            Object.ReferenceEquals(_validator, other._validator) &&
            Object.Equals(Target, other.Target) &&
            Object.Equals(Message, other.Message) &&
            Object.Equals(Details, other.Details);
      }

      public override int GetHashCode() {
         return HashCodeService.CalculateHashCode(
            this,
            _validator,
            Target,
            Message,
            Details
         );
      }

      public override string ToString() {
         return Message;
      }

      internal bool OriginatedFrom(ICollectionValidationTarget target) {
         Check.NotNull(target, nameof(target));

         return
            Target.Step == target.Step &&
            Target.Collection == target.Collection &&
            Target.Property == target.Property;
      }
   }
}
