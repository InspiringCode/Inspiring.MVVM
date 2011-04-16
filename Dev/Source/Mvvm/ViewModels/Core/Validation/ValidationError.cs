namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Diagnostics.Contracts;
   using Inspiring.Mvvm.Common;

   /// <summary>
   ///   A <see cref="ValidationError"/> is the result of a failed validation
   ///   performed by a <see cref="Validator"/>.
   /// </summary>
   public sealed class ValidationError {
      private IValidator _validator;

      public ValidationError(string message) {
         Contract.Requires<ArgumentNullException>(message != null);
         Message = message;
      }

      //public ValidationError(IViewModel target, IValidator validator, string message) {
      //   throw new NotImplementedException();
      //}

      //public ValidationError(IViewModel target, Validator validator, string message) {
      //   Contract.Requires<ArgumentNullException>(target != null);
      //   Contract.Requires<ArgumentNullException>(validator != null);
      //   Contract.Requires<ArgumentNullException>(message != null);
      //   Message = message;
      //   Validator = validator;
      //   Target = target;
      //}

      public ValidationError(
         IValidator validator,
         IViewModel target,
         string message,
         object details = null
      ) {
         // TODO: Enable after refactoring!
         // Contract.Requires(validator != null);
         Contract.Requires(target != null);
         Contract.Requires(message != null);

         _validator = validator;
         Target = target;
         Message = message;
         Details = details;
      }

      public ValidationError(
         IValidator validator,
         IViewModel target,
         IVMPropertyDescriptor targetProperty,
         string message,
         object details = null
      )
         : this(validator, target, message, details) {

         Contract.Requires(targetProperty != null);
         TargetProperty = targetProperty;
      }

      public IViewModel Target { get; private set; }

      public IVMPropertyDescriptor TargetProperty { get; private set; }

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
            Object.ReferenceEquals(Target, other.Target) &&
            Object.ReferenceEquals(TargetProperty, other.TargetProperty) &&
            Object.Equals(Message, other.Message) &&
            Object.Equals(Details, other.Details);
      }

      public override int GetHashCode() {
         return HashCodeService.CalculateHashCode(
            this,
            _validator,
            Target,
            TargetProperty,
            Message,
            Details
         );
      }

      public override string ToString() {
         return Message;
      }
   }
}
