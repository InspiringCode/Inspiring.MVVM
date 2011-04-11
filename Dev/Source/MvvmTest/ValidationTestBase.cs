﻿namespace Inspiring.MvvmTest {
   using System.Collections.Generic;
   using System.Linq;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Inspiring.MvvmTest.ViewModels;
   using Microsoft.VisualStudio.TestTools.UnitTesting;
   using ValidationArgs = Inspiring.Mvvm.ViewModels.Core.Validation.Validators.ValidationArgs;

   [TestClass]
   public class ValidationTestBase : TestBase {
      protected static ValidationResult CreateValidationResult(params string[] errors) {
         IEnumerable<ValidationResult> states = errors
            .Select(error => new ValidationResult(new ValidationError(error)));

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
   }
}