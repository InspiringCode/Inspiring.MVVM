namespace Inspiring.MvvmTest.ApiTests.ViewModels.Validation {
   using System;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class BasicValidationTests : ValidationTestBase {
      private const string ExpectedNamePropertyError = "Name property error";
      private const string ExpectedViewModelError = "View model error";

      private string NamePropertyErrorToReturn { get; set; }
      private string ViewModelErrorToReturn { get; set; }

      [TestMethod]
      public void SetValue_PropertyValidationSucceeds_RemovesPreviousErrorFromProperty() {
         var vm = EmployeeVM.Create(WithPropertyAndViewModelValidation);

         NamePropertyErrorToReturn = ExpectedNamePropertyError;
         vm.SetValue(x => x.Name, "Value 1");

         NamePropertyErrorToReturn = null;
         vm.SetValue(x => x.Name, "Value 2");

         Assert.IsTrue(vm.IsValid);
      }

      [TestMethod]
      public void SetValue_PropertyValidationFails_AddsValidationErrorToProperty() {
         var vm = EmployeeVM.Create(WithPropertyAndViewModelValidation);

         NamePropertyErrorToReturn = ExpectedNamePropertyError;
         vm.SetValue(x => x.Name, "New value");

         ValidationAssert.ErrorMessages(vm.ValidationResult, ExpectedNamePropertyError);
      }

      [TestMethod]
      public void SetValue_ViewModelValidationFails_AddsValidationErrorToViewModel() {
         var vm = EmployeeVM.Create(WithPropertyAndViewModelValidation);

         ViewModelErrorToReturn = ExpectedViewModelError;
         vm.SetValue(x => x.Name, "New value");

         ValidationAssert.ErrorMessages(vm.ValidationResult, ExpectedViewModelError);
      }

      [TestMethod]
      public void SetValue_PropertyAndViewModelValidationFail_ValidationResultContainsBothErrors() {
         var vm = EmployeeVM.Create(WithPropertyAndViewModelValidation);

         NamePropertyErrorToReturn = ExpectedNamePropertyError;
         ViewModelErrorToReturn = ExpectedViewModelError;
         vm.SetValue(x => x.Name, "New value");

         ValidationAssert.ErrorMessages(vm.ValidationResult, ExpectedNamePropertyError, ExpectedViewModelError);
      }

      [TestMethod]
      public void PropertyValidationDefinedOnParent_IsPerformedIfChildOnlyCallsEnableParentValidation() {
         var error = "Parent added error";

         var parent = EmployeeVM.Create(b => {
            b.ValidateDescendant(x => x.SelectedProject)
               .Check(x => x.Name)
               .Custom(args => args.AddError(error));
         });

         var child = ProjectVM.Create(b => {
            b.EnableParentValidation(x => x.Name);
         });

         parent.SetValue(x => x.SelectedProject, child);

         parent.Revalidate(ValidationScope.SelfAndAllDescendants);
         ValidationAssert.ErrorMessages(child.ValidationResult, error);
      }

      [TestMethod]
      public void ViewModelValidationDefinedOnParent_IsPerformedIfChildOnlyCallsEnableParentValidation() {
         var error = "Parent added error";

         var parent = EmployeeVM.Create(b => {
            b.ValidateDescendant(x => x.SelectedProject)
               .CheckViewModel(args => args.AddError(error));
         });

         var child = ProjectVM.Create(b => {
            b.EnableParentViewModelValidation();
         });

         parent.SetValue(x => x.SelectedProject, child);

         parent.Revalidate(ValidationScope.SelfAndAllDescendants);
         ValidationAssert.ErrorMessages(child.ValidationResult, error);
      }

      private void WithPropertyAndViewModelValidation(
         RootValidatorBuilder<EmployeeVM, EmployeeVM, EmployeeVMDescriptor> builder
      ) {
         builder.Check(x => x.Name).Custom(args => {
            if (NamePropertyErrorToReturn != null) {
               args.AddError(NamePropertyErrorToReturn);
            }
         });

         builder.CheckViewModel(args => {
            if (ViewModelErrorToReturn != null) {
               args.AddError(ViewModelErrorToReturn);
            }
         });
      }

      private class EmployeeVM : ViewModel<EmployeeVMDescriptor> {
         private EmployeeVM(EmployeeVMDescriptor descriptor)
            : base(descriptor) {
         }

         public static EmployeeVM Create(
            Action<RootValidatorBuilder<EmployeeVM, EmployeeVM, EmployeeVMDescriptor>> validationConfigurationAction
         ) {
            var descriptor = VMDescriptorBuilder
               .OfType<EmployeeVMDescriptor>()
               .For<EmployeeVM>()
               .WithProperties((d, c) => {
                  var vm = c.GetPropertyBuilder();

                  d.Name = vm.Property.Of<string>();
                  d.SelectedProject = vm.VM.Of<ProjectVM>();
               })
               .WithValidators(validationConfigurationAction)
               .Build();

            return new EmployeeVM(descriptor);
         }

         public override string ToString() {
            return "EmployeeVM";
         }
      }

      private class EmployeeVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<string> Name { get; set; }
         public IVMPropertyDescriptor<ProjectVM> SelectedProject { get; set; }
      }

      private class ProjectVM : ViewModel<ProjectVMDescriptor> {
         private ProjectVM(ProjectVMDescriptor descriptor)
            : base(descriptor) {
         }

         public static ProjectVM Create(
            Action<RootValidatorBuilder<ProjectVM, ProjectVM, ProjectVMDescriptor>> validatorConfigurationAction
         ) {
            var descriptor = VMDescriptorBuilder
               .OfType<ProjectVMDescriptor>()
               .For<ProjectVM>()
               .WithProperties((d, b) => {
                  var v = b.GetPropertyBuilder();
                  d.Name = v.Property.Of<string>();
               })
               .WithValidators(validatorConfigurationAction)
               .Build();

            return new ProjectVM(descriptor);
         }
      }

      private class ProjectVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<string> Name { get; set; }
      }
   }
}