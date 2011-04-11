namespace Inspiring.MvvmTest.ViewModels.Core.Validation {
   using Inspiring.Mvvm;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Inspiring.Mvvm.ViewModels.Core.Validation.Validators;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class ValidatorExecutorBehaviorTests : ValidationTestBase {
      [TestMethod]
      public void Validate_CallsAddedValidatorWithCorrectArgs() {
         var vm = new ViewModelWithValidatorExecutor();

         var validator = CreateValidator();
         var request = CreateRequest();

         vm.Behavior.AddValidator(validator);

         vm.Behavior.Validate(vm.GetContext(), request);
         Assert.AreEqual(1, validator.Invocations, "Validator was not called.");
         DomainAssert.AreEqual(request, validator.LastRequest);
      }

      [TestMethod]
      public void Validate_CallsParentWithExtendedPath() {
         var parentVM = new ViewModelWithValidatorExecutor();
         var vm = new ViewModelWithValidatorExecutor(parents: parentVM);

         var validator = CreateValidator();
         parentVM.Behavior.AddValidator(validator);

         var originalPath = Path.Empty;
         vm.Behavior.Validate(vm.GetContext(), CreateRequest(originalPath));

         var expectedPath = originalPath.Prepend(parentVM);
         Assert.AreEqual(1, validator.Invocations, "Parent was not called.");
         DomainAssert.AreEqual(expectedPath, validator.LastRequest.TargetPath);
      }

      [TestMethod]
      public void Validate_WhenParentDoesNotHaveValidatorExecutor_CallsGrandparentWithExtendedPath() {
         var grandParentVM = new ViewModelWithValidatorExecutor();
         var parentVM = new ViewModelWithoutValidatorExecutor(parents: grandParentVM);
         var vm = new ViewModelWithValidatorExecutor(parents: parentVM);

         var validator = CreateValidator();
         grandParentVM.Behavior.AddValidator(validator);

         var originalPath = Path.Empty;
         vm.Behavior.Validate(vm.GetContext(), CreateRequest(originalPath));

         var expectedPath = originalPath.Prepend(parentVM).Prepend(grandParentVM);

         Assert.AreEqual(1, validator.Invocations, "Grandparent was not called.");
         DomainAssert.AreEqual(expectedPath, validator.LastRequest.TargetPath);
      }

      [TestMethod]
      public void Validate_WhenParentHasValidatorExecutor_CallsGrandparentWithExtendedPath() {
         var grandParentVM = new ViewModelWithValidatorExecutor();
         var parentVM = new ViewModelWithValidatorExecutor(parents: grandParentVM);
         var vm = new ViewModelWithValidatorExecutor(parents: parentVM);

         var validator = CreateValidator();
         grandParentVM.Behavior.AddValidator(validator);

         var originalPath = Path.Empty;
         vm.Behavior.Validate(vm.GetContext(), CreateRequest(originalPath));

         var expectedPath = originalPath.Prepend(parentVM).Prepend(grandParentVM);

         Assert.AreEqual(1, validator.Invocations, "Grandparent was not called.");
         DomainAssert.AreEqual(expectedPath, validator.LastRequest.TargetPath);
      }

      /// <remarks>
      ///   vm (*) ---> parent1 -------> grandParent1 (*)
      ///          |
      ///          +--> parent2 (*) ---> grandParent2
      ///                           |
      ///                           +--> grandParent3 (*)
      ///                       
      ///   (*) means with <see cref="ValidatorExecutorBehavior"/>
      /// </remarks>
      [TestMethod]
      public void Validate_CallsAllAncestorsAndReturnsJoinedResults() {
         var vmError = CreateValidationResult("vm error");
         var parent2Error = CreateValidationResult("parent2 error");
         var grandParent1Error = CreateValidationResult("grandParent1 error");
         var grandParent3Error = CreateValidationResult("grandParent3 error");


         var grandParent1 = CreateViewModelWithExecutorResult(grandParent1Error);
         var grandParent2 = CreateViewModelWithoutExecutor();
         var grandParent3 = CreateViewModelWithExecutorResult(grandParent3Error);
         var parent1 = CreateViewModelWithoutExecutor(grandParent1);
         var parent2 = CreateViewModelWithExecutorResult(parent2Error, grandParent2, grandParent3);
         var vm = CreateViewModelWithExecutorResult(vmError, parent1, parent2);

         var expectedResult = ValidationResult.Join(new[] {
            vmError,
            grandParent1Error,
            parent2Error,
            grandParent3Error
         });

         var actualResult = vm.Behavior.Validate(vm.GetContext(), CreateRequest());

         Assert.AreEqual(expectedResult, actualResult);
      }

      private ViewModelWithValidatorExecutor CreateViewModelWithExecutorResult(ValidationResult result, params IViewModel[] parents) {
         var vm = new ViewModelWithValidatorExecutor(parents);
         vm.Behavior.AddValidator(CreateValidator(result));
         return vm;
      }

      private ViewModelWithoutValidatorExecutor CreateViewModelWithoutExecutor(params IViewModel[] parents) {
         return new ViewModelWithoutValidatorExecutor(parents);
      }

      private ValidatorStub CreateValidator(ValidationResult resultToReturn = null) {
         return new ValidatorStub { ResultToReturn = resultToReturn ?? ValidationResult.Valid };
      }

      private class ViewModelWithValidatorExecutor : ViewModelStub {
         public ViewModelWithValidatorExecutor(params IViewModel[] parents)
            : this(new ValidatorExecutorBehavior()) {
            parents.ForEach(Kernel.Parents.Add);
         }

         private ViewModelWithValidatorExecutor(ValidatorExecutorBehavior behavior)
            : base(DescriptorStub.WithBehaviors(behavior).Build()) {
            Behavior = behavior;
         }

         public ValidatorExecutorBehavior Behavior { get; private set; }
      }

      private class ViewModelWithoutValidatorExecutor : ViewModelStub {
         public ViewModelWithoutValidatorExecutor(params IViewModel[] parents) {
            parents.ForEach(Kernel.Parents.Add);
         }
      }

      private class ValidatorStub : IValidator {
         public ValidatorStub() {
            ResultToReturn = ValidationResult.Valid;
         }

         public int Invocations { get; private set; }
         public ValidationResult ResultToReturn { get; set; }
         public ValidationRequest LastRequest { get; set; }

         public ValidationResult Execute(ValidationRequest request) {
            Invocations++;
            LastRequest = request;
            return ResultToReturn;
         }
      }
   }
}