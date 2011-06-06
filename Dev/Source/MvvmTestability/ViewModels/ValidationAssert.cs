namespace Inspiring.Mvvm.ViewModels {
   using System;
   using System.Collections.Generic;
   using System.Linq;
   using Inspiring.Mvvm.Testability;
   using Inspiring.Mvvm.ViewModels.Core;

   public static class ValidationAssert {
      public static readonly IEqualityComparer<ValidationError> EssentialErrorComparer = new EssentialValidationErrorComparer();
      public static readonly IEqualityComparer<ValidationError> FullErrorComparer = new FullValidationErrorComparer();

      public static void Errors(params ValidationError[] expectedErrors) {
         var errorsPerVM = expectedErrors.GroupBy(x => x.Target.VM);

         foreach (var errorGroup in errorsPerVM) {
            var expectedResult = new ValidationResult(errorGroup);
            HasResult(expectedResult, errorGroup.Key);
         }
      }

      public static void ErrorMessages(ValidationResult actualResult, params string[] expectedMessages) {
         var orderedExpected = expectedMessages.OrderBy(x => x);

         var orderedActual = actualResult
            .Errors
            .Select(x => x.Message)
            .OrderBy(x => x);

         if (!orderedExpected.SequenceEqual(orderedActual)) {
            TestFrameworkAdapter.Current.Fail(
               String.Format(
                  "Expected error messages [{0}] but got [{1}].",
                  String.Join(", ", orderedExpected),
                  String.Join(", ", orderedActual)
               )
            );
         }
      }

      public static void HasValidationResult(IViewModel vm, ValidationError singleExpectedError) {
         HasErrors(vm.Kernel.GetValidationResult(ValidationResultScope.All), singleExpectedError);
      }

      public static void HasPropertyValidationResult(IViewModel vm, ValidationError singleExpectedError, IVMPropertyDescriptor property) {
         HasErrors(vm.Kernel.GetValidationResult(property), singleExpectedError);
      }

      public static void HasViewModelValidationResult(IViewModel vm, ValidationError singleExpectedError) {
         HasErrors(vm.Kernel.GetValidationResult(ValidationResultScope.ViewModelValidationsOnly), singleExpectedError);
      }

      public static void HasErrors(ValidationResult actualResult, params ValidationError[] expectedErrors) {
         HasErrors(actualResult, EssentialErrorComparer, expectedErrors);
      }

      public static void HasErrors(
         ValidationResult actualResult,
         IEqualityComparer<ValidationError> comparer,
         params ValidationError[] expectedErrors
      ) {
         var expectedResult = new ValidationResult(expectedErrors);
         AreEqual(expectedResult, actualResult, comparer);
      }

      public static void AreEqual(ValidationResult expectedResult, ValidationResult actualResult, IEqualityComparer<ValidationError> comparer = null) {
         comparer = comparer ?? EssentialErrorComparer;

         bool errorsEqual = expectedResult.Errors.SequenceEqual(
            actualResult.Errors,
            comparer
         );

         if (!errorsEqual) {
            TestFrameworkAdapter.Current.Fail(
               String.Format(
                  "Expected validaton result {0} but was {1}.",
                  expectedResult,
                  actualResult
               )
            );
         }
      }

      private static void HasResult(ValidationResult expectedResult, IViewModel vm) {
         var actualResult = vm.Kernel.GetValidationResult(ValidationResultScope.Self);

         bool errorsEqual = expectedResult.Errors.SequenceEqual(
            actualResult.Errors,
            EssentialErrorComparer
         );

         if (!errorsEqual) {
            TestFrameworkAdapter.Current.Fail(
               String.Format(
                  "Expected validaton result {0} for {1} but was {2}.",
                  expectedResult,
                  vm,
                  actualResult
               )
            );
         }
      }

      private class EssentialValidationErrorComparer : IEqualityComparer<ValidationError> {
         public bool Equals(ValidationError x, ValidationError y) {
            return
               x.Details == y.Details &&
               x.Message == y.Message &&
               x.Target.VM == y.Target.VM &&
               x.Target.Property == y.Target.Property;
         }

         public int GetHashCode(ValidationError obj) {
            return HashCodeService.CalculateHashCode(
               obj,
               obj.Details,
               obj.Message,
               obj.Target.VM,
               obj.Target.Property
            );
         }
      }

      private class FullValidationErrorComparer : IEqualityComparer<ValidationError> {
         public bool Equals(ValidationError x, ValidationError y) {
            return
               x.Details == y.Details &&
               x.Message == y.Message &&
               Object.Equals(x.Target, y.Target);
         }

         public int GetHashCode(ValidationError obj) {
            return HashCodeService.CalculateHashCode(
               obj,
               obj.Details,
               obj.Message,
               obj.Target
            );
         }
      }

      public static void IsValid(IViewModel vm) {
         if (!vm.Kernel.IsValid) {
            TestFrameworkAdapter.Current.Fail(
               String.Format(
                  "Expected {0} to be valid but returned result {1}.",
                  vm,
                  vm.Kernel.GetValidationResult()
               )
            );
         }
      }

      public static void IsValid(ValidationResult result) {
         if (!result.IsValid) {
            TestFrameworkAdapter.Current.Fail(
               String.Format(
                  "Expected result to be valid but was {0}.",
                  result
               )
            );
         }
      }

      public static void IsValid<TDescriptor>(
         IViewModel<TDescriptor> vm,
         Func<TDescriptor, IVMPropertyDescriptor> propertySelector
      ) where TDescriptor : IVMDescriptor {
         var property = propertySelector((TDescriptor)vm.Descriptor);
         IsValid(vm, property);
      }

      public static void IsValid(IViewModel vm, IVMPropertyDescriptor property) {
         var propertyResult = vm.Kernel.GetValidationResult(property);

         if (!propertyResult.IsValid) {
            TestFrameworkAdapter.Current.Fail(
               String.Format(
                  "Expected {0}.{1} to be valid but returned result {2}.",
                  vm,
                  property,
                  propertyResult
               )
            );
         }
      }

      public static void ValidViewModelValidationResultIsValid(IViewModel vm) {
         var result = vm.Kernel.GetValidationResult(ValidationResultScope.ViewModelValidationsOnly);

         if (!result.IsValid) {
            TestFrameworkAdapter.Current.Fail(
               String.Format(
                  "Expected view model validations of {0} to be valid but returned result {1}.",
                  vm,
                  result
               )
            );
         }
      }
   }
}
