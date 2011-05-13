namespace Inspiring.MvvmTest.ApiTests.ViewModels.DeclarativeDeendencies {
   using System;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public sealed class DependencyValidationActionTests : DependencyTestBase {

      [TestMethod]
      public void DependencyOnViewModelTarget_RevalidatesViewModel() {
         var employeeVM = CreateEmployeeVM(
            b => b
               .OnChangeOf
               .Properties(x => x.Name)
               .Revalidate
               .Properties(x => x.SelectedProject)
         );

         var projectVM = CreateProjectVM();
         employeeVM.SelectedProject = projectVM;

         employeeVM.Name = "TriggerChange";

         Assert.IsTrue(employeeVM.SelectedProjectPropertyRevalidationBehaviorMock.WasRevalidated);
      }

      [TestMethod]
      public void DependencyOnDescendantCollection_Revalidates() {
         var descendantValidationMock = new DescendantValidationBehaviorMock();
         var viewModelValidationMock = new ViewModelRevalidationBehaviorMock();
         var titleValidationMock = new PropertyRevalidationBehaviorMock();
         var customerValidationMock = new PropertyRevalidationBehaviorMock();

         var projectVMDescriptor = ProjectVM.CreateDescriptor(
            null,
            descendantValidationMock,
            viewModelValidationMock,
            titleValidationMock,
            customerValidationMock,
            true
         );

         var employeeVM = CreateEmployeeVM(
            b => b
               .OnChangeOf
               .Properties(x => x.Name)
               .Revalidate
               .Descendant(x => x.Projects),
            projectVMDescriptor
         );

         var projectVM1 = CreateProjectVM();
         var projectVM2 = CreateProjectVM();

         employeeVM.Projects.Add(projectVM1);
         employeeVM.Projects.Add(projectVM2);

         employeeVM.Name = "TriggerChange";
      }

      private EmployeeVM CreateEmployeeVM(
         Action<IVMDependencyBuilder<EmployeeVM, EmployeeVMDescriptor>> dependencyConfigurationAction,
         ProjectVMDescriptor projectVMDescriptor = null
      ) {
         projectVMDescriptor = projectVMDescriptor ?? ProjectVM.CreateDescriptor(null, null, null, null, null, false);
         return new EmployeeVM(dependencyConfigurationAction, projectVMDescriptor, true);
      }

      private ProjectVM CreateProjectVM() {
         return new ProjectVM(true);
      }

      private CustomerVM CreateCustomerVM() {
         return new CustomerVM(true);
      }
   }
}
