namespace Inspiring.MvvmTest.ViewModels.Core.Validation.Behaviors {
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class CollectionPropertyDescendantsValidatorBehaviorTests : DescendantsValidatorBehaviorFixture {
      private DescendantsValidatorBehavior Behavior { get; set; }
      private ValueAccessorStub<IVMCollection<ViewModelStub>> ViewModelPropertyAccessor { get; set; }
      private BehaviorContextStub Context { get; set; }

      [TestInitialize]
      public void Setup() {
         Behavior = new CollectionPropertyDescendantsValidatorBehavior<ViewModelStub>();

         ViewModelPropertyAccessor = new ValueAccessorStub<IVMCollection<ViewModelStub>>();
         ViewModelPropertyAccessor.Value = VMCollectionStub.Build();

         var property = PropertyStub
            .WithBehaviors(Behavior, Next, ViewModelPropertyAccessor)
            .Of<string>();

         Context = CreateBehaviorContextFor(property);
      }

      [TestMethod]
      public void GetDescendantsValidationResult_OnUnloadedDescendant_OnlyCallsNextBehavior() {
         SetupItemsValidationErrors();
         SetupUnloaded();
         var result = Behavior.GetDescendantsValidationResult(Context);
         Assert.AreEqual(GetDescendantsValidationResultNext, ActionLog);
         Assert.AreEqual(ValidationResult.Valid, result);
      }

      [TestMethod]
      public void GetDescendantsValidationResult_OnUnloadedValidatedDescendant_OnlyCallsNextBehavior() {
         SetupItemsValidationErrors();
         SetupUnloadedAndValidated();
         var result = Behavior.GetDescendantsValidationResult(Context);
         Assert.AreEqual(GetDescendantsValidationResultNext, ActionLog);
         Assert.AreEqual(ValidationResult.Valid, result);
      }

      [TestMethod]
      public void GetDescendantsValidationResult_OnLoadedAndValidatedDescendant_ReturnsValidationResultOfChildVM() {
         var expectedResult = SetupItemsValidationErrors();
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

      private ValidationResult SetupItemsValidationErrors() {
         var item1Result = CreateValidationResult("Item 1 error");
         var item2Result = CreateValidationResult("Item 2 error");

         ViewModelPropertyAccessor.Value.Add(CreateInvalidVM(item1Result));
         ViewModelPropertyAccessor.Value.Add(CreateInvalidVM(item2Result));

         return ValidationResult.Join(item1Result, item2Result);
      }
   }
}