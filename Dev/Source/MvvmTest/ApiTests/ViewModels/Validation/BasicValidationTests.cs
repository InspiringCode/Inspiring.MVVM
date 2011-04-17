namespace Inspiring.MvvmTest.ApiTests.ViewModels.Validation {
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class BasicValidationTests : ValidationTestBase {
      private const string ErrorMessage = "Test";

      private ValidationError NamePropertyValidationError { get; set; }
      private ValidationError ViewModelValidationError { get; set; }

      private EmployeeVM VM { get; set; }

      [TestInitialize]
      public void Setup() {
         VM = new EmployeeVM();

         NamePropertyValidationError = Error("Property error").For(VM, x => x.Name);
         ViewModelValidationError = Error("View model error").For(VM);
      }

      [TestMethod]
      public void SetValue_PropertyValidationSucceeds_PerformsPropertyValidation() {
         VM.ExpectedNameError = NamePropertyValidationError;
         VM.SetValue(x => x.Name, ArbitraryString);
         VM.ExpectedNameError = null;
         VM.SetValue(x => x.Name, ArbitraryString);

         Assert.IsTrue(VM.IsValid);
      }

      [TestMethod]
      public void SetValue_PropertyValidationFails_PerformsPropertyValidation() {
         VM.ExpectedNameError = NamePropertyValidationError;
         VM.SetValue(x => x.Name, ArbitraryString);

         ValidationAssert.Errors(NamePropertyValidationError);
      }

      [TestMethod]
      public void SetValue_ViewModelValidationFails_PerformsViewModelValidation() {
         VM.ExpectedViewModelError = ViewModelValidationError;
         VM.SetValue(x => x.Name, ArbitraryString);

         ValidationAssert.Errors(ViewModelValidationError);
      }

      [TestMethod]
      public void SetValue_PropertyAndViewModelValidationFail_ValidationResultContainsBothErrors() {
         VM.ExpectedNameError = NamePropertyValidationError;
         VM.ExpectedViewModelError = ViewModelValidationError;
         VM.SetValue(x => x.Name, ArbitraryString);

         ValidationAssert.Errors(NamePropertyValidationError, ViewModelValidationError);
      }

      private class EmployeeVM : ViewModel<EmployeeVMDescriptor> {
         public static EmployeeVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<EmployeeVMDescriptor>()
            .For<EmployeeVM>()
            .WithProperties((d, c) => {
               var vm = c.GetPropertyBuilder();

               d.Name = vm.Property.Of<string>();
               d.SelectedProject = vm.VM.Of<ProjectVM>();
            })
            .WithValidators(c => {
               c.Check(x => x.Name).Custom(args => {
                  var error = args.Owner.ExpectedNameError;

                  if (error != null) {
                     args.AddError(error.Message);
                  }
               });

               c.CheckViewModel(args => {
                  var error = args.Owner.ExpectedViewModelError;

                  if (error != null) {
                     args.AddError(error.Message);
                  }
               });
            })
            .Build();

         public EmployeeVM()
            : base(ClassDescriptor) {
         }

         public ValidationError ExpectedNameError { get; set; }
         public ValidationError ExpectedSelectedProjectError { get; set; } // TODO: Remove test case?
         public ValidationError ExpectedViewModelError { get; set; }

         public override string ToString() {
            return "EmployeeVM";
         }
      }

      private class EmployeeVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<string> Name { get; set; }
         public IVMPropertyDescriptor<ProjectVM> SelectedProject { get; set; }
      }

      private class ProjectVM : ViewModel<ProjectVMDescriptor> {
         public static readonly ProjectVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<ProjectVMDescriptor>()
            .For<ProjectVM>()
            .WithProperties((d, b) => { })
            .Build();

         public ProjectVM()
            : base(ClassDescriptor) {
         }
      }

      private class ProjectVMDescriptor : VMDescriptor { }
   }
}