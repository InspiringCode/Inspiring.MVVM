namespace Inspiring.MvvmTest.ViewModels.Core.Validation {
   using System.Linq;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;
   using Moq;

   [TestClass]
   public class ValidationPropertyBehaviorTests : TestBase {
      private ContextTestHelper _ctx;
      private PropertyValidationBehavior<string> _behavior;

      [TestInitialize]
      public void Setup() {
         _ctx = new ContextTestHelper();
         _behavior = new PropertyValidationBehavior<string>();
         _behavior.Successor = new InstancePropertyBehavior<string>();
         _behavior.Initialize(_ctx.InitializationContext);
      }

      #region Validate tests

      [TestMethod]
      public void Validate_WithStateParameter_CallsPropertyValidating() {
         ValidationContext.BeginValidation();
         _behavior.Validate(_ctx.Context, ValidationContext.Current);
         AssertNotifyPropertyValidatingWasCalledOnContext();

      }

      [TestMethod]
      public void Validate_CallsPropertyValidating() {
         Validate();
         AssertNotifyPropertyValidatingWasCalledOnContext();
      }

      [TestMethod]
      public void Validate_ValidationStateChangesToInvalid_NotifyChangeIsCalled() {
         var watcher = new NotifyChangeWatcher(_ctx);
         SetupInvalidPropertyValidationCallback();

         watcher.StartWatching();
         Validate();
         watcher.ExpectOneValidationStateChangedInvocation();
      }

      [TestMethod]
      public void Validate_ValidationStateChangesToValid_NotifyChangeIsCalled() {
         var watcher = new NotifyChangeWatcher(_ctx);
         SetupInvalidPropertyValidationCallback();
         Validate();
         SetupValidPropertyValidationCallback();

         watcher.StartWatching();
         Validate();
         watcher.ExpectOneValidationStateChangedInvocation();
      }

      [TestMethod]
      public void Validate_ValidationStateStaysValid_NotifyChangeIsNotCalled() {
         SetupValidPropertyValidationCallback();
         Validate();

         var watcher = new NotifyChangeWatcher(_ctx);
         watcher.StartWatching();
         Validate();
         watcher.ExepctNoInvocation();
      }

      #endregion

      #region GetValidationState tests

      [TestMethod]
      public void GetValidationState_Initially_ReturnsValidState() {
         var state = _behavior.GetValidationState(_ctx.Context);
         Assert.AreSame(ValidationResult.Valid, state);
      }

      [TestMethod]
      public void GetValidationState_AfterValidateWithoutErrors_ReturnsValidState() {
         Validate();
         var state = _behavior.GetValidationState(_ctx.Context);
         Assert.AreSame(ValidationResult.Valid, state);
      }

      [TestMethod]
      public void GetValidationState_AfterValidateWithErrors_ReturnsInvalidState() {
         ValidationError expectedError = new ValidationError(_ctx.VM, Mock<Validator>(), "Test");
         SetupInvalidPropertyValidationCallback(expectedError);

         Validate();
         var state = _behavior.GetValidationState(_ctx.Context);

         CollectionAssert.AreEquivalent(
            new ValidationError[] { expectedError },
            state.Errors.ToArray()
         );
      }

      #endregion

      #region SetValue tests

      [TestMethod]
      public void SetValue_CallsNotifyPropertyChanging() {
         _behavior.SetValue(_ctx.Context, "Test");
         AssertNotifyPropertyValidatingWasCalledOnContext();
      }

      #endregion

      private void Validate() {
         ValidationContext.BeginValidation();
         _behavior.Validate(_ctx.Context, ValidationContext.Current);
         ValidationContext.CompleteValidation(ValidationMode.CommitValidValues);
      }

      private void SetupValidPropertyValidationCallback() {
         _ctx
           .ContextMock
           .Setup(x => x.NotifyValidating(It.IsAny<ValidationArgs>()))
           .Callback<ValidationArgs>(args => { });
      }

      private void SetupInvalidPropertyValidationCallback(ValidationError expectedError = null) {
         expectedError = expectedError ?? new ValidationError(_ctx.VM, Mock<Validator>(), "Test");

         _ctx
           .ContextMock
           .Setup(x => x.NotifyValidating(It.IsAny<ValidationArgs>()))
           .Callback<ValidationArgs>(args => args.SetTargetValidator(expectedError.Validator).AddError(expectedError.Message));
      }

      private void AssertNotifyPropertyValidatingWasCalledOnContext() {
         _ctx
            .ContextMock
            .Verify(
               x => x.NotifyValidating(It.IsAny<ValidationArgs>()),
               Times.Once()
            );
      }

      private class NotifyChangeWatcher {
         private ContextTestHelper _ctx;

         public NotifyChangeWatcher(ContextTestHelper ctx) {
            _ctx = ctx;
         }

         public int InvocationCount { get; set; }

         public ChangeArgs Args { get; set; }

         public void StartWatching() {
            _ctx
               .ContextMock
               .Setup(x => x.NotifyChange(It.IsAny<ChangeArgs>()))
               .Callback<ChangeArgs>(args => {
                  InvocationCount++;
                  Args = args;
               });
         }

         public void ExpectOneValidationStateChangedInvocation() {
            Assert.AreEqual(1, InvocationCount);
            Assert.AreEqual(ChangeType.ValidationStateChanged, Args.ChangeType);
            Assert.AreEqual(_ctx.VM, Args.ChangedVM);
            Assert.AreEqual(_ctx.Property, Args.ChangedProperty);
         }

         public void ExepctNoInvocation() {
            Assert.AreEqual(0, InvocationCount);
         }
      }
   }
}