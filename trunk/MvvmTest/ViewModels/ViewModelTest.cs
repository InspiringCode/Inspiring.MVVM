namespace Inspiring.MvvmTest.ViewModels {
   using System.ComponentModel;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class ViewModelTest {
      [TestMethod]
      public void RowValidation_WithoutValidations_IsValid() {
         var vm = new EmptyVM(new EmptyVMDescriptor());
         bool isValid = vm.IsValid(true);
         Assert.IsTrue(isValid);
      }

      [TestMethod]
      public void RowValidation_WithPositiveValidation_IsValid() {
         var descriptor = new EmptyVMDescriptor();
         descriptor.GetService<ViewModelValidatorHolder>().AddValidator(args => { });
         var vm = new EmptyVM(descriptor);

         ValidationResult result = vm.InvokeValidateMethod();

         Assert.IsTrue(result.Successful);
      }

      [TestMethod]
      public void RowValidation_WithNegativeValidation_IsNotValid() {
         var descriptor = new EmptyVMDescriptor();
         descriptor.GetService<ViewModelValidatorHolder>().AddValidator(args => {
            args.AddError("Error");
         });
         var vm = new EmptyVM(descriptor);

         ValidationResult result = vm.InvokeValidateMethod();

         Assert.IsFalse(result.Successful);
         Assert.AreEqual("Error", result.ErrorMessage);
      }

      [TestMethod]
      public void RowValidation_ValidationStateChange_RaisesPropertyChanged() {
         // Arrange
         bool returnError = false;
         var descriptor = new EmptyVMDescriptor();

         descriptor.GetService<ViewModelValidatorHolder>().AddValidator(args => {
            if (returnError) {
               args.AddError("Error");
            }
         });

         var vm = new EmptyVM(descriptor);

         var counter = new PropertyChangedCounter(vm, "Error");

         // Act & assert
         vm.InvokeValidateMethod();
         counter.AssertNoRaise(
            "Initial call of Validate for a valid VM sould not raise " +
            "PropertyChanged for Error."
         );

         // Arrange
         returnError = true;
         vm.InvokeValidateMethod();
         counter.AssertOneRaise(
            "Validation should raise PropertyChanged for Error if validation " +
            "state changes from valid to invalid."
         );

         vm.InvokeValidateMethod();
         counter.AssertNoRaise(
            "Validation should not raise PropertyChanged for Error if validation " +
            "state does not change."
         );

         returnError = false;
         vm.InvokeValidateMethod();
         counter.AssertOneRaise(
            "Validation should raise PropertyChanged for Error if validation state " +
            "changes from invalid to valid."
         );
      }



      [TestMethod]
      public void OnValidating() {
         TestVM vm = new TestVM();
         IBehaviorContext context = vm;

         var args = new ValidationEventArgs(TestVM.Descriptor.LocalProperty, 0.0m, vm);

         bool validatingCalled = false;

         vm.Validating += (sender, e) => {
            validatingCalled = true;
            Assert.AreEqual(args, e);
         };

         context.OnValidating(args);
         Assert.IsTrue(validatingCalled);
      }

      [TestMethod]
      public void OnValidated() {
         TestVM vm = new TestVM();
         IBehaviorContext context = vm;

         var args = new ValidationEventArgs(TestVM.Descriptor.LocalProperty, 0.0m, vm);

         bool validatedCalled = false;

         vm.Validated += (sender, e) => {
            validatedCalled = true;
            Assert.AreEqual(args, e);
         };

         context.OnValidated(args);
         Assert.IsTrue(validatedCalled);
      }

      private class EmptyVM : ViewModel<EmptyVMDescriptor> {
         public EmptyVM(EmptyVMDescriptor descriptor)
            : base(descriptor) {
         }

         public ValidationResult InvokeValidateMethod() {
            // TODO HACK !!
            this.Invoke("InvokeValidate", this, null);
            string error = ((IDataErrorInfo)this).Error;
            return error != null ?
               ValidationResult.Failure(error) :
               ValidationResult.Success();
         }
      }

      private class EmptyVMDescriptor : VMDescriptor {
      }
   }
}