namespace Inspiring.MvvmTest.ApiTests.ViewModels.DeclarativeDeendencies {
   using System;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class DependencyExecuteActionTests : DependencyTestBase {

      [TestMethod]
      public void DependencyOnSelfWithExecuteAction_PropertyChangeOrValidationStateChange_ExecutesAction() {
         bool actionWasCalled = false;
         Action<EmployeeVM, ChangeArgs> action = (vm, args) => {
            actionWasCalled = true;
         };

         var employeeVM = CreateEmployeeVM(
            b => b
               .OnChangeOf
               .Self
               .Execute(action)
         );

         employeeVM.Name = "TriggerChange";
         Assert.IsTrue(actionWasCalled);

         actionWasCalled = false;

         string invalidName = string.Empty;
         employeeVM.Name = invalidName;
         Assert.IsTrue(actionWasCalled);
      }

      [TestMethod]
      public void DependencyOnCollectionWithExecuteAction_ItemAddedOrRemovedFromCollection_ExecutesAction() {
         bool actionWasCalled = false;
         Action<EmployeeVM, ChangeArgs> action = (vm, args) => {
            actionWasCalled = true;
         };

         var employeeVM = CreateEmployeeVM(
            b => b
               .OnChangeOf
               .Collection(x => x.Projects)
               .Execute(action)
         );

         employeeVM.Projects.Add(CreateProjectVM());
         Assert.IsTrue(actionWasCalled);

         actionWasCalled = false;

         string invalidName = string.Empty;
         employeeVM.Name = invalidName;
         Assert.IsFalse(actionWasCalled);
      }

      [TestMethod]
      public void DependencyOnSelfOrAnyDescendantWithExecuteAction_PropertyChangeOrValidationStateChange_ExecutesAction() {
         bool actionWasCalled = false;
         Action<EmployeeVM, ChangeArgs> action = (vm, args) => {
            actionWasCalled = true;
         };

         var employeeVM = CreateEmployeeVM(
            b => b
               .OnChangeOf
               .Self
               .OrAnyDescendant
               .Execute(action)
         );

         var customerVM = CreateCustomerVM();
         var projectVM = CreateProjectVM();
         employeeVM.Projects.Add(projectVM);
         projectVM.Customer = customerVM;

         customerVM.Name = "TriggerChange";
         Assert.IsTrue(actionWasCalled);
      }

      [TestMethod]
      public void DependencyOnDescendentPropertiesWithExecuteChangeAction_SpecifiedPropertiesChange_ExecutesAction() {
         bool actionWasCalled = false;
         Action<EmployeeVM, ChangeArgs> action = (vm, args) => {
            actionWasCalled = true;
         };

         var employeeVM = CreateEmployeeVM(
            b => b
               .OnChangeOf
               .Descendant(x => x.Projects)
               .Descendant(x => x.Customer)
               .Properties(x => x.Name, x => x.Rating)
               .Execute(action)
         );

         var customerVM = CreateCustomerVM();
         var projectVM = CreateProjectVM();

         employeeVM.Projects.Add(projectVM);
         projectVM.Customer = customerVM;

         customerVM.Name = "TriggerChange";
         Assert.IsTrue(actionWasCalled);

         actionWasCalled = false;

         customerVM.Rating = 1;
         Assert.IsTrue(actionWasCalled);

         actionWasCalled = false;

         customerVM.Address = "ShouldNotTriggerExecuteOfAction";
         Assert.IsFalse(actionWasCalled);

         projectVM.Title = "ShouldNotTriggerExecuteOfAction";
         Assert.IsFalse(actionWasCalled);

         employeeVM.Name = "ShouldNotTriggerExecuteOfAction";
         Assert.IsFalse(actionWasCalled);
      }

      [TestMethod]
      public void DependencyForDescendantWithExecuteAction_PropertyChangeOrValidationStateChange_ExecutesAction() {
         bool actionWasCalled = false;
         Action<EmployeeVM, ChangeArgs> action = (vm, args) => {
            actionWasCalled = true;
         };

         var employeeVM = CreateEmployeeVM(
            b => b
               .OnChangeOf
               .Descendant(x => x.SelectedProject)
               .Execute(action)
         );

         var projectVM = CreateProjectVM();
         employeeVM.SelectedProject = projectVM;

         projectVM.Title = "TriggerExecuteAction";
         Assert.IsTrue(actionWasCalled);
      }

      [TestMethod]
      public void DependencyForDescendantOrAnyDescendantWithExecuteAction_PropertyChangeOrValidationStateChange_ExecutesAction() {
         bool actionWasCalled = false;
         Action<EmployeeVM, ChangeArgs> action = (vm, args) => {
            actionWasCalled = true;
         };

         var employeeVM = CreateEmployeeVM(
            b => b
               .OnChangeOf
               .Descendant(x => x.SelectedProject)
               .OrAnyDescendant
               .Execute(action)
         );

         var customerVM = CreateCustomerVM();
         var projectVM = CreateProjectVM();
         projectVM.Customer = customerVM;
         employeeVM.SelectedProject = projectVM;

         projectVM.Title = "TriggerExecuteAction";
         Assert.IsTrue(actionWasCalled);

         actionWasCalled = false;

         customerVM.Name = "TriggerExecuteAction";
         Assert.IsTrue(actionWasCalled);
      }

      [TestMethod]
      public void DependencyForOnlyDescendantCollection_PropertyChange_ExecutesAction() {
         bool actionWasCalled = false;
         Action<EmployeeVM, ChangeArgs> action = (vm, args) => {
            actionWasCalled = true;
         };

         var projectVM = CreateProjectVM();
         var employeeVM = CreateEmployeeVM(
            b => b
               .OnChangeOf
               .Descendant(x => x.Projects)
               .Execute(action)
         );

         employeeVM.Projects.Add(projectVM);
         Assert.IsFalse(actionWasCalled);

         projectVM.Title = "ShouldTriggerExecuteAction";
         Assert.IsTrue(actionWasCalled);
      }

      private EmployeeVM CreateEmployeeVM(
         Action<IVMDependencyBuilder<EmployeeVM, EmployeeVMDescriptor>> dependencyConfigurationAction
      ) {
         var projectVMDescriptor = ProjectVM.CreateDescriptor(null, null, null, null, null, false);
         return new EmployeeVM(dependencyConfigurationAction, projectVMDescriptor, false);
      }

      private ProjectVM CreateProjectVM() {
         return new ProjectVM(false);
      }

      private CustomerVM CreateCustomerVM() {
         return new CustomerVM(false);
      }
   }
}