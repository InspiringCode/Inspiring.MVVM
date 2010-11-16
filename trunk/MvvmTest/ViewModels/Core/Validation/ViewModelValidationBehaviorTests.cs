namespace Inspiring.MvvmTest.ViewModels.Core.Validation {
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;
   using Moq;

   [TestClass]
   public class ViewModelValidationBehaviorTests : TestBase {
      private ViewModelBehaviorContextHelper _ctx;
      private ViewModelValidationBehavior _behavior;

      [TestInitialize]
      public void Setup() {
         _ctx = new ViewModelBehaviorContextHelper();
         _behavior = new ViewModelValidationBehavior();
         _behavior.Initialize(_ctx.InitializationContext);
      }

      #region Validate tests

      [TestMethod]
      public void Validate_CallsViewModelValidating() {
         _behavior.Validate(_ctx.Context);
         Expect_ViewModelValidating_WasCalledOnContext();
      }

      [TestMethod]
      public void Validate_WithStateParameter_CallsViewModelValidating() {
         _behavior.Validate(_ctx.Context, new ValidationContext());
         Expect_ViewModelValidating_WasCalledOnContext();
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

      #region ViewModelValidating tests



      #endregion

      private void Expect_ViewModelValidating_WasCalledOnContext() {
         _ctx
            .ContextMock
            .Verify(
               x => x.NotifyViewModelValidating(It.IsAny<ValidationState>()),
               Times.Once()
            );
      }

      private void SetupValidPropertyValidationCallback() {
         _ctx
           .ContextMock
           .Setup(x => x.NotifyViewModelValidating(It.IsAny<ValidationState>()))
           .Callback<ValidationState>(state => { });
      }

      private void SetupInvalidPropertyValidationCallback(ValidationError expectedError = null) {
         expectedError = expectedError ?? new ValidationError("Test");

         _ctx
           .ContextMock
           .Setup(x => x.NotifyViewModelValidating(It.IsAny<ValidationState>()))
           .Callback<ValidationState>(state => state.Errors.Add(expectedError));
      }

      private class NotifyChangeWatcher {
         private ViewModelBehaviorContextHelper _ctx;

         public NotifyChangeWatcher(ViewModelBehaviorContextHelper ctx) {
            _ctx = ctx;
         }

         public int InvocationCount { get; set; }

         public ChangeArgs Args { get; set; }

         public NotifyChangeWatcher StartWatching() {
            _ctx
               .ContextMock
               .Setup(x => x.NotifyChange(It.IsAny<ChangeArgs>()))
               .Callback<ChangeArgs>(args => {
                  InvocationCount++;
                  Args = args;
               });

            return this;
         }

         public void ExpectOneValidationStateChangedInvocation() {
            Assert.AreEqual(1, InvocationCount);
            Assert.AreEqual(ChangeType.ValidationStateChanged, Args.ChangeType);
            Assert.AreEqual(_ctx.VM, Args.ChangedVM);
            Assert.IsNull(Args.ChangedProperty);
         }

         public void ExepctNoInvocation() {
            Assert.AreEqual(0, InvocationCount);
         }
      }

      //private class ViewModelValidatingWatcher {
      //   private ViewModelBehaviorContextHelper _ctx;

      //   private ViewModelValidatingWatcher(ViewModelBehaviorContextHelper ctx) {
      //      _ctx = ctx;
      //   }

      //   public int InvocationCount { get; set; }

      //   public object Args { get; set; }

      //   public static ViewModelValidatingWatcher StartWatching(ViewModelBehaviorContextHelper ctx) {
      //      var watcher = new ViewModelValidatingWatcher { _ctx = ctx };

      //   }

      //   private void StartWatching() {
      //      _ctx.ContextMock.Setup(x => x.NotifyViewModelValidating())
      //         .Callback<object>();
      //   }
      //}

      //[TestMethod]
      //public void OnChanged_RootValidator_GetsCalled() {
      //   var behavior = new ViewModelValidationBehavior();
      //   var validator = new ValidatorMock();
      //   var employeeVM = new Mock<IViewModel>().Object;

      //   behavior.AddValidation(VMPropertyPath.Empty, validator);

      //   behavior.OnChanged(
      //      new ViewModelBehaviorContextStub(employeeVM),
      //      new ChangeArgs(ChangeType.PropertyChanged, employeeVM),
      //      new InstancePath(employeeVM)
      //   );

      //   Assert.AreEqual(1, validator.InvocationCount);
      //   Assert.AreSame(employeeVM, validator.Args.ChangedVM);
      //   Assert.AreSame(employeeVM, validator.Args.ValidationTarget);
      //   Assert.IsTrue(validator.ChangedVMPath.IsEmpty);
      //}

      //[TestMethod]
      //public void OnChanged_ChildValidator_GetsNotCalled() {
      //   var behavior = new ViewModelValidationBehavior();
      //   var validator = new ValidatorMock();
      //   var employeeVM = new Mock<IViewModel>();
      //   var addressProperty = new Mock<IVMProperty>().Object;

      //   behavior.AddValidation(new VMPropertyPath(addressProperty), validator);

      //   behavior.OnChanged(
      //      new ViewModelBehaviorContextStub(employeeVM.Object),
      //      new ChangeArgs(ChangeType.PropertyChanged, employeeVM.Object),
      //      new InstancePath(employeeVM.Object)
      //   );

      //   Assert.AreEqual(0, validator.InvocationCount);
      //}

      //[TestMethod]
      //public void OnChildChanged_RootValidator_GetsCalled() {
      //   var behavior = new ViewModelValidationBehavior();
      //   var validator = new ValidatorMock();
      //   var employeeVM = new Mock<IViewModel>().Object;
      //   var addressVM = new Mock<IViewModel>().Object;

      //   behavior.AddValidation(VMPropertyPath.Empty, validator);

      //   behavior.OnChanged(
      //      new ViewModelBehaviorContextStub(employeeVM),
      //      new ChangeArgs(ChangeType.PropertyChanged, addressVM),
      //      new InstancePath(employeeVM, addressVM)
      //   );

      //   Assert.AreEqual(1, validator.InvocationCount);
      //   Assert.AreSame(addressVM, validator.Args.ChangedVM);
      //   Assert.AreSame(employeeVM, validator.Args.ValidationTarget);

      //   AssertHelper.AreEqual(CreateSteps(addressVM), validator.ChangedVMPath.Steps, StepsAreEqual);
      //}

      //[TestMethod]
      //public void OnChildChanged_ChildValidatorWithMatchingPath_GetsCalled() {
      //   var behavior = new ViewModelValidationBehavior();
      //   var validator = new ValidatorMock();
      //   var employeeVM = new Mock<IViewModel>();
      //   var addressVM = new Mock<IViewModel>();
      //   var addressProperty = new Mock<IVMProperty>().Object;

      //   employeeVM.Setup(x => x.GetValue(addressProperty)).Returns(addressVM.Object);

      //   behavior.AddValidation(new VMPropertyPath(addressProperty), validator);

      //   behavior.OnChanged(
      //      new ViewModelBehaviorContextStub(employeeVM.Object),
      //      new ChangeArgs(ChangeType.PropertyChanged, addressVM.Object),
      //      new InstancePath(employeeVM.Object, addressVM.Object)
      //   );

      //   Assert.AreEqual(1, validator.InvocationCount);
      //   Assert.AreSame(addressVM.Object, validator.Args.ChangedVM);
      //   Assert.AreSame(addressVM.Object, validator.Args.ValidationTarget);
      //   Assert.IsTrue(validator.ChangedVMPath.IsEmpty);
      //}

      //[TestMethod]
      //public void OnChildChanged_ChildValidatorWithNonMatchingPath_GetsNotCalled() {

      //}

      //private static InstancePathStep[] CreateSteps(params IViewModel[] steps) {
      //   return steps.Select(x => new InstancePathStep(x)).ToArray();
      //}

      //private static bool StepsAreEqual(InstancePathStep x, InstancePathStep y) {
      //   if (Object.ReferenceEquals(x, y)) {
      //      return true;
      //   }

      //   return
      //      x != null &&
      //      y != null &&
      //      Object.ReferenceEquals(x.VM, y.VM) &&
      //      Object.ReferenceEquals(x.ParentCollection, y.ParentCollection);
      //}

      //private class ValidatorMock : ViewModelValidator {
      //   public int InvocationCount { get; private set; }
      //   public ViewModelValidationArgs Args { get; private set; }
      //   public InstancePath ChangedVMPath { get; private set; }


      //   public override void Validate(ViewModelValidationArgs args) {
      //      throw new NotImplementedException();
      //   }
      //   //public override void Validate(ViewModelValidationArgs args) {
      //   //   throw new NotImplementedException();
      //   //}
      //}
   }
}