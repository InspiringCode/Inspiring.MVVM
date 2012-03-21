namespace Inspiring.MvvmTest.ApiTests.ViewModels.Validation {
   using System;
   using System.Linq;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;
   using System.Collections.Generic;

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

      [TestMethod]
      public void SetValueToDifferentInvalidValue_TriggersChangeNotification() {
         bool addError = false;

         var vm = EmployeeVM.Create(b => b
            .Check(x => x.Name)
            .Custom(args => {
               if (addError) {
                  args.AddError("Test error");
               }
            })
         );

         vm.SetValue(x => x.Name, "old value");
         Assert.AreEqual(1, vm.OnChangeInvocations.Count);

         addError = true;

         vm.SetValue(x => x.Name, "old value");
         Assert.AreEqual(2, vm.OnChangeInvocations.Count);

         vm.SetValue(x => x.Name, "new value");
         Assert.AreEqual(3, vm.OnChangeInvocations.Count);
      }

      [TestMethod]
      public void SetValue_RaisesAppropriateChangeNotifications() {
         ValueStage noChangeNotifciation = null;

         EmployeeVM vm = EmployeeVM.Create(b => {
            b.Check(x => x.NumericProperty).ValueInRange(min: 1, max: 10);
         });

         Action<int, int, int, ValueStage> testCode = (
            oldValue,
            oldSourceValue,
            newValue,
            expectedChangeNotification
         ) => {
            vm.SetValue(x => x.NumericProperty, oldSourceValue);
            vm.SetValue(x => x.NumericProperty, oldValue);
            vm.OnChangeInvocations.Clear();

            vm.SetValue(x => x.NumericProperty, newValue);

            IEnumerable<ChangeArgs> args = vm
               .OnChangeInvocations
               .Where(x => x.ChangeType == ChangeType.PropertyChanged);

            if (expectedChangeNotification != null) {
               Assert.AreEqual(1, args.Count(), "Expected a single change notification.");

               ChangeArgs arg = args.Single();
               Assert.AreEqual(expectedChangeNotification, arg.Stage);
            } else {
               Assert.AreEqual(0, args.Count(), "Expected no change notification.");
            }
         };

         // Valid values: { 5, 6 }, invalid values: { 77, 78 }
         ParameterizedTest
            // oldValue | oldSourceValue | newValue | expectedChange
            .TestCase(5, 5, 5, noChangeNotifciation)
            .TestCase(5, 5, 6, ValueStage.ValidatedValue)
            .TestCase(5, 5, 77, ValueStage.Value)
            .TestCase(77, 5, 77, noChangeNotifciation)
            .TestCase(77, 5, 78, ValueStage.Value)
            .TestCase(77, 5, 5, ValueStage.Value)
            .TestCase(77, 5, 6, ValueStage.ValidatedValue)
            .Run(testCode);
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

      private class EmployeeVM : TestViewModel<EmployeeVMDescriptor> {
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
                  d.NumericProperty = vm.Property.Of<int>();
               })
               .WithValidators(validationConfigurationAction)
               .Build();

            return new EmployeeVM(descriptor);
         }

         public int NumericProperty {
            get { return GetValue(Descriptor.NumericProperty); }
         }

         public override string ToString() {
            return "EmployeeVM";
         }
      }

      private class EmployeeVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<int> NumericProperty { get; set; }
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