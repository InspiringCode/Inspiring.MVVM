namespace Inspiring.MvvmTest.ApiTests.ViewModels.DeclarativeDeendencies {
   using System;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public sealed class DependencyValidationActionTests : DependencyTestBase {

      [TestMethod]
      public void DependencyOnViewModelTargetProperty_RevalidatesProperty() {
         var employeeVM = CreateEmployeeVM(
            b => b
               .OnChangeOf
               .Properties(x => x.Name)
               .Revalidate
               .Properties(x => x.SelectedProject)
         );

         var projectVM = CreateProjectVM();
         employeeVM.SelectedProject = projectVM;

         Results.SetupFailing().PropertyValidation
           .Targeting(employeeVM, x => x.SelectedProject)
           .On(employeeVM);

         employeeVM.Name = "TriggerChange";

         Results.VerifySetupValidationResults();
      }

      [TestMethod]
      public void DependencyOnViewModelTargetDescendant_RevalidatesProperty() {
         var employeeVM = CreateEmployeeVM(
            b => b
               .OnChangeOf
               .Properties(x => x.Name)
               .Revalidate
               .Descendant(x => x.SelectedProject)
         );

         var projectVM = CreateProjectVM();
         employeeVM.SelectedProject = projectVM;

         Results.SetupFailing().PropertyValidation
           .Targeting(projectVM, x => x.Title)
           .On(projectVM);

         employeeVM.Name = "TriggerChange";

         Results.VerifySetupValidationResults();
      }

      [TestMethod]
      public void DependencyOnDescendantCollection_RevalidatesAllViewModelsInCollection() {
         var employeeVM = CreateEmployeeVM(
            b => b
               .OnChangeOf
               .Properties(x => x.Name)
               .Revalidate
               .Descendant(x => x.Projects)
         );

         var projectVM1 = CreateProjectVM();
         var projectVM2 = CreateProjectVM();

         employeeVM.Projects.Add(projectVM1);
         employeeVM.Projects.Add(projectVM2);

         Results.SetupFailing().PropertyValidation
           .Targeting(projectVM1, x => x.Title)
           .On(projectVM1);

         Results.SetupFailing().PropertyValidation
           .Targeting(projectVM2, x => x.Title)
           .On(projectVM2);

         employeeVM.Name = "TriggerChange";

         Results.VerifySetupValidationResults();
      }

      [TestMethod]
      public void DependencyOnDescendantOfDescendantCollection_RevalidatesAllDescendants() {
         var employeeVM = CreateEmployeeVM(
            b => b
               .OnChangeOf
               .Properties(x => x.Name)
               .Revalidate
               .Descendant(x => x.Projects)
               .Descendant(x => x.Customer)
         );

         var projectVM1 = CreateProjectVM();
         var projectVM2 = CreateProjectVM();

         employeeVM.Projects.Add(projectVM1);
         employeeVM.Projects.Add(projectVM2);

         var customerVM1 = CreateCustomerVM();
         var customerVM2 = CreateCustomerVM();

         projectVM1.Customer = customerVM1;
         projectVM2.Customer = customerVM2;

         Results.SetupFailing().PropertyValidation
           .Targeting(customerVM1, x => x.Name)
           .On(customerVM1);

         Results.SetupFailing().PropertyValidation
           .Targeting(customerVM2, x => x.Name)
           .On(customerVM2);

         employeeVM.Name = "TriggerChange";

         Results.VerifySetupValidationResults();
      }

      [TestMethod]
      public void DependencyOnTargetPropertyOfDescendantCollection_RevalidatesAllProperties() {
         var employeeVM = CreateEmployeeVM(
            b => b
               .OnChangeOf
               .Properties(x => x.Name)
               .Revalidate
               .Descendant(x => x.Projects)
               .Properties(x => x.Customer)
         );

         var projectVM1 = CreateProjectVM();
         var projectVM2 = CreateProjectVM();

         employeeVM.Projects.Add(projectVM1);
         employeeVM.Projects.Add(projectVM2);

         var customerVM1 = CreateCustomerVM();
         var customerVM2 = CreateCustomerVM();

         projectVM1.Customer = customerVM1;
         projectVM2.Customer = customerVM2;

         Results.SetupFailing().PropertyValidation
           .Targeting(projectVM1, x => x.Customer)
           .On(projectVM1);

         Results.SetupFailing().PropertyValidation
           .Targeting(projectVM2, x => x.Customer)
           .On(projectVM2);

         employeeVM.Name = "TriggerChange";

         Results.VerifySetupValidationResults();
      }

      [TestMethod]
      public void DependencyOnTargetPropertiesOfDescendantOfDescendantCollection_RevalidatesPropertiesOfAllDescendantsInCollection() {
         var employeeVM = CreateEmployeeVM(
            b => b
               .OnChangeOf
               .Properties(x => x.Name)
               .Revalidate
               .Descendant(x => x.Projects)
               .Descendant(x => x.Customer)
               .Properties(x => x.Name, x => x.Rating)
         );

         var projectVM1 = CreateProjectVM();
         var projectVM2 = CreateProjectVM();

         employeeVM.Projects.Add(projectVM1);
         employeeVM.Projects.Add(projectVM2);

         var customerVM1 = CreateCustomerVM();
         var customerVM2 = CreateCustomerVM();

         projectVM1.Customer = customerVM1;
         projectVM2.Customer = customerVM2;

         Results.SetupFailing().PropertyValidation
           .Targeting(customerVM1, x => x.Name)
           .On(customerVM1);

         Results.SetupFailing().PropertyValidation
           .Targeting(customerVM1, x => x.Rating)
           .On(customerVM1);

         Results.SetupFailing().PropertyValidation
           .Targeting(customerVM2, x => x.Name)
           .On(customerVM2);

         Results.SetupFailing().PropertyValidation
           .Targeting(customerVM2, x => x.Rating)
           .On(customerVM2);

         employeeVM.Name = "TriggerChange";

         Results.VerifySetupValidationResults();
      }

      private EmployeeVM CreateEmployeeVM(
         Action<IVMDependencyBuilder<EmployeeVM, EmployeeVMDescriptor>> dependencyConfigurationAction,
         ProjectVMDescriptor projectVMDescriptor = null
      ) {
         projectVMDescriptor = projectVMDescriptor ?? ProjectVM.CreateDescriptor(null, false);
         return new EmployeeVM(dependencyConfigurationAction, projectVMDescriptor, true, Results, new RefreshControllerBehaviorMock());
      }

      private ProjectVM CreateProjectVM() {
         return new ProjectVM(true, Results);
      }

      private CustomerVM CreateCustomerVM() {
         return new CustomerVM(true, Results);
      }
   }
}
