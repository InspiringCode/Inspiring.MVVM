namespace Inspiring.MvvmTest.ViewModels.Core.Validation {
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class ViewModelPropertyDescendantsValidatorBehaviorTests : DescendantsValidatorBehaviorFixture {
      private DescendantsValidatorBehavior Behavior { get; set; }
      private ValueAccessorStub<ViewModelStub> ViewModelPropertyAccessor { get; set; }
      private BehaviorContextStub Context { get; set; }

      [TestInitialize]
      public void Setup() {
         Behavior = new ViewModelPropertyDescendantsValidatorBehavior<ViewModelStub>();

         ViewModelPropertyAccessor = new ValueAccessorStub<ViewModelStub>();
         ViewModelPropertyAccessor.Value = ViewModelStub.Build();

         var property = PropertyStub
            .WithBehaviors(Behavior, Next, ViewModelPropertyAccessor)
            .Of<string>();

         Context = CreateBehaviorContextFor(property);
      }

      [TestMethod]
      public void GetDescendantsValidationResult_OnUnloadedDescendant_OnlyCallsNextBehavior() {
         SetupChildVMValidationError();
         SetupUnloaded();
         var result = Behavior.GetDescendantsValidationResult(Context);
         Assert.AreEqual(GetDescendantsValidationResultNext, ActionLog);
         Assert.AreEqual(ValidationResult.Valid, result);
      }

      [TestMethod]
      public void GetDescendantsValidationResult_OnUnloadedValidatedDescendant_OnlyCallsNextBehavior() {
         SetupChildVMValidationError();
         SetupUnloadedAndValidated();
         var result = Behavior.GetDescendantsValidationResult(Context);
         Assert.AreEqual(GetDescendantsValidationResultNext, ActionLog);
         Assert.AreEqual(ValidationResult.Valid, result);
      }

      [TestMethod]
      public void GetDescendantsValidationResult_OnLoadedAndValidatedDescendant_ReturnsValidationResultOfChildVM() {
         var expectedResult = SetupChildVMValidationError();
         SetupLoadedAndValidated();
         var actualResult = Behavior.GetDescendantsValidationResult(Context);
         Assert.AreEqual(expectedResult, actualResult);
      }

      internal override DescendantsValidatorBehavior GetConcreteBehavior() {
         return Behavior;
      }

      internal override IBehaviorContext GetContext() {
         return Context;
      }

      private ValidationResult SetupChildVMValidationError() {
         var result = CreateValidationResult("Child error");
         ViewModelPropertyAccessor.Value = CreateInvalidVM(result);
         return result;
      }
   }
}