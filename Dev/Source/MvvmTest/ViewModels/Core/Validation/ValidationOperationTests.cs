namespace Inspiring.MvvmTest.ViewModels.Core.Validation {
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class ValidationOperationTests : ValidationTestBase {
      private TestValidationExecutor Executor { get; set; }
      private TestCollectionResultCache CollectionCache { get; set; }
      private ViewModelStub ViewModel { get; set; }
      private PropertyStub<string> Property { get; set; }

      [TestInitialize]
      public void Setup() {
         Executor = new TestValidationExecutor();
         CollectionCache = new TestCollectionResultCache();
         Property = PropertyStub.Of<string>();

         ViewModel = ViewModelStub
            .WithBehaviors(Executor)
            .WithProperties(Property)
            .Build();
      }

      [TestMethod]
      public void PerformPropertyValidation_ReturnsErrorsOfPropertyAndCollectionValidators() {
         Executor.ResultToReturn = CreateValidationResult("Property error");
         CollectionCache.ResultToReturn = CreateValidationResult("Collection error");

         var expected = ValidationResult.Join(Executor.ResultToReturn, CollectionCache.ResultToReturn);
         var actual = PerformPropertyValidation();

         Assert.AreEqual(expected, actual);
      }

      [TestMethod]
      public void PerformPropertyValidation_InvokesCollectionResultCacheWithCorrectArguments() {
         var step = ValidationStep.Value;
         PerformPropertyValidation(step);

         Assert.AreEqual(ViewModel, CollectionCache.LastItem);
         Assert.AreEqual(step, CollectionCache.LastStep);
         Assert.AreEqual(Property, CollectionCache.LastProperty);

         step = ValidationStep.DisplayValue;
         PerformPropertyValidation(step);
         Assert.AreEqual(step, CollectionCache.LastStep);
      }

      [TestMethod]
      public void PerformPropertyValidation_InvokesValidationExecutorWithCorrectArguments() {
         var step = ValidationStep.Value;
         PerformPropertyValidation(step);

         Assert.AreEqual(ViewModel.GetContext(), Executor.LastContext);
         Assert.AreEqual(step, Executor.LastRequest.Step);

         step = ValidationStep.DisplayValue;
         PerformPropertyValidation(step);
         Assert.AreEqual(step, Executor.LastRequest.Step);
         DomainAssert.AreEqual(
            Path.Empty
               .Append(ViewModel)
               .Append(Property),
            Executor.LastRequest.TargetPath
         );
      }

      [TestMethod]
      public void PerformViewModelValidation_ReturnsErrorsOfViewModelAndCollectionValidators() {
         Executor.ResultToReturn = CreateValidationResult("Property error");
         CollectionCache.ResultToReturn = CreateValidationResult("Collection error");

         var expected = ValidationResult.Join(Executor.ResultToReturn, CollectionCache.ResultToReturn);
         var actual = PerformViewModelValidation();

         Assert.AreEqual(expected, actual);
      }

      [TestMethod]
      public void PerformViewModelValidation_InvokesCollectionResultCacheWithCorrectArguments() {
         PerformViewModelValidation();

         Assert.AreEqual(ViewModel, CollectionCache.LastItem);
         Assert.AreEqual(ValidationStep.ViewModel, CollectionCache.LastStep);
         Assert.IsNull(CollectionCache.LastProperty);
      }

      [TestMethod]
      public void PerformViewModelValidation_InvokesValidationExecutorWithCorrectArguments() {
         PerformViewModelValidation();

         Assert.AreEqual(ViewModel.GetContext(), Executor.LastContext);
         Assert.AreEqual(ValidationStep.ViewModel, Executor.LastRequest.Step);
         DomainAssert.AreEqual(Path.Empty.Append(ViewModel), Executor.LastRequest.TargetPath);
      }

      private ValidationResult PerformPropertyValidation(ValidationStep step = ValidationStep.Value) {
         return ValidationOperation.PerformPropertyValidation(
            CollectionCache,
            step,
            ViewModel,
            Property
         );
      }

      private ValidationResult PerformViewModelValidation() {
         return ValidationOperation.PerformViewModelValidation(
            CollectionCache,
            ViewModel
         );
      }

      private class TestCollectionResultCache : CollectionResultCache {
         public TestCollectionResultCache() {
            ResultToReturn = ValidationResult.Valid;
         }

         public ValidationResult ResultToReturn { get; set; }

         public bool WasInvoked { get; set; }
         public ValidationStep LastStep { get; set; }
         public IViewModel LastItem { get; set; }
         public IVMPropertyDescriptor LastProperty { get; set; }

         public override ValidationResult GetCollectionValidationResults(
            ValidationStep step,
            IViewModel item,
            IVMPropertyDescriptor property
         ) {
            Assert.IsFalse(WasInvoked, "GetCollectionValidatorResult was not expected to be called more than once.");
            LastStep = step;
            LastItem = item;
            LastProperty = property;
            return ResultToReturn;
         }
      }

      private class TestValidationExecutor : Behavior, IValidationExecutorBehavior {
         public TestValidationExecutor() {
            ResultToReturn = ValidationResult.Valid;
         }

         public ValidationResult ResultToReturn { get; set; }

         public bool WasInvoked { get; set; }
         public IBehaviorContext LastContext { get; set; }
         public ValidationRequest LastRequest { get; set; }

         public ValidationResult Validate(IBehaviorContext context, ValidationRequest request) {
            Assert.IsFalse(WasInvoked, "Validate was not expected to be called more than once.");
            LastContext = context;
            LastRequest = request;
            return ResultToReturn;
         }
      }
   }
}