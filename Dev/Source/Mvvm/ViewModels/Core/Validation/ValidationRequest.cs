namespace Inspiring.Mvvm.ViewModels.Core {
   using System.Diagnostics.Contracts;

   public sealed class ValidationRequest {
      public ValidationRequest(ValidationStep step, Path validationTarget) {
         Contract.Requires(validationTarget != null);

         Step = step;
         ValidationTarget = validationTarget;
      }

      public ValidationStep Step { get; private set; }

      public Path ValidationTarget { get; private set; }
   }
}
