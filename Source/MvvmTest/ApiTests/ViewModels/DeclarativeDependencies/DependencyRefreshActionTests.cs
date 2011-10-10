namespace Inspiring.MvvmTest.ApiTests.ViewModels.DeclarativeDeendencies {
   using System;
   using System.Linq;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public sealed class DependencyRefreshActionTests : DependencyTestBase {

      [TestMethod]
      public void DependencyOnViewModelTarget_RefreshesViewModel() {

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

         var refreshedViewModels = projectVM.RefreshControllerBehaviorMock.RefreshedViewModels;
         Assert.IsTrue(refreshedViewModels.Contains(projectVM));
      }

      [TestMethod]
      public void DependencyOnPropertyTarget_RefreshesProperty() {

         var refreshControllerBehaviorMock = new RefreshControllerBehaviorMock();

         var employeeVM = CreateEmployeeVM(
            b => b
               .OnChangeOf
               .Properties(x => x.SelectedProject)
               .Refresh
               .Properties(x => x.Name)
         );

         var changeTrigger = CreateProjectVM();
         employeeVM.SelectedProject = changeTrigger;

         var refreshedProperties = employeeVM.RefreshControllerBehaviorMock.RefreshedProperties;
         var expectedRefreshedProperty = employeeVM.Descriptor.Name;
         Assert.IsTrue(refreshedProperties.Contains(expectedRefreshedProperty));
      }

      [TestMethod]
      public void DependencyOnCollectionTarget_RefreshesAllViewModelsInCollection() {
         var refreshMock = new RefreshControllerBehaviorMock();
         var projectVMDescriptor = ProjectVM.CreateDescriptor(refreshMock, true);

         var employeeVM = CreateEmployeeVM(
            b => b
               .OnChangeOf
               .Properties(x => x.Name)
               .Refresh
               .Descendant(x => x.Projects),
            projectVMDescriptor
         );

         var projectVM1 = CreateProjectVM();
         var projectVM2 = CreateProjectVM();

         employeeVM.Projects.Add(projectVM1);
         employeeVM.Projects.Add(projectVM2);

         employeeVM.Name = "TriggerChange";

         CollectionAssert.AreEquivalent(employeeVM.Projects, refreshMock.RefreshedViewModels);
      }

      [TestMethod]
      public void DependencyTargetingSelf_RefreshesSelf() {
         var refreshMock = new RefreshControllerBehaviorMock();
         
         var employeeVM = CreateEmployeeVM(
            b => b
               .OnChangeOf
               .Self
               .OrAnyDescendant
               .Refresh
               .Self(),
            refreshControllerMock: refreshMock
         );
                     
         employeeVM.Name = "TriggerChange";

         CollectionAssert.AreEquivalent(new[] { employeeVM }, refreshMock.RefreshedViewModels);
      }

      [TestMethod]
      public void DependencyOnTargetPropertiesOfDescendantOfDescendantCollection_RefreshesSpecifiedPropertiesDescendantsInCollection() {
         var refreshMock = new RefreshControllerBehaviorMock();
         var projectVMDescriptor = ProjectVM.CreateDescriptor(refreshMock, true);

         var employeeVM = CreateEmployeeVM(
            b => b
               .OnChangeOf
               .Properties(x => x.Name)
               .Refresh
               .Descendant(x => x.Projects)
               .Descendant(x => x.Customer)
               .Properties(x => x.Name, x => x.Rating),
            projectVMDescriptor
         );

         var projectVM1 = CreateProjectVM();
         var projectVM2 = CreateProjectVM();

         employeeVM.Projects.Add(projectVM1);
         employeeVM.Projects.Add(projectVM2);

         var customerVM1 = CreateCustomerVM();
         customerVM1.Name = "aValidName";
         var customerVM2 = CreateCustomerVM();
         customerVM2.Name = "aValidName";

         projectVM1.Customer = customerVM1;
         projectVM2.Customer = customerVM2;

         employeeVM.Name = "TriggerChange";

         var expectedRefreshedPropertiesOnCustomer1 = new IVMPropertyDescriptor[] {
            customerVM1.Descriptor.Name,
            customerVM1.Descriptor.Rating
         };

         var expectedRefreshedPropertiesOnCustomer2 = new IVMPropertyDescriptor[] {
            customerVM2.Descriptor.Name,
            customerVM2.Descriptor.Rating
         };

         var refreshedPropertiesOnCustomer1 = customerVM1.RefreshControllerBehaviorMock.RefreshedProperties;
         var refreshedPropertiesOnCustomer2 = customerVM2.RefreshControllerBehaviorMock.RefreshedProperties;

         CollectionAssert.AreEquivalent(
            expectedRefreshedPropertiesOnCustomer1,
            refreshedPropertiesOnCustomer1
         );
         CollectionAssert.AreEquivalent(
            expectedRefreshedPropertiesOnCustomer2,
            refreshedPropertiesOnCustomer2
         );
      }

      [TestMethod]
      public void DependencyOnTargetDescendantOfDescendantCollection_RefreshesAllViewModelsInCollection() {
         var refreshMock = new RefreshControllerBehaviorMock();
         var projectVMDescriptor = ProjectVM.CreateDescriptor(refreshMock, true);

         var employeeVM = CreateEmployeeVM(
            b => b
               .OnChangeOf
               .Properties(x => x.Name)
               .Refresh
               .Descendant(x => x.Projects)
               .Descendant(x => x.Customer),
            projectVMDescriptor
         );

         var projectVM1 = CreateProjectVM();
         var projectVM2 = CreateProjectVM();

         employeeVM.Projects.Add(projectVM1);
         employeeVM.Projects.Add(projectVM2);

         var customerVM1 = CreateCustomerVM();
         customerVM1.Name = "aValidName";
         var customerVM2 = CreateCustomerVM();
         customerVM2.Name = "aValidName";

         projectVM1.Customer = customerVM1;
         projectVM2.Customer = customerVM2;

         employeeVM.Name = "TriggerChange";

         var expectedRefreshedCustomers = employeeVM.Projects.Select(x => x.Customer).ToArray();

         var refreshedViewModelsOnCustomer1 = customerVM1.RefreshControllerBehaviorMock.RefreshedViewModels;
         var refreshedViewModelsOnCustomer2 = customerVM2.RefreshControllerBehaviorMock.RefreshedViewModels;

         Assert.IsTrue(refreshedViewModelsOnCustomer1.Contains(customerVM1));
         Assert.IsTrue(refreshedViewModelsOnCustomer2.Contains(customerVM2));
      }

      [TestMethod]
      public void DependencyOnTargetPropertiesOfDescendantCollection_RefreshesAllPropertiesOfAllViewModelsInCollection() {
         var refreshMock = new RefreshControllerBehaviorMock();
         var projectVMDescriptor = ProjectVM.CreateDescriptor(refreshMock, true);
         var employeeVM = CreateEmployeeVM(
            b => b
               .OnChangeOf
               .Properties(x => x.Name)
               .Refresh
               .Descendant(x => x.Projects)
               .Properties(x => x.Title, x => x.Customer),
            projectVMDescriptor
         );

         var projectVM1 = CreateProjectVM();
         var projectVM2 = CreateProjectVM();

         employeeVM.Projects.Add(projectVM1);
         employeeVM.Projects.Add(projectVM2);

         employeeVM.Name = "TriggerChange";

         var expectedRefreshedProperties = new IVMPropertyDescriptor[] {
            projectVM1.Descriptor.Title,
            projectVM1.Descriptor.Customer,
            projectVM1.Descriptor.Title,
            projectVM1.Descriptor.Customer,
         };

         var refreshedProperties = refreshMock.RefreshedProperties;

         CollectionAssert.AreEquivalent(expectedRefreshedProperties, refreshedProperties);
      }

      [TestMethod]
      public void ToString_ReturnsReadableOutput() {
         var employeeVM = CreateEmployeeVM(
            b => b
               .OnChangeOf
               .Self
               .OrAnyDescendant
               .Refresh
               .Descendant(x => x.Projects)
               .Descendant(x => x.Customer)
               .Properties(x => x.Name, x => x.Rating)
         );

         var behavior = employeeVM
            .Descriptor
            .Behaviors
            .GetNextBehavior<DeclarativeDependencyBehavior>();

         DeclarativeDependency dp = behavior.Dependencies.Single();

         string expected =
            "On change of 'EmployeeVMDescriptor.[anything]?' refresh 'EmployeeVMDescriptor.Projects.Customer.[Name AND Rating]'";

         Assert.AreEqual(expected, dp.ToString());
      }

      private EmployeeVM CreateEmployeeVM(
         Action<IVMDependencyBuilder<EmployeeVM, EmployeeVMDescriptor>> dependencyConfigurationAction,
         ProjectVMDescriptor projectVMDescriptor = null,
         RefreshControllerBehaviorMock refreshControllerMock = null
      ) {
         refreshControllerMock = refreshControllerMock ?? new RefreshControllerBehaviorMock();

         projectVMDescriptor = projectVMDescriptor ?? ProjectVM.CreateDescriptor(null, false);
         return new EmployeeVM(dependencyConfigurationAction, projectVMDescriptor, true, Results, refreshControllerMock);
      }

      private ProjectVM CreateProjectVM() {
         return new ProjectVM(true, Results);
      }

      private CustomerVM CreateCustomerVM() {
         return new CustomerVM(true, Results);
      }
   }
}
