namespace Inspiring.MvvmTest.ApiTests.ViewModels.Validation {
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class RevalidateTests : ValidationTestBase {
      private ValidationResult InvalidValidationResult { get; set; }

      private TestVM VM { get; set; }

      private string PropertySourceValue {
         get { return VM.GetValidatedValue(TestVM.ClassDescriptor.Property); }
      }

      [TestInitialize]
      public void Setup() {
         VM = new TestVM();
         InvalidValidationResult = new ValidationResult(
            CreateValidationError("Invalid validation result", VM, TestVM.ClassDescriptor.Property)
         );
      }

      [TestMethod]
      public void Revalidate_ValidationSucceeds_UpdatesSourceValue() {
         var previouslyInvalidValue = "Previously invalid value";

         SetPropertyToValidValue();
         SetPropertyToInvalidValue(previouslyInvalidValue);
         VM.PropertyResultToReturn = ValidationResult.Valid;

         RevalidateProperty();
         Assert.AreEqual(previouslyInvalidValue, PropertySourceValue);
      }

      [TestMethod]
      public void Revalidate_ValidationSucceeds_UpdatesValidationState() {
         SetPropertyToValidValue();
         SetPropertyToInvalidValue();
         VM.PropertyResultToReturn = ValidationResult.Valid;

         RevalidateProperty();
         ValidationAssert.IsValid(VM.ValidationResult);
      }

      [TestMethod]
      public void Revalidate_ValidationSucceeds_RaisesValidationStateChangedAndPropertyChanged() {
         SetPropertyToValidValue();
         SetPropertyToInvalidValue();
         VM.NotifyChangeInvocations.Clear();

         VM.PropertyResultToReturn = ValidationResult.Valid;
         RevalidateProperty();

         var expectedChangeNotifications = new[] {
            ChangeArgs
               .PropertyChanged(TestVM.ClassDescriptor.Property)
               .PrependViewModel(VM),
            ChangeArgs
               .ValidationResultChanged(TestVM.ClassDescriptor.Property)
               .PrependViewModel(VM)
         };

         DomainAssert.AreEqual(expectedChangeNotifications, VM.NotifyChangeInvocations);
      }

      [TestMethod]
      public void Revalidate_ValidationFails_DoesNotUpdateSourceValue() {
         var lastValidValue = "Last valid value";

         SetPropertyToValidValue(lastValidValue);
         SetPropertyToInvalidValue();

         VM.PropertyResultToReturn = InvalidValidationResult;
         RevalidateProperty();

         Assert.AreEqual(lastValidValue, PropertySourceValue);
      }

      [TestMethod]
      public void Revalidate_ValidationFails_UpdatesValidationState() {
         SetPropertyToValidValue();
         SetPropertyToInvalidValue();
         VM.PropertyResultToReturn = InvalidValidationResult;

         RevalidateProperty();
         ValidationAssert.AreEqual(InvalidValidationResult, VM.ValidationResult);
      }

      [TestMethod]
      public void Revalidate_ValidationFails_RaisesOnlyValidationStateChanged() {
         SetPropertyToValidValue();
         SetPropertyToInvalidValue();
         VM.NotifyChangeInvocations.Clear();

         VM.PropertyResultToReturn = InvalidValidationResult;
         RevalidateProperty();

         var expectedChangeNotifications = new[] {
            ChangeArgs
               .ValidationResultChanged(TestVM.ClassDescriptor.Property)
               .PrependViewModel(VM)
         };

         DomainAssert.AreEqual(expectedChangeNotifications, VM.NotifyChangeInvocations);
      }

      private void SetPropertyToInvalidValue(string value = "Default invalid value") {
         VM.PropertyResultToReturn = CreateValidationResult("Temporary error");
         VM.SetValue(x => x.Property, value);
      }

      private void SetPropertyToValidValue(string value = "Default valid value") {
         VM.PropertyResultToReturn = ValidationResult.Valid;
         VM.SetValue(x => x.Property, value);
      }

      private void RevalidateProperty() {
         VM.Revalidate(x => x.Property);
      }

      public sealed class TestVM : TestViewModel<TestVMDescriptor> {
         public static readonly TestVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<TestVMDescriptor>()
            .For<TestVM>()
            .WithProperties((d, b) => {
               var v = b.GetPropertyBuilder();
               d.Property = v.Property.Of<string>();
            })
            .WithValidators(b => {
               b.Check(x => x.Property).Custom(args => {
                  args
                     .Owner
                     .PropertyResultToReturn
                     .Errors
                     .ForEach(e => args.AddError(e.Message, e.Details));
               });
            })
            .Build();

         public TestVM()
            : base(ClassDescriptor) {
            PropertyResultToReturn = ValidationResult.Valid;
         }

         public ValidationResult PropertyResultToReturn { get; set; }
      }

      public sealed class TestVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<string> Property { get; set; }
      }
   }
}