namespace Inspiring.MvvmTest.ViewModels.Core.Validation.Behaviors {
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;
   using Inspiring.Mvvm.ViewModels;

   [TestClass]
   public class ViewModelPropertyDescendantsValidatorBehaviorTests : DescendantsValidatorBehaviorFixture {
      private ViewModelPropertyDescendantsValidatorBehavior<ChildVM> Behavior { get; set; }
      private ValueAccessorStub<ChildVM> ViewModelPropertyAccessor { get; set; }
      private BehaviorContextStub Context { get; set; }

      [TestInitialize]
      public void Setup() {
         Behavior = new ViewModelPropertyDescendantsValidatorBehavior<ChildVM>();

         ViewModelPropertyAccessor = new ValueAccessorStub<ChildVM>();
         ViewModelPropertyAccessor.Value = new ChildVM();

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

      [TestMethod]
      public void GetDescendantsValidationResult_IfChildVMIsNull_ReturnsValidResult() {
         ViewModelPropertyAccessor.Value = null;
         SetupLoadedAndValidated();
         Assert.AreEqual(ValidationResult.Valid, Behavior.GetDescendantsValidationResult(Context));
      }

      [TestMethod]
      public void SetValueNext_PreviousValueWasNull_RevalidatesNewChildVM() {
         ChildVM oldChild = null;
         ChildVM newChild = new ChildVM();
         ViewModelPropertyAccessor.Value = oldChild;

         Behavior.SetValue(Context, newChild);

         Assert.IsTrue(newChild.RevalidationBehavior.WasCalled);
      }

      [TestMethod]
      public void SetValueNext_ToNull_RevalidatesOldChildVM() {
         ChildVM oldChild = new ChildVM();
         ChildVM newChild = null;
         ViewModelPropertyAccessor.Value = oldChild;

         Behavior.SetValue(Context, newChild);

         Assert.IsTrue(oldChild.RevalidationBehavior.WasCalled);
      }

      [TestMethod]
      public void SetValueNext_FromOldToNewValue_ValidatesOldAndNewChildVM() {
         ChildVM oldChild = new ChildVM();
         ChildVM newChild = new ChildVM();
         ViewModelPropertyAccessor.Value = oldChild;

         Behavior.SetValue(Context, newChild);

         Assert.IsTrue(oldChild.RevalidationBehavior.WasCalled);
         Assert.IsTrue(newChild.RevalidationBehavior.WasCalled);
      }

      internal override DescendantsValidatorBehavior GetConcreteBehavior() {
         return Behavior;
      }

      internal override IBehaviorContext GetContext() {
         return Context;
      }

      private ValidationResult SetupChildVMValidationError() {
         var result = CreateValidationResult("Child error");

         var child = new ChildVM();
         child.ReturnedValidationResults[ValidationResultScope.All] = result;
         ViewModelPropertyAccessor.Value = child;
         return result;
      }

      private class RevalidationBehaviorStub :
         Behavior,
         IViewModelRevalidationBehavior {

         public void Revalidate(IBehaviorContext context, ValidationController controller) {
            WasCalled = true;
         }

         public bool WasCalled { get; private set; }
      }


      private class ChildVM : ViewModelStub {
         public ChildVM()
            : this(new RevalidationBehaviorStub()) {
         }

         private ChildVM(RevalidationBehaviorStub revalidationBehavior)
            : base(DescriptorStub.WithBehaviors(revalidationBehavior).Build()) {
            RevalidationBehavior = revalidationBehavior;
         }

         public RevalidationBehaviorStub RevalidationBehavior { get; private set; }
      }
   }
}