namespace Inspiring.MvvmTest {
   using System;
   using Inspiring.Mvvm.ViewModels.Core;

   internal class NullValidator : IValidator {
      public static readonly NullValidator Instance = new NullValidator();

      public ValidationResult Execute(ValidationRequest request) {
         throw new NotSupportedException();
      }
   }
}
