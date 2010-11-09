namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Diagnostics.Contracts;

   public sealed class ValidationError {
      public ValidationError(string message) {
         Contract.Requires<ArgumentNullException>(message != null);
         Message = message;
      }

      public string Message { get; private set; }
   }
}
