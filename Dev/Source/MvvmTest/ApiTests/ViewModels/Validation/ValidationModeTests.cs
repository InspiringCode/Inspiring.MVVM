namespace Inspiring.MvvmTest.ApiTests.ViewModels.Validation {
   using System.Collections.Generic;
   using System.Linq;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Inspiring.MvvmTest.ViewModels;
   using Microsoft.VisualStudio.TestTools.UnitTesting;
   [TestClass]
   public class ValidationModeTests : ValidationTestBase {

      [TestClass]
      public abstract class AbstractValidationModeTests : TestBase {
         protected TaskVM VM { get; set; }

         protected abstract object NewValue { get; }

         protected abstract object OriginalValue { get; }

         protected abstract object SourceValue { get; set; }

         protected abstract object DisplayValue { get; set; }

         protected abstract ValidationState ValidationState { get; }

         protected abstract string PropertyName { get; }

         private static ValidationState InvalidValidationState {
            get { return CreateValidationState("Validation error"); }
         }

         [TestInitialize]
         public void Setup() {
            VM = new TaskVM();
         }

         [TestMethod]
         public void RevalidateCommitMode_ValidationSucceeds_UpdatesSourceValue() {
            SetPropertyToValidValue(OriginalValue);
            SetPropertyToInvalidValue(NewValue);
            SetupValidatorToReturnSuccess();

            RevalidateCommit();

            Assert.AreEqual(NewValue, SourceValue);
         }

         [TestMethod]
         public void RevalidateCommitMode_ValidationSucceeds_UpdatesValidationState() {
            SetPropertyToValidValue(OriginalValue);
            SetPropertyToInvalidValue(NewValue);
            SetupValidatorToReturnSuccess();

            RevalidateCommit();

            DomainAssert.AreEqual(ValidationState.Valid, ValidationState);
         }

         [TestMethod]
         public void RevalidateCommitMode_ValidationSucceeds_RaisesPropertyChanged() {
            SetPropertyToValidValue(OriginalValue);
            SetPropertyToInvalidValue(NewValue);
            SetupValidatorToReturnSuccess();

            VM.PropertyChangedLog.Clear();
            RevalidateCommit();
            int propertyChangedCount = VM.PropertyChangedLog.Count(x => x == PropertyName);
            Assert.AreEqual(1, propertyChangedCount);
         }

         [TestMethod]
         public void RevalidateCommitMode_ValidationFails_DoesNotUpdateSourceValue() {
            SetPropertyToValidValue(OriginalValue);
            SetPropertyToInvalidValue(NewValue);
            SetupValidatorToReturnError();

            RevalidateCommit();

            Assert.AreEqual(OriginalValue, SourceValue);
         }

         [TestMethod]
         public void RevalidateCommitMode_ValidationFails_UpdatesValidationState() {
            SetPropertyToValidValue(OriginalValue);
            SetPropertyToInvalidValue(NewValue);
            SetupValidatorToReturnError();

            RevalidateCommit();

            DomainAssert.AreEqual(InvalidValidationState, ValidationState);
         }

         [TestMethod]
         public void RevalidateCommitMode_ValidationFails_DoesNotRaisePropertyChanged() {
            SetPropertyToValidValue(OriginalValue);
            SetPropertyToInvalidValue(NewValue);
            SetupValidatorToReturnError();

            var listener = CreatePropertyChangedListener();
            RevalidateCommit();
            listener.AssertNoRaise();
         }

         [TestMethod]
         public void RevalidateDiscardMode_ValidationSucceeds_UpdatesDisplayValue() {
            SetPropertyToValidValue(OriginalValue);
            SetPropertyToInvalidValue(NewValue);
            SetupValidatorToReturnSuccess();

            RevalidateDiscard();

            Assert.AreEqual(OriginalValue, DisplayValue);
         }

         [TestMethod]
         public void RevalidateDiscardMode_ValidationSucceeds_UpdatesValidationState() {
            SetPropertyToValidValue(OriginalValue);
            SetPropertyToInvalidValue(NewValue);
            SetupValidatorToReturnSuccess();

            RevalidateDiscard();

            Assert.AreEqual(ValidationState.Valid, ValidationState);
         }

         [TestMethod]
         public void RevalidateDiscardMode_ValidationFails_UpdatesDisplayValue() {
            SetPropertyToValidValue(OriginalValue);
            SetPropertyToInvalidValue(NewValue);
            SetupValidatorToReturnError();

            RevalidateDiscard();

            Assert.AreEqual(OriginalValue, DisplayValue);
         }

         [TestMethod]
         public void RevalidateDiscardMode_ValidationFails_UpdatesValidationState() {
            SetPropertyToValidValue(OriginalValue);
            SetPropertyToInvalidValue(NewValue);
            SetupValidatorToReturnError();

            RevalidateDiscard();

            DomainAssert.AreEqual(InvalidValidationState, ValidationState);
         }

         [TestMethod]
         public void RevalidateDiscardMode_ValueChanges_PropertyChangedIsRaised() {
            SetPropertyToValidValue(OriginalValue);
            SetPropertyToInvalidValue(NewValue);
            SetupValidatorToReturnSuccess();

            VM.PropertyChangedLog.Clear();
            RevalidateDiscard();
            int propertyChangedCount = VM.PropertyChangedLog.Count(x => x == PropertyName);
            Assert.AreEqual(1, propertyChangedCount);
         }

         [TestMethod]
         public void RevalidateDiscardMode_ValueDoesNotChange_PropertyChangedIsNotRaised() {
            SetPropertyToValidValue(OriginalValue);

            var listener = CreatePropertyChangedListener();
            RevalidateDiscard();
            listener.AssertNoRaise();
         }

         private void SetPropertyToInvalidValue(object invalidValue) {
            SetupValidatorToReturnError();
            DisplayValue = invalidValue;
         }

         private void SetPropertyToValidValue(object validValue) {
            SetupValidatorToReturnSuccess();
            DisplayValue = validValue;
         }

         private void SetupValidatorToReturnSuccess() {
            VM.ReturnError = false;
         }

         private void SetupValidatorToReturnError() {
            VM.ReturnError = true;
         }

         private void RevalidateDiscard() {
            VM.Revalidate(ValidationMode.DiscardInvalidValues);
         }

         private void RevalidateCommit() {
            VM.Revalidate(ValidationMode.CommitValidValues);
         }

         private PropertyChangedCounter CreatePropertyChangedListener() {
            return new PropertyChangedCounter(VM, PropertyName);
         }

         public sealed class TaskVM : ViewModel<TaskVMDescriptor> {
            public static readonly TaskVMDescriptor ClassDescriptor = VMDescriptorBuilder
               .OfType<TaskVMDescriptor>()
               .For<TaskVM>()
               .WithProperties((d, c) => {
                  var v = c.GetPropertyBuilder();

                  d.Title = v.Property.Of<string>();
                  d.State = v.VM.Of<StateVM>();
               })
               .WithValidators(c => {
                  c.Check(x => x.Title).Custom((vm, val, args) => {
                     if (vm.ReturnError) {
                        args.AddError(InvalidValidationState.Errors.Single().Message);
                     }
                  });
                  c.Check(x => x.State).Custom((vm, val, args) => {
                     if (vm.ReturnError) {
                        args.AddError(InvalidValidationState.Errors.Single().Message);
                     }
                  });
               })
               .Build();

            public TaskVM()
               : base(ClassDescriptor) {
               PropertyChangedLog = new List<string>();
            }

            public object TitleDisplayValue {
               get { return GetDisplayValue(Descriptor.Title); }
               set { SetDisplayValue(Descriptor.Title, value); }
            }

            public string TitleSourceValue {
               get { return GetValidatedValue(Descriptor.Title); }
               set { SetValue(Descriptor.Title, value); }
            }

            public ValidationState TitleValidationState {
               get { return Kernel.GetValidationState(Descriptor.Title); }
            }

            public object StateDisplayValue {
               get { return GetDisplayValue(Descriptor.State); }
               set { SetDisplayValue(Descriptor.State, value); }
            }

            public StateVM StateSourceValue {
               get { return GetValidatedValue(Descriptor.State); }
               set { SetValue(Descriptor.State, value); }
            }

            public ValidationState StateValidationState {
               get { return Kernel.GetValidationState(Descriptor.State); }
            }

            public bool ReturnError { get; set; }

            public List<string> PropertyChangedLog { get; set; }

            public void Revalidate(ValidationMode mode) {
               Revalidate(ValidationScope.SelfOnly, mode);
            }

            protected override void OnPropertyChanged(IVMPropertyDescriptor property) {
               base.OnPropertyChanged(property);
               PropertyChangedLog.Add(property.PropertyName);
            }
         }

         public sealed class TaskVMDescriptor : VMDescriptor {
            public IVMPropertyDescriptor<string> Title { get; set; }
            public IVMPropertyDescriptor<StateVM> State { get; set; }
         }
      }

      [TestClass]
      public class PropertyValidationModeTests : AbstractValidationModeTests {
         protected override object OriginalValue {
            get { return "Original Value"; }
         }

         protected override object NewValue {
            get { return "New Value"; }
         }

         protected override object SourceValue {
            get { return VM.TitleSourceValue; }
            set { VM.TitleSourceValue = (string)value; }
         }

         protected override object DisplayValue {
            get { return VM.TitleDisplayValue; }
            set { VM.TitleDisplayValue = value; }
         }

         protected override ValidationState ValidationState {
            get { return VM.TitleValidationState; }
         }

         protected override string PropertyName {
            get { return "Title"; }
         }
      }

      [TestClass]
      public class ViewModelValidationModeTests : AbstractValidationModeTests {
         private readonly StateVM _originalValue = new StateVM("Orignal Value");
         private readonly StateVM _newValue = new StateVM("New Value");

         protected override object OriginalValue {
            get { return _originalValue; }
         }

         protected override object NewValue {
            get { return _newValue; }
         }

         protected override object SourceValue {
            get { return VM.StateSourceValue; }
            set { VM.StateSourceValue = (StateVM)value; }
         }

         protected override object DisplayValue {
            get { return VM.StateDisplayValue; }
            set { VM.StateDisplayValue = value; }
         }

         protected override ValidationState ValidationState {
            get { return VM.StateValidationState; }
         }

         protected override string PropertyName {
            get { return "State"; }
         }
      }
   }
}