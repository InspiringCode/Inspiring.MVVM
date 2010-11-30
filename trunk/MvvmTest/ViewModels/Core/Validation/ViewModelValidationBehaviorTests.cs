namespace Inspiring.MvvmTest.ViewModels.Core.Validation {
   using Inspiring.Mvvm.ViewModels;
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

      [TestClass]
      public class ViewModelBehavior_ValidateTests {
         private ViewModelBehaviorContextHelper _ctx;
         private ViewModelValidationBehavior _behavior;

         [TestInitialize]
         public void Setup() {
            _ctx = new ViewModelBehaviorContextHelper();
            _behavior = new ViewModelValidationBehavior();
            _behavior.Initialize(_ctx.InitializationContext);
         }

         [TestMethod]
         public void Validate_CallsViewModelValidating() {
            _behavior.Validate(_ctx.Context);
            Expect_NotifyValidating_WasCalledOnContext();
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

         private void Expect_NotifyValidating_WasCalledOnContext() {
            _ctx
               .ContextMock
               .Verify(
                  x => x.NotifyValidating(It.IsAny<ValidationArgs>()),
                  Times.Once()
               );
         }

         private void SetupValidPropertyValidationCallback() {
            _ctx
              .ContextMock
              .Setup(x => x.NotifyValidating(It.IsAny<ValidationArgs>()))
              .Callback<ValidationArgs>(args => { });
         }

         private void SetupInvalidPropertyValidationCallback(ValidationError expectedError = null) {
            expectedError = expectedError ?? new ValidationError("Test");

            _ctx
              .ContextMock
              .Setup(x => x.NotifyValidating(It.IsAny<ValidationArgs>()))
              .Callback<ValidationArgs>(args => args.Errors.Add(expectedError));
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
      }

      [TestClass]
      public class ViewModelBehavior_OnValidatingTests : TestBase {
         private ValidatorSpy _validator;

         private IViewModel _employeeVM;
         private IViewModel _addressVM;
         private IVMProperty _addressProperty;

         private ViewModelValidationBehavior _behavior;

         [TestInitialize]
         public void Setup() {
            _validator = new ValidatorSpy();
            _behavior = new ViewModelValidationBehavior();

            _addressProperty = Mock<IVMProperty>();
            _addressVM = Mock<IViewModel>();

            var mock = new Mock<IViewModel>();
            mock
               .Setup(x => x.GetValue(_addressProperty, ValueStage.PreValidation))
               .Returns(_addressVM);

            _employeeVM = mock.Object;

            var behavior = new ViewModelValidationBehavior();
         }

         [TestMethod]
         public void OnValidating_ViewModelArgs_ArgsArePassedToValidator() {
            var args = CreateViewModelValidationArgs();
            AddViewModelValidatorSpy();
            InvokeOnValidating(withArgs: args);
            Assert.AreSame(args, _validator.Args);
         }

         [TestMethod]
         public void OnValidating_ViewModelArgs_RootViewModelValidatorIsInvoked() {
            AddViewModelValidatorSpy();
            InvokeOnValidating(withArgs: CreateViewModelValidationArgs());
            Assert.AreEqual(1, _validator.InvocationCount);
         }

         [TestMethod]
         public void OnValidating_ViewModelArgs_RootPropertyValidatorIsNotInvoked() {
            AddPropertyValidatorSpy(Mock<IVMProperty>());
            InvokeOnValidating(withArgs: CreateViewModelValidationArgs());
            Assert.AreEqual(0, _validator.InvocationCount);
         }

         [TestMethod]
         public void OnValidating_PropertyArgs_RootPropertyValidatorIsInvoked() {
            var targetProperty = Mock<IVMProperty>();
            AddPropertyValidatorSpy(forProperty: targetProperty);
            InvokeOnValidating(withArgs: CreatePropertyValidationArgs(targetProperty));
            Assert.AreEqual(1, _validator.InvocationCount);
         }

         [TestMethod]
         public void OnValidating_PropertyArgs_RootViewModelValidatorIsNotInvoked() {
            AddViewModelValidatorSpy();
            InvokeOnValidating(withArgs: CreatePropertyValidationArgs(Mock<IVMProperty>()));
            Assert.AreEqual(0, _validator.InvocationCount);
         }


         [TestMethod]
         public void OnValidating_ChildValidator_GetsCalled() {
            AddViewModelValidatorSpy(new VMPropertyPath(_addressProperty));

            InvokeOnValidating(
               withArgs: CreateViewModelValidationArgs(
                  changedVM: _addressVM
               )
               .PrependTargetPath(_employeeVM)
            );

            Assert.AreEqual(1, _validator.InvocationCount);
         }

         [TestMethod]
         public void OnValidating_ChildValidator_GetsNotCalled() {
            AddViewModelValidatorSpy(new VMPropertyPath(_addressProperty));

            var anotherVM = Mock<IViewModel>();

            InvokeOnValidating(
               withArgs: CreateViewModelValidationArgs(
                  changedVM: anotherVM
               )
               .PrependTargetPath(_employeeVM)
            );

            Assert.AreEqual(0, _validator.InvocationCount);
         }

         private static ValidationArgs CreateViewModelValidationArgs(IViewModel changedVM = null) {
            return new ValidationArgs(
               validationState: new ValidationState(),
               changedPath: new InstancePath(changedVM ?? Mock<IViewModel>())
            );
         }

         private static ValidationArgs CreatePropertyValidationArgs(IVMProperty targetProperty, IViewModel changedVM = null) {
            return new ValidationArgs(
               validationState: new ValidationState(),
               changedPath: new InstancePath(changedVM ?? Mock<IViewModel>()),
               changedProperty: targetProperty,
               targetProperty: targetProperty
            );
         }

         private void AddViewModelValidatorSpy(VMPropertyPath path = null) {
            _behavior.AddValidator(path ?? VMPropertyPath.Empty, null, _validator);
         }

         private void AddPropertyValidatorSpy(IVMProperty forProperty, VMPropertyPath path = null) {
            _behavior.AddValidator(path ?? VMPropertyPath.Empty, forProperty, _validator);
         }

         private void InvokeOnValidating(ValidationArgs withArgs) {
            _behavior.OnValidating(Mock<IBehaviorContext>(), withArgs);
         }


         private class ValidatorSpy : Validator {
            public int InvocationCount { get; private set; }
            public ValidationArgs Args { get; private set; }

            public override void Validate(ValidationArgs args) {
               InvocationCount++;
               Args = args;
            }
         }
      }

      [TestMethod]
      public void OnChanged_InvokesNotifyValidatingOnContext() {
         var ctx = new ViewModelBehaviorContextHelper();

         var targetVM = Mock<IViewModel>();
         var changedVM = Mock<IViewModel>();
         var changedProperty = Mock<IVMProperty>();

         var behavior = new ViewModelValidationBehavior();
         behavior.Initialize(ctx.InitializationContext);

         var changeArgs = new ChangeArgs(
            ChangeType.PropertyChanged,
            changedVM,
            changedProperty
         );

         var changedPath = new InstancePath(targetVM, changedVM);

         behavior.OnChanged(ctx.Context, changeArgs, changedPath);

         ctx
            .ContextMock
            .Verify(x =>
               x.NotifyValidating(
                  It.Is<ValidationArgs>(args =>
                     args.ChangedPath.RootVM == changedPath.RootVM &&
                     args.ChangedPath.LeafVM == changedPath.LeafVM &&
                     args.ChangedProperty == changedProperty &&
                     args.TargetVM == targetVM &&
                     args.TargetProperty == null
                  )
               ),
               Times.Once()
            );
      }
   }
}