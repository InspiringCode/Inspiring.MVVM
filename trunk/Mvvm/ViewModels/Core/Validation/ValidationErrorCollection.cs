namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Collections;
   using System.Collections.Generic;
   using System.Diagnostics.Contracts;
   using Inspiring.Mvvm.Common;

   /// <summary>
   ///   Holds a list of <see cref="ValidationError"/>s that were caused either
   ///   by a <see cref="ViewModelValidator"/> or a <see cref="PropertyValidator"/>.
   /// </summary>
   public sealed class ValidationErrorCollection : IEnumerable<ValidationError> {
      /// <summary>
      ///   Provides a default empty instance that can be used to minimize memory
      ///   consumption. This instance is readonly and no errors can be added.
      /// </summary>
      public static readonly ValidationErrorCollection Empty = new ValidationErrorCollection();

      private List<ValidationError> _errors = new List<ValidationError>();

      public int Count {
         get { return _errors.Count; }
      }

      public void Add(ValidationError error) {
         Contract.Requires<ArgumentNullException>(error != null);
         Contract.Requires<ArgumentException>(
            this != Empty,
            "The default 'ValidationErrorCollection' cannot be modified."
         );

         _errors.Add(error);
      }

      /// <summary>
      ///   Two <see cref="ValidationErrorCollection"/>s are equal, if their 
      ///   <see cref="ValidationError"/>s are equal and were added in the same
      ///   order.
      /// </summary>
      public override bool Equals(object obj) {
         var other = obj as ValidationErrorCollection;
         if (other == null || other.Count != Count) {
            return false;
         }

         for (int i = 0; i < Count; i++) {
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

      /// <inheritdoc />
      public override string ToString() {
         return String.Format(
            "[{0}]",
            String.Join(", ", this)
         );
      }

      /// <inheritdoc />
      public IEnumerator<ValidationError> GetEnumerator() {
         return _errors.GetEnumerator();
      }

      /// <inheritdoc />
      IEnumerator IEnumerable.GetEnumerator() {
         return _errors.GetEnumerator();
      }
   }
}
