namespace Inspiring.MvvmTest.ViewModels.Core.Validation.Behaviors {
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class ValidationResultAggregatorBehaviorTests : ValidationTestBase {
      private static readonly ValidationResult PropertyResult = CreateValidationResult("Property error");
      private static readonly ValidationResult ViewModelResult = CreateValidationResult("ViewModel error");
      private static readonly ValidationResult DescendantResult = CreateValidationResult("Descendant error");

      private ValidationResultAggregatorBehavior Behavior { get; set; }
      private IBehaviorContext Context { get; set; }

      [TestInitialize]
      public void Setup() {
         Behavior = new ValidationResultAggregatorBehavior();

         var propertyBehavior = new ValidationResultProviderStub {
            ReturnedResult = PropertyResult,
            ReturnedDescendantsResult = DescendantResult
         };

         var viewModelBehavior = new ValidationResultProviderStub {
            ReturnedResult = ViewModelResult
         };

         var property = PropertyStub
            .WithBehaviors(propertyBehavior)
            .Of<ViewModelStub>();

         Context = ViewModelStub
            .WithProperties(property)
            .WithBehaviors(Behavior, viewModelBehavior)
            .BuildContext();
      }


      [TestMethod]
      public void GetValidationResultWithProperty_ReturnsOnlyPropertyErrors() {
         var actual = Behavior.GetValidationResult(Context, ValidationResultScope.PropertiesOnly);
         Assert.AreEqual(PropertyResult, actual);
      }

      [TestMethod]
      public void GetValidationResultWithViewModel_ReturnsOnlyViewModelErrors() {
         var actual = Behavior.GetValidationResult(Context, ValidationResultScope.ViewModelValidationsOnly);
         Assert.AreEqual(ViewModelResult, actual);
      }

      [TestMethod]
      public void GetValidationResultWithDescendants_ReturnsDescendantsErrorsOnly() {
         var actual = Behavior.GetValidationResult(Context, ValidationResultScope.Descendants);
         Assert.AreEqual(DescendantResult, actual);
      }

      [TestMethod]
      public void GetValidationResultWithSelf_ReturnsPropertyAndViewModelErrors() {
         var expected = ValidationResult.Join(PropertyResult, ViewModelResult);
         var actual = Behavior.GetValidationResult(Context, ValidationResultScope.Self);
         Assert.AreEqual(expected, actual);
      }

      [TestMethod]
      public void GetValidationResultWithAll_ReturnsPropertyViewModelAndDescendantErrors() {
         var expected = ValidationResult.Join(new[] { PropertyResult, ViewModelResult, DescendantResult });
         var actual = Behavior.GetValidationResult(Context, ValidationResultScope.All);
         Assert.AreEqual(expected, actual);
      }
   }
}