using System;
using Inspiring.Mvvm.ViewModels.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Inspiring.MvvmTest.ApiTests.ViewModels.DeclarativeDeendencies {

   [TestClass]
   public sealed class DependencyRefreshActionTests : DependencyTestBase {

      [TestMethod]
      public void DependencyOnProperty_WhenPropertyChanges_RefreshesTarget() {

         var refreshControllerBehaviorMock = new RefreshControllerBehaviorMock();

         var employeeVM = CreateEmployeeVM(
            b => b
               .OnChangeOf
               .Properties(x => x.Name)
               .Refresh
               .Properties(x => x.SelectedProject)
         );

         var projectVM = CreateProjectVM();
         employeeVM.SelectedProject = projectVM;

         employeeVM.Name = "TriggerChange";

         Assert.AreSame(projectVM, projectVM.RefreshControllerBehaviorMock.RefreshedViewModel);
      }

      private EmployeeVM CreateEmployeeVM(
         Action<IVMDependencyBuilder<EmployeeVM, EmployeeVMDescriptor>> dependencyConfigurationAction,
         ProjectVMDescriptor projectVMDescriptor = null
      ) {
         projectVMDescriptor = projectVMDescriptor ?? ProjectVM.CreateDescriptor(null, null, null, null, false);
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
