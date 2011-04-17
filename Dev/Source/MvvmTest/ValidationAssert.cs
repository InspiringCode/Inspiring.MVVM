namespace Inspiring.MvvmTest {
   using System.Collections.Generic;
   using System.Linq;
   using Inspiring.Mvvm.Common;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   internal static class ValidationAssert {
      public static void Errors(params ValidationError[] expectedErrors) {
         var errorsPerVM = expectedErrors.GroupBy(x => x.Target);

         foreach (var errorGroup in errorsPerVM) {
            var expectedResult = new ValidationResult(errorGroup);
            HasResult(expectedResult, errorGroup.Key);
         }
      }

      public static void HasValidationResult(IViewModel vm, ValidationError singleExpectedError) {
         AreEqual(singleExpectedError, vm.Kernel.GetValidationState(ValidationResultScope.All));
      }

      public static void HasPropertyValidationResult(IViewModel vm, ValidationError singleExpectedError, IVMPropertyDescriptor property) {
         AreEqual(singleExpectedError, vm.Kernel.GetValidationState(property));
      }

      public static void HasViewModelValidationResult(IViewModel vm, ValidationError singleExpectedError) {
         AreEqual(singleExpectedError, vm.Kernel.GetValidationState(ValidationResultScope.ViewModelValidationsOnly));
      }

      public static void AreEqual(ValidationError singleExpectedError, ValidationResult actualResult) {
         AreEqual(new ValidationResult(singleExpectedError), actualResult);
      }

      public static void AreEqual(ValidationResult expectedResult, ValidationResult actualResult) {
         bool errorsEqual = expectedResult.Errors.SequenceEqual(
            expectedResult.Errors,
            ValidationErrorComparer.Default
         );

         if (!errorsEqual) {
            Assert.Fail("Expected validaton result {0} but was {1}.", expectedResult, actualResult);
         }
      }

      private static void HasResult(ValidationResult expectedResult, IViewModel vm) {
         var actualResult = vm.Kernel.GetValidationState(ValidationResultScope.Self);

         bool errorsEqual = expectedResult.Errors.SequenceEqual(
            actualResult.Errors,
            ValidationErrorComparer.Default
         );

         if (!errorsEqual) {
            Assert.Fail("Expected validaton result {0} for {1} but was {2}.", expectedResult, vm, actualResult);
         }
      }

      private class ValidationErrorComparer : IEqualityComparer<ValidationError> {
         public static readonly ValidationErrorComparer Default = new ValidationErrorComparer();

         public bool Equals(ValidationError x, ValidationError y) {
            return
               x.Message == y.Message &&
               x.Target == y.Target &&
               x.TargetProperty == y.TargetProperty;
         }

         public int GetHashCode(ValidationError obj) {
            return HashCodeService.CalculateHashCode(obj, obj.Message, obj.Target, obj.TargetProperty);
         }
      }


      public static void IsValid(IViewModel vm) {
         if (!vm.Kernel.IsValid) {
            Assert.Fail(
               "Expected {0} to be valid but returned result {1}.",
               vm,
               vm.Kernel.GetValidationState()
            );
         }
      }
   }
}
