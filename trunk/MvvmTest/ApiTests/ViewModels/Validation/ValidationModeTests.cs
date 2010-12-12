namespace Inspiring.MvvmTest.ApiTests.ViewModels.Validation {
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class ValidationModeTests {
      private const string OriginalValue = "Original value";
      private const string NewValue = "New value";
      private const string ErrorText = "Validation error";

      private TaskVM VM { get; set; }

      [TestInitialize]
      public void Setup() {
         VM = new TaskVM();
         VM.ReturnValidationSuccess();
         VM.TitleDisplayValue = OriginalValue;
      }

      [TestMethod]
      public void RevalidateCommitMode_ValidationSucceeds_UpdatesSourceValue() {
         VM.ReturnValidationError();
         VM.TitleDisplayValue = NewValue;
         VM.ReturnValidationSuccess();

         VM.RevalidateCommit();

         Assert.AreEqual(NewValue, VM.TitleSourceValue);
      }

      [TestMethod]
      public void RevalidateCommitMode_ValidationSucceeds_UpdatesValidationState() {
         VM.ReturnValidationError();
         VM.TitleDisplayValue = NewValue;
         VM.ReturnValidationSuccess();

         VM.RevalidateCommit();

         DomainAssert.AreEqual(ValidationState.Valid, VM.TitleValidationState);
      }

      [TestMethod]
      public void RevalidateCommitMode_ValidationSucceeds_RaisesPropertyChanged() {
         VM.ReturnValidationError();
         VM.TitleDisplayValue = NewValue;
         VM.ReturnValidationSuccess();

         var listener = CreatePropertyChangedListener();
         VM.RevalidateCommit();
         listener.AssertOneRaise();
      }

      [TestMethod]
      public void RevalidateCommitMode_ValidationFails_DoesNotUpdateSourceValue() {
         VM.ReturnValidationError();
         VM.TitleDisplayValue = NewValue;

         VM.RevalidateCommit();

         Assert.AreEqual(OriginalValue, VM.TitleSourceValue);
      }

      [TestMethod]
      public void RevalidateCommitMode_ValidationFails_UpdatesValidationState() {
         VM.ReturnValidationError();
         VM.TitleDisplayValue = NewValue;

         DomainAssert.AreEqual(CreateInvalidValidationState(), VM.TitleValidationState);
      }

      [TestMethod]
      public void RevalidateCommitMode_ValidationFails_DoesNotRaisePropertyChanged() {
         VM.ReturnValidationError();
         VM.TitleDisplayValue = NewValue;

         var listener = CreatePropertyChangedListener();
         VM.RevalidateCommit();
         listener.AssertNoRaise();
      }

      [TestMethod]
      public void RevalidateDiscardMode_ValidationSucceeds_UpdatesDisplayValue() {
         VM.ReturnValidationError();
         VM.TitleDisplayValue = NewValue;
         VM.ReturnValidationSuccess();

         VM.RevalidateDiscard();
         Assert.AreEqual(OriginalValue, VM.TitleDisplayValue);
      }

      [TestMethod]
      public void RevalidateDiscardMode_ValidationSucceeds_UpdatesValidationState() {
         VM.ReturnValidationError();
         VM.TitleDisplayValue = NewValue;
         VM.ReturnValidationSuccess();

         VM.RevalidateDiscard();
         Assert.AreEqual(ValidationState.Valid, VM.TitleValidationState);
      }

      [TestMethod]
      public void RevalidateDiscardMode_ValidationFails_UpdatesDisplayValue() {
         VM.ReturnValidationError();
         VM.TitleDisplayValue = NewValue;

         VM.RevalidateDiscard();
         Assert.AreEqual(OriginalValue, VM.TitleDisplayValue);
      }

      [TestMethod]
      public void RevalidateDiscardMode_ValidationFails_UpdatesValidationState() {
         VM.ReturnValidationError();
         VM.TitleDisplayValue = NewValue;

         VM.RevalidateDiscard();
         Assert.AreEqual(OriginalValue, VM.TitleDisplayValue);
      }

      [TestMethod]
      public void RevalidateDiscardMode_ValueChanges_PropertyChangedIsRaised() {
         VM.ReturnValidationError();
         VM.TitleDisplayValue = NewValue;
         VM.ReturnValidationSuccess();


      }

      [TestMethod]
      public void RevalidateDiscardMode_ValueDoesNotChange_PropertyChangedIsNotRaised() {
         Assert.Inconclusive("Does this make sense?");
      }

      private PropertyChangedCounter CreatePropertyChangedListener() {
         return new PropertyChangedCounter(VM, "Title");
      }

      private ValidationState CreateInvalidValidationState() {
         var state = new ValidationState();
         state.Errors.Add(new ValidationError(ErrorText));
         return state;
      }

      public sealed class TaskVM : ViewModel<TaskVMDescriptor> {
         public static readonly TaskVMDescriptor Descriptor = VMDescriptorBuilder
            .For<TaskVM>()
            .CreateDescriptor(c => {
               var v = c.GetPropertyFactory();

               return new TaskVMDescriptor {
                  Title = v.Local.Property<string>()
               };
            })
            .WithValidations((d, c) => {
               c.Check(x => x.Title).Custom((vm, val, args) => {
                  if (vm.ReturnError) {
                     args.Errors.Add(new ValidationError(ErrorText));
                  }
               });
            })
            .Build();

         public TaskVM()
            : base(Descriptor) {
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

         private bool ReturnError { get; set; }

         public void ReturnValidationError() {
            ReturnError = true;
         }

         public void ReturnValidationSuccess() {
            ReturnError = false;
         }

         public void RevalidateCommit() {
            Revalidate(ValidationScope.SelfOnly, ValidationMode.CommitValidValues);
         }

         public void RevalidateDiscard() {
            Revalidate(ValidationScope.SelfOnly, ValidationMode.DiscardInvalidValues);
         }
      }

      public sealed class TaskVMDescriptor : VMDescriptor {
         public VMProperty<string> Title { get; set; }
      }
   }
}