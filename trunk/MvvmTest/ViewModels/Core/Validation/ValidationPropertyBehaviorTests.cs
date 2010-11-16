namespace Inspiring.MvvmTest.ViewModels.Core.Validation {
   using System.Linq;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;
   using Moq;

   [TestClass]
   public class ValidationPropertyBehaviorTests : TestBase {
      private PropertyBehaviorContextTestHelper _ctx;
      private PropertyValidationBehavior<string> _behavior;

      [TestInitialize]
      public void Setup() {
         _ctx = new PropertyBehaviorContextTestHelper();
         _behavior = new PropertyValidationBehavior<string>();
         _behavior.Initialize(_ctx.InitializationContext);
      }

      #region Validate tests

      [TestMethod]
      public void Validate_WithStateParameter_CallsPropertyValidating() {
         _behavior.Validate(_ctx.Context, new ValidationContext());
         AssertNotifyPropertyValidatingWasCalledOnContext();

      }

      [TestMethod]
      public void Validate_CallsPropertyValidating() {
         _behavior.Validate(_ctx.Context);
         AssertNotifyPropertyValidatingWasCalledOnContext();
      }

      [TestMethod]
      public void Validate_ValidationStateChangesToInvalid_NotifyChangeIsCalled() {
         var watcher = new NotifyChangeWatcher(_ctx);
         SetupInvalidPropertyValidationCallback();

         watcher.StartWatching();
         _behavior.Validate(_ctx.Context);
         watcher.ExpectOneValidationStateChangedInvocation();
      }

      [TestMethod]
      public void Validate_ValidationStateChangesToValid_NotifyChangeIsCalled() {
         var watcher = new NotifyChangeWatcher(_ctx);
         SetupInvalidPropertyValidationCallback();
         _behavior.Validate(_ctx.Context);
         SetupValidPropertyValidationCallback();

         watcher.StartWatching();
         _behavior.Validate(_ctx.Context);
         watcher.ExpectOneValidationStateChangedInvocation();
      }

      [TestMethod]
      public void Validate_ValidationStateStaysValid_NotifyChangeIsNotCalled() {
         SetupValidPropertyValidationCallback();
         _behavior.Validate(_ctx.Context);

         var watcher = new NotifyChangeWatcher(_ctx);
         watcher.StartWatching();
         _behavior.Validate(_ctx.Context);
         watcher.ExepctNoInvocation();
      }

      #endregion

      #region GetValidationState tests

      [TestMethod]
      public void GetValidationState_Initially_ReturnsValidState() {
         var state = _behavior.GetValidationState(_ctx.Context);
         Assert.AreSame(ValidationState.Valid, state);
      }

      [TestMethod]
      public void GetValidationState_AfterValidateWithoutErrors_ReturnsValidState() {
         _behavior.Validate(_ctx.Context);
         var state = _behavior.GetValidationState(_ctx.Context);
         Assert.AreSame(ValidationState.Valid, state);
      }

      [TestMethod]
      public void GetValidationState_AfterValidateWithErrors_ReturnsInvalidState() {
         ValidationError expectedError = new ValidationError("Test");
         SetupInvalidPropertyValidationCallback(expectedError);

         _behavior.Validate(_ctx.Context);
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

      private void SetupValidPropertyValidationCallback() {
         _ctx
           .ContextMock
           .Setup(x => x.NotifyPropertyValidating(It.IsAny<IVMProperty>(), It.IsAny<ValidationState>()))
           .Callback<IVMProperty, ValidationState>((_, state) => { });
      }

      private void SetupInvalidPropertyValidationCallback(ValidationError expectedError = null) {
         expectedError = expectedError ?? new ValidationError("Test");

         _ctx
           .ContextMock
           .Setup(x => x.NotifyPropertyValidating(It.IsAny<IVMProperty>(), It.IsAny<ValidationState>()))
           .Callback<IVMProperty, ValidationState>((_, state) => state.Errors.Add(expectedError));
      }

      private void AssertNotifyPropertyValidatingWasCalledOnContext() {
         _ctx
            .ContextMock
            .Verify(
               x => x.NotifyPropertyValidating(_ctx.Property, It.IsAny<ValidationState>()),
               Times.Once()
            );
      }

      private class NotifyChangeWatcher {
         private PropertyBehaviorContextTestHelper _ctx;

         public NotifyChangeWatcher(PropertyBehaviorContextTestHelper ctx) {
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