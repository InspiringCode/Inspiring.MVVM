namespace Inspiring.MvvmTest {
   using System;
   using System.Collections.Generic;
   using System.Linq;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Inspiring.MvvmTest.ViewModels;
   using Microsoft.VisualStudio.TestTools.UnitTesting;
   using ValidationArgs = Inspiring.Mvvm.ViewModels.Core.ValidationArgs;

   [TestClass]
   public class ValidationTestBase : TestBase {
      protected static ValidationResult CreateValidationResult(
         params string[] errors
      ) {
         return CreateValidationResult(target: null, errors: errors);
      }

      protected static ValidationResult CreateValidationResult(
         IViewModel target = null,
         params string[] errors
      ) {
         target = target ?? NullViewModel.Instance;

         IEnumerable<ValidationResult> states = errors
            .Select(error => new ValidationResult(new ValidationError(NullValidator.Instance, target, error)));

         return ValidationResult.Join(states);
      }

      protected static ValidationRequest CreateRequest(Path targetPath = null) {
         return new ValidationRequest(ValidationStep.Value, targetPath ?? Path.Empty);
      }


      protected static ViewModelStub CreateInvalidVM(ValidationResult validationResult) {
         var behavior = new ValidationResultAggregatorStub();
         behavior.ReturnedValidationResults[ValidationResultScope.All] = validationResult;

         return ViewModelStub
            .WithBehaviors(behavior)
            .Build();
      }

      protected static void AssertErrors(ValidationArgs args, params ValidationError[] errors) {
         var expected = new ValidationResult(errors);
         Assert.AreEqual(expected, args.Result);
      }

      private class NullValidator : IValidator {
         public static readonly NullValidator Instance = new NullValidator();

         public ValidationResult Execute(ValidationRequest request) {
            throw new NotSupportedException();
         }
      }

      private class NullViewModel : IViewModel {
         public static readonly NullViewModel Instance = new NullViewModel();

         public VMKernel Kernel { get; set; }

         public VMDescriptorBase Descriptor { get; set; }

         public IBehaviorContext GetContext() {
            throw new NotSupportedException();
         }

         public void NotifyChange(ChangeArgs args) {
         }
      }
   }
}