namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Diagnostics.Contracts;
   using Inspiring.Mvvm.Common;

   /// <summary>
   ///   A <see cref="ValidationError"/> is the result of a failed validation
   ///   performed by a <see cref="Validator"/>.
   /// </summary>
   public sealed class ValidationError {
      public ValidationError(string message) {
         Contract.Requires<ArgumentNullException>(message != null);
         Message = message;
      }

      public ValidationError(Validator validator, string message) {
         Contract.Requires<ArgumentNullException>(validator != null);
         Contract.Requires<ArgumentNullException>(message != null);
         Message = message;
         Validator = validator;
      }

      /// <summary>
      ///   The error message that should be displayed to the user.
      /// </summary>
      public string Message { get; private set; }

      internal Validator Validator { get; private set; }

      /// <summary>
      ///   Two <see cref="ValidationError"/>s are equal if their <see cref="Message"/>
      ///   is equal.
      /// </summary>
      public override bool Equals(object obj) {
         var other = obj as ValidationError;
         return other != null && other.Message == Message;
      }

      /// <inheritdoc />
      public override int GetHashCode() {
         return HashCodeService.CalculateHashCode(this, Message);
      }

      /// <inheritdoc />
      public override string ToString() {
         return Message;
      }
   }
}
