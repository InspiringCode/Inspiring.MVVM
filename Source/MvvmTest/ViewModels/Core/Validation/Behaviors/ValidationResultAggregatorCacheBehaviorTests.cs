namespace Inspiring.MvvmTest.ViewModels.Core.Validation.Behaviors {
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class ValidationResultAggregatorCacheBehaviorTests : ValidationTestBase {
      private static readonly ValidationResult PropertyResult = CreateValidationResult("Property error");
      private static readonly ValidationResult ViewModelResult = CreateValidationResult("ViewModel error");
      private static readonly ValidationResult DescendantResult = CreateValidationResult("Descendant error");

      private ValidationResultAggregatorCacheBehavior Behavior { get; set; }
      private NextAggregatorInvocationCounter Counter { get; set; }
      private ValidationResultAggregatorStub Next { get; set; }
      private IBehaviorContext Context { get; set; }

      [TestInitialize]
      public void Setup() {
         Next = new ValidationResultAggregatorStub();
         Next.ReturnedValidationResults[ValidationResultScope.PropertiesOnly] = PropertyResult;
         Next.ReturnedValidationResults[ValidationResultScope.ViewModelValidationsOnly] = ViewModelResult;
         Next.ReturnedValidationResults[ValidationResultScope.Descendants] = DescendantResult;

         Behavior = new ValidationResultAggregatorCacheBehavior();

         Counter = new NextAggregatorInvocationCounter();

         Context = ViewModelStub
            .WithBehaviors(Behavior, Counter, Next)
            .BuildContext();
      }

      [TestMethod]
      public void GetValidationResult_CalledTwice_CallsNextBehaviorOnlyOnce() {
         var basicScopes = new[] { 
            ValidationResultScope.PropertiesOnly, 
            ValidationResultScope.ViewModelValidationsOnly, 
            ValidationResultScope.Descendants 
         };

         foreach (ValidationResultScope scope in basicScopes) {
            Behavior.GetValidationResult(Context, scope);
            Counter.Invocations = 0;
            Behavior.GetValidationResult(Context, scope);
            Assert.AreEqual(0, Counter.Invocations);
         }
      }

      [TestMethod]
      public void GetValidationResult_ReturnsResultOfNextBehavior() {
         Assert.AreEqual(PropertyResult, Behavior.GetValidationResult(Context, ValidationResultScope.PropertiesOnly));
         Assert.AreEqual(ViewModelResult, Behavior.GetValidationResult(Context, ValidationResultScope.ViewModelValidationsOnly));
         Assert.AreEqual(DescendantResult, Behavior.GetValidationResult(Context, ValidationResultScope.Descendants));
      }

      [TestMethod]
      public void GetValidationResultForSelf_UsesCachedResults() {
         Behavior.GetValidationResult(Context, ValidationResultScope.PropertiesOnly);
         Behavior.GetValidationResult(Context, ValidationResultScope.ViewModelValidationsOnly);
         Counter.Invocations = 0;

         var actualResult = Behavior.GetValidationResult(Context, ValidationResultScope.Self);
         var expectedResult = ValidationResult.Join(PropertyResult, ViewModelResult);

         Assert.AreEqual(0, Counter.Invocations);
         Assert.AreEqual(expectedResult, actualResult);
      }

      [TestMethod]
      public void GetValidationResultForAll_UsesCachedResult() {
         Behavior.GetValidationResult(Context, ValidationResultScope.PropertiesOnly);
         Behavior.GetValidationResult(Context, ValidationResultScope.ViewModelValidationsOnly);
         Behavior.GetValidationResult(Context, ValidationResultScope.Descendants);
         Counter.Invocations = 0;

         var actualResult = Behavior.GetValidationResult(Context, ValidationResultScope.All);
         var expectedResult = ValidationResult.Join(new[] { PropertyResult, ViewModelResult, DescendantResult });

         Assert.AreEqual(0, Counter.Invocations);
         Assert.AreEqual(expectedResult, actualResult);
      }

      [TestMethod]
      public void ValidationStateChange_InvalidatesCache() {
         Behavior.GetValidationResult(Context, ValidationResultScope.All);
         var expectedResult = ChangeNextResultsAndReturnJoinedResults();

         Behavior.HandleChange(
            Context,
            ChangeArgs.ValidationResultChanged().PrependViewModel(ViewModelStub.Build())
         );

         var actualResult = Behavior.GetValidationResult(Context, ValidationResultScope.All);

         Assert.AreEqual(expectedResult, actualResult);
      }

      [TestMethod]
      public void PropertyChange_DoesNotInvalidateCache() {
         Behavior.GetValidationResult(Context, ValidationResultScope.All);
         Counter.Invocations = 0;

         Behavior.HandleChange(
            Context,
            ChangeArgs.PropertyChanged(PropertyStub.Of<string>(), ValueStage.ValidatedValue).PrependViewModel(ViewModelStub.Build())
         );

         var actualResult = Behavior.GetValidationResult(Context, ValidationResultScope.All);
         var expectedResult = ValidationResult.Join(new[] { PropertyResult, ViewModelResult, DescendantResult });

         Assert.AreEqual(0, Counter.Invocations);
         Assert.AreEqual(expectedResult, actualResult);
      }

      [TestMethod]
      public void ValidationResultChanged_OfDescendant_DoesNotInvalidateCache() {
         var expectedResult = Behavior.GetValidationResult(Context, ValidationResultScope.All);
         ChangeNextResultsAndReturnJoinedResults();

         Behavior.HandleChange(
            Context,
            ChangeArgs.ValidationResultChanged()
               .PrependViewModel(ViewModelStub.Build())
               .PrependViewModel(ViewModelStub.Build())
         );

         var actualResult = Behavior.GetValidationResult(Context, ValidationResultScope.All);
         Assert.AreEqual(expectedResult, actualResult);
      }

      [TestMethod]
      public void HandleChange_AlsoRefreshesParentCaches() {
         Behavior.GetValidationResult(Context, ValidationResultScope.All);
         var newResult = ChangeNextResultsAndReturnJoinedResults();

         var childBehavior = new ValidationResultAggregatorCacheBehavior();
         var childVM = ViewModelStub
            .WithBehaviors(childBehavior, new ValidationResultAggregatorStub())
            .Build();

         childVM.Kernel.Parents.Add(Context.VM);

         childBehavior.HandleChange(
            childVM.GetContext(),
            ChangeArgs.ValidationResultChanged().PrependViewModel(ViewModelStub.Build())
         );

         var actualResult = Behavior.GetValidationResult(Context, ValidationResultScope.All);
         Assert.AreEqual(newResult, actualResult);
      }

      private ValidationResult ChangeNextResultsAndReturnJoinedResults() {
         var propertyResult = CreateValidationResult("New property error");
         var viewModelResult = CreateValidationResult("New view model error");
         var descendantsResult = CreateValidationResult("New descendants error");

         Next.ReturnedValidationResults[ValidationResultScope.PropertiesOnly] = propertyResult;
         Next.ReturnedValidationResults[ValidationResultScope.ViewModelValidationsOnly] = viewModelResult;
         Next.ReturnedValidationResults[ValidationResultScope.Descendants] = descendantsResult;

         return ValidationResult.Join(new[] { propertyResult, viewModelResult, descendantsResult });
      }

      private class NextAggregatorInvocationCounter :
         Behavior,
         IValidationResultAggregatorBehavior {

         public int Invocations { get; set; }

         public ValidationResult GetValidationResult(IBehaviorContext context, ValidationResultScope scope) {
            Invocations++;
            return this.GetValidationResultNext(context, scope);
         }
      }

   }
}