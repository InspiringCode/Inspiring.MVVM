namespace Inspiring.MvvmTest {
   using System;
   using System.Collections.Generic;
   using System.Linq;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Inspiring.MvvmTest.ViewModels;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class ValidationTestBase : TestBase {
      protected static ValidationError CreateValidationError(
         string message = "Validation error",
         IViewModel targetVM = null,
         IVMPropertyDescriptor targetProperty = null
      ) {
         targetVM = targetVM ?? NullViewModel.Instance;
         var step = targetProperty != null ?
            ValidationStep.Value :
            ValidationStep.ViewModel;

         return new ValidationError(
            NullValidator.Instance,
            ValidationTarget.ForError(step, targetVM, null, targetProperty),
            message
         );
      }

      protected static ValidationResult CreateValidationResult(
         params string[] errors
      ) {
         return CreateValidationResult(NullViewModel.Instance, null, errors: errors);
      }

      protected static ValidationResult CreateValidationResult(
         IViewModel target,
         IVMPropertyDescriptor targetProperty,
         params string[] errors
      ) {
         target = target ?? NullViewModel.Instance;

         IEnumerable<ValidationResult> states = errors
            .Select(error => new ValidationResult(CreateValidationError(error, target, targetProperty)));

         return ValidationResult.Join(states);
      }

      protected static ValidationResult CreateValidationResult(
         IViewModel target,
         params string[] errors
      ) {
         target = target ?? NullViewModel.Instance;

         IEnumerable<ValidationResult> states = errors
            .Select(error => new ValidationResult(CreateValidationError(error, target, null)));

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

      protected static ValidationErrorBuilder Error(string errorMessage = "Error") {
         return new ValidationErrorBuilder(errorMessage);
      }

      protected class ValidationErrorBuilder {
         private string _errorMessage;

         public ValidationErrorBuilder(string errorMessage) {
            _errorMessage = errorMessage;
         }

         public ValidationError For(IViewModel targetVM) {
            var target = ValidationTarget.ForError(
               ValidationStep.ViewModel,
               targetVM,
               null,
               null
            );

            return new ValidationError(NullValidator.Instance, target, _errorMessage);
         }

         public ValidationError For<T>(
            IViewModel<T> targetVM,
            Func<T, IVMPropertyDescriptor> targetPropertySelector
         ) where T : IVMDescriptor {
            var targetProperty = targetPropertySelector((T)targetVM.Descriptor);

            var target = ValidationTarget.ForError(
               ValidationStep.ViewModel,
               targetVM,
               null,
               targetProperty
            );

            return new ValidationError(NullValidator.Instance, target, _errorMessage);
         }

         public ValidationError Anonymous() {
            return For(NullViewModel.Instance);
         }
      }

      private class NullViewModel : IViewModel {
         public static readonly NullViewModel Instance = new NullViewModel();

         public VMKernel Kernel { get; set; }

         public IVMDescriptor Descriptor { get; set; }

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