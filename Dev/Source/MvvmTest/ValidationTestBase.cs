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
      protected static ValidationError CreateValidationError(
         string message = "Validation error",
         IViewModel target = null,
         IVMPropertyDescriptor targetProperty = null
      ) {
         target = target ?? NullViewModel.Instance;
         return targetProperty != null ?
            new ValidationError(NullValidator.Instance, target, targetProperty, message) :
            new ValidationError(NullValidator.Instance, target, message);
      }

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

      protected static ValidationErrorBuilder Error(string errorMessage = "Error") {
         return new ValidationErrorBuilder(errorMessage);
      }

      protected class ValidationErrorBuilder {
         private string _errorMessage;

         public ValidationErrorBuilder(string errorMessage) {
            _errorMessage = errorMessage;
         }

         public ValidationError For(IViewModel target) {
            return new ValidationError(NullValidator.Instance, target, _errorMessage);
         }

         public ValidationError For<T>(
            IViewModel<T> target,
            Func<T, IVMPropertyDescriptor> targetPropertySelector
         ) where T : VMDescriptorBase {
            var targetProperty = targetPropertySelector((T)target.Descriptor);
            return new ValidationError(NullValidator.Instance, target, targetProperty, _errorMessage);
         }

         public ValidationError Anonymous() {
            return For(NullViewModel.Instance);
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

      protected class ValidationExecutorStub : Behavior, IValidationExecutorBehavior {
         public ValidationExecutorStub() {
            Requests = new List<ValidationRequest>();
            ResultToReturn = ValidationResult.Valid;
         }

         public ValidationResult ResultToReturn { get; set; }

         public List<ValidationRequest> Requests { get; private set; }

         public bool WasCalled {
            get { return Requests.Any(); }
         }

         public ValidationResult Validate(IBehaviorContext context, ValidationRequest request) {
            Requests.Add(request);
            return ResultToReturn;
         }
      }
   }
}