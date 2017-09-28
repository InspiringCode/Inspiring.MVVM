namespace Inspiring.MvvmTest.ViewModels.Core.Validation {
   using System.Linq;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   // TODO: Test if next behaviors are called correctly!

   [TestClass]
   public class PropertyValidationSourceBehaviorBaseTests : ValidationTestBase {
      private ValidationResult ValidationResult { get; set; }

      private TestValidationSourceBehavior Behavior { get; set; }
      private PropertyStub<string> Property { get; set; }

      private ValidationExecutorStub Executor { get; set; }
      private BehaviorContextStub Context { get; set; }


      [TestInitialize]
      public void Setup() {
         ValidationResult = CreateValidationResult("Error");

         Behavior = new TestValidationSourceBehavior(ValidationStep.Value);
         Property = PropertyStub.WithBehaviors(Behavior).Of<string>();
         Executor = new ValidationExecutorStub();

         var vm = ViewModelStub
            .WithProperties(Property)
            .WithBehaviors(Executor)
            .Build();

         Context = new BehaviorContextStub(vm);
      }

      [TestMethod]
      public void SetValue_ValidationSucceeds_SetsNextValue() {
         string value = "New value";
         SetValidValue(value);
         Assert.AreEqual(value, Behavior.LastSetNextValue);
      }

      [TestMethod]
      public void SetValue_ValidationFails_DoesNotSetNextValue() {
         string value = "New value";
         SetInvalidValue(value);
         Assert.IsNull(Behavior.LastSetNextValue);
      }

      [TestMethod]
      public void SetValue_HasInvalidValueAndValidationSucceeds_DoesNotCallNotifyChangeForValue() {
         // The PropertyChangedBehavior calls notify change anyhow

         SetInvalidValue();
         Context
            .NotifyChangeInvocations
            .Clear();

         string value = "New value";
         SetValidValue(value);

         Assert.IsFalse(
            Context
               .NotifyChangeInvocations
               .Any(x => x.ChangeType == ChangeType.PropertyChanged)
         );
      }

      [TestMethod]
      public void GetValue_ValidationSucceeded_ReturnsNextValue() {
         SetValidValue();
         Behavior.NextValue = "Source value";
         Assert.AreEqual(Behavior.NextValue, Behavior.GetValue(Context));
      }

      [TestMethod]
      public void GetValue_ValidationFailed_ReturnsInvalidValue() {
         var invalidValue = "Invalid value";
         SetInvalidValue(invalidValue);
         Assert.AreEqual(invalidValue, Behavior.GetValue(Context));
      }

      [TestMethod]
      public void GetValidationResult_ValidationSucceeded_ReturnsValidResult() {
         SetValidValue();
         Assert.IsTrue(Behavior.GetValidationResult(Context).IsValid);
      }

      [TestMethod]
      public void GetValidationResult_ValidationFailed_ReturnsErrors() {
         SetInvalidValue();
         Assert.IsFalse(Behavior.GetValidationResult(Context).IsValid);
      }

      [TestMethod]
      public void HandlePropertyChanged_ValidationFailed_ClearsValidationResult() {
         SetInvalidValue();
         Behavior.NextValue = "Source value";
         Behavior.HandlePropertyChanged(Context, ChangeArgs.PropertyChanged(Property, ValueStage.ValidatedValue));
         Assert.AreEqual(Behavior.NextValue, Behavior.GetValue(Context));
      }

      [TestMethod]
      public void Revalidate_ValidationSucceeds_GetValidationResultReturnsValidResult() {
         SetInvalidValue();
         Executor.ResultToReturn = ValidationResult.Valid;
         Revalidate();
         Assert.IsTrue(Behavior.GetValidationResult(Context).IsValid);
      }

      [TestMethod]
      public void Revalidate_ValidationFails_GetValidationResultReturnsError() {
         SetValidValue();
         Executor.ResultToReturn = ValidationResult;
         Revalidate();
         Assert.IsFalse(Behavior.GetValidationResult(Context).IsValid);
      }

      [TestMethod]
      public void Revalidate_HasInvalidValueAndValidationSucceeds_NextValueIsSetToPreviouslyInvalidValue() {
         var value = "Previously invalid value";
         SetInvalidValue(value);
         Executor.ResultToReturn = ValidationResult.Valid;
         Revalidate();
         Assert.AreEqual(value, Behavior.LastSetNextValue);
      }

      [TestMethod]
      public void Revalidate_HasInvalidValueAndValidationSucceeds_DiscardsInvalidValue() {
         SetInvalidValue();
         Executor.ResultToReturn = ValidationResult.Valid;
         Revalidate();
         Behavior.NextValue = "New source value";
         Assert.AreEqual(Behavior.NextValue, Behavior.GetValue(Context));
      }

      [TestMethod]
      public void Revalidate_HasInvalidValueAndValidationSucceeds_DoesNotCallNotifyChangeForValue() {
         // The PropertyChangedBehavior calls notify change anyhow

         SetValidValue();
         Executor.ResultToReturn = ValidationResult;
         Revalidate();

         Assert.IsFalse(
            Context
               .NotifyChangeInvocations
               .Any(x => x.ChangeType == ChangeType.PropertyChanged)
         );
      }

      private void SetInvalidValue(string value = "Invalid value") {
         Executor.ResultToReturn = ValidationResult;
         Behavior.SetValue(Context, value);
      }

      private void SetValidValue(string value = "Valid value") {
         Executor.ResultToReturn = ValidationResult.Valid;
         Behavior.SetValue(Context, value);
      }

      private void Revalidate() {
         var controller = new ValidationController();
         controller.RequestPropertyRevalidation(Context.VM, Property);
         controller.ProcessPendingValidations();
      }

      private class TestValidationSourceBehavior : PropertyValidationSourceBehaviorBase<string> {
         public TestValidationSourceBehavior(ValidationStep step)
            : base(step, ValueStage.Value) {
            NextValue = "Next test value";
         }

         public string NextValue { get; set; }

         public string LastSetNextValue { get; private set; }

         public string GetValue(IBehaviorContext context) {
            return GetInvalidValueOrNext(context);
         }

         public void SetValue(IBehaviorContext context, string value) {
            SetValueNextIfValidationSucceeds(context, value);
         }

         protected override string GetValueNext(IBehaviorContext context) {
            return NextValue;
         }

         protected override void SetValueNext(IBehaviorContext context, string value) {
            LastSetNextValue = value;
         }
      }
   }
}