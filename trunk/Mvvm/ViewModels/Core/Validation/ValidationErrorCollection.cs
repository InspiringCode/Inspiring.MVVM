namespace Inspiring.Mvvm.ViewModels.Core {
   using System.Collections;
   using System.Collections.Generic;
   using System.Diagnostics.Contracts;

   /// <summary>
   ///   Holds a list of <see cref="ValidationError"/>s that were caused either
   ///   by a <see cref="ViewModelValidator"/> or a <see cref="PropertyValidator"/>.
   /// </summary>
   internal sealed class ValidationErrorCollection : IEnumerable<ValidationError> {
      private List<ValidationError> _errors = new List<ValidationError>();

      public void Add(ValidationError error) {
         Contract.Requires(error != null);
         _errors.Add(error);
      }

      public IEnumerator<ValidationError> GetEnumerator() {
         return _errors.GetEnumerator();
      }

      IEnumerator IEnumerable.GetEnumerator() {
         return _errors.GetEnumerator();
      }
   }
}
