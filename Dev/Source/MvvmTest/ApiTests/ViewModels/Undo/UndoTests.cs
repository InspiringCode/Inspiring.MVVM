namespace Inspiring.MvvmTest.ApiTests.ViewModels.Undo {
   using System.Collections.Generic;
   using System.Linq;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class UndoTests {
      private EmployeeVM EmployeeVM { get; set; }

      [TestInitialize]
      public void Setup() {
         EmployeeVM = new EmployeeVM();
      }

      [TestMethod]
      public void GetRollbackPoint_NoModifications_ReturnsPseudoAction() {
         var rollbackPoint = EmployeeVM.UndoManager.GetRollbackPoint();
         Assert.IsInstanceOfType(rollbackPoint, typeof(InitialPseudoAction));
      }

      [TestMethod]
      public void GetRollbackPoint_PropertyWasModifiedBefore_ReturnsLastAction() {
         EmployeeVM.Name = "ModifiedName";

         var rollbackPoint = EmployeeVM.UndoManager.GetRollbackPoint();

         Assert.IsInstanceOfType(rollbackPoint, typeof(SetValueAction<string>));
      }

      [TestMethod]
      public void RollbackTo_ModifyProperty_RestoresRollbackPoint() {
         var originalEmployeeName = "Name";
         EmployeeVM.InitializeFrom(CreateEmployee(originalEmployeeName));

         var rollbackPoint = EmployeeVM.UndoManager.GetRollbackPoint();

         EmployeeVM.Name = "ModifiedName";

         EmployeeVM.UndoManager.RollbackTo(rollbackPoint);

         Assert.AreEqual(originalEmployeeName, EmployeeVM.Name);
         Assert.AreSame(rollbackPoint, EmployeeVM.UndoManager.GetRollbackPoint());
      }

      [TestMethod]
      public void RollbackTo_ModifyChildVM_RestoresRollbackPoint() {
         var projectTitle = "Project1";
         var projects = new Project[] {
            CreateProject(projectTitle)
         };

         EmployeeVM.InitializeFrom(CreateEmployee(projects: projects));

         var expectedProjectVM = EmployeeVM
            .Projects
            .Single(x => x.Title == projectTitle);

         EmployeeVM.SelectedProject = expectedProjectVM;

         var rollbackPoint = EmployeeVM.UndoManager.GetRollbackPoint();

         EmployeeVM.SelectedProject = null;

         EmployeeVM.UndoManager.RollbackTo(rollbackPoint);

         Assert.AreSame(expectedProjectVM, EmployeeVM.SelectedProject);
      }

      [TestMethod]
      public void RollbackTo_AddNewItemToCollection_RestoresRollbackPoint() {
         var projects = new Project[] {
            CreateProject("Project1"),
            CreateProject("Project2")
         };

         EmployeeVM.InitializeFrom(CreateEmployee(projects: projects));

         ProjectVM[] expectedProjectVMColl = new ProjectVM[EmployeeVM.Projects.Count];
         EmployeeVM.Projects.CopyTo(expectedProjectVMColl, 0);

         var rollbackPoint = EmployeeVM.UndoManager.GetRollbackPoint();

         // Act
         EmployeeVM.Projects.Add(new ProjectVM());

         EmployeeVM.UndoManager.RollbackTo(rollbackPoint);

         CollectionAssert.AreEqual(expectedProjectVMColl, EmployeeVM.Projects);
         Assert.AreSame(rollbackPoint, EmployeeVM.UndoManager.GetRollbackPoint());
      }

      [TestMethod]
      public void RollbackTo_RemoveItemFromCollection_RestoresRollbackPoint() {
         string projectTitle = "Project2";

         var projects = new Project[] {
            CreateProject("Project1"),
            CreateProject(projectTitle),
            CreateProject("Project3"),
         };

         EmployeeVM.InitializeFrom(CreateEmployee(projects: projects));

         ProjectVM[] expectedProjectVMColl = new ProjectVM[EmployeeVM.Projects.Count];
         EmployeeVM.Projects.CopyTo(expectedProjectVMColl, 0);

         var rollbackPoint = EmployeeVM.UndoManager.GetRollbackPoint();

         // Act
         var itemToRemove = EmployeeVM.Projects.Single(x => x.Title == projectTitle);
         EmployeeVM.Projects.Remove(itemToRemove);

         EmployeeVM.UndoManager.RollbackTo(rollbackPoint);

         CollectionAssert.AreEqual(expectedProjectVMColl, EmployeeVM.Projects);
         Assert.AreSame(rollbackPoint, EmployeeVM.UndoManager.GetRollbackPoint());
      }

      [TestMethod]
      public void RollbackTo_SetCollectionItem_RestoresRollbackPoint() {
         var projects = new Project[] {
            CreateProject("Project1"),
            CreateProject("Project2"),
            CreateProject("Project3"),
         };

         EmployeeVM.InitializeFrom(CreateEmployee(projects: projects));

         ProjectVM[] expectedProjectVMColl = new ProjectVM[EmployeeVM.Projects.Count];
         EmployeeVM.Projects.CopyTo(expectedProjectVMColl, 0);

         var rollbackPoint = EmployeeVM.UndoManager.GetRollbackPoint();

         // Act
         EmployeeVM.Projects[0] = new ProjectVM();
         EmployeeVM.UndoManager.RollbackTo(rollbackPoint);

         CollectionAssert.AreEqual(expectedProjectVMColl, EmployeeVM.Projects);
         Assert.AreSame(rollbackPoint, EmployeeVM.UndoManager.GetRollbackPoint());
      }

      [TestMethod]
      public void RollbackTo_ClearCollection_RestoresRollbackPoint() {
         var projects = new Project[] {
            CreateProject("Project1"),
            CreateProject("Project2"),
            CreateProject("Project3"),
            CreateProject("Project4"),
         };

         EmployeeVM.InitializeFrom(CreateEmployee(projects: projects));

         ProjectVM[] expectedProjectVMColl = new ProjectVM[EmployeeVM.Projects.Count];
         EmployeeVM.Projects.CopyTo(expectedProjectVMColl, 0);

         var rollbackPoint = EmployeeVM.UndoManager.GetRollbackPoint();

         // Act
         EmployeeVM.Projects.Clear();
         EmployeeVM.UndoManager.RollbackTo(rollbackPoint);

         CollectionAssert.AreEqual(expectedProjectVMColl, EmployeeVM.Projects);
         Assert.AreSame(rollbackPoint, EmployeeVM.UndoManager.GetRollbackPoint());
      }

      [TestMethod]
      public void RollbackTo_RefreshCollection_RestoresRollbackPoint() {
         var projects = new Project[] {
            CreateProject("Project1"),
            CreateProject("Project2"),
            CreateProject("Project3"),
            CreateProject("Project4"),
         };

         EmployeeVM.InitializeFrom(CreateEmployee(projects: projects));

         ProjectVM[] expectedProjectVMColl = new ProjectVM[EmployeeVM.Projects.Count];
         EmployeeVM.Projects.CopyTo(expectedProjectVMColl, 0);

         var rollbackPoint = EmployeeVM.UndoManager.GetRollbackPoint();

         // Act
         ((IViewModel)EmployeeVM).Kernel.Refresh(EmployeeVM.ClassDescriptor.Projects);
         EmployeeVM.UndoManager.RollbackTo(rollbackPoint);

         CollectionAssert.AreEqual(expectedProjectVMColl, EmployeeVM.Projects);
         Assert.AreSame(rollbackPoint, EmployeeVM.UndoManager.GetRollbackPoint());
      }

      [TestMethod]
      public void RollbackTo_ModifyingSourceObjectAndCallRefresh_DoesNotUndo() {
         var employee = CreateEmployee();

         EmployeeVM.InitializeFrom(employee);

         var rollbackPoint = EmployeeVM.UndoManager.GetRollbackPoint();

         var modifiedName = string.Format("Modified{0}", EmployeeVM.Name);
         employee.Name = modifiedName;

         ((IViewModel)EmployeeVM).Kernel.Refresh();

         EmployeeVM.UndoManager.RollbackTo(rollbackPoint);

         Assert.AreEqual(modifiedName, EmployeeVM.Name);
      }

      [TestMethod]
      public void RollbackTo_SeveralModifications_RestoresRollbackPoint() {
         // Arrange
         var originalCustomer1Name = "Customer1";
         var customer1 = CreateCustomer(originalCustomer1Name);
         var customer2 = CreateCustomer();

         var originalProject1Title = "Project1";
         var projects = new Project[] {
            CreateProject(originalProject1Title, customer1),
            CreateProject("Project2", customer2)
         };
         var originalEmployeeName = "Name";
         var employee = CreateEmployee(name: originalEmployeeName, projects: projects);

         EmployeeVM.InitializeFrom(employee);


         ProjectVM[] expectedProjectVMColl = new ProjectVM[EmployeeVM.Projects.Count];
         EmployeeVM.Projects.CopyTo(expectedProjectVMColl, 0);

         var rollbackPoint = EmployeeVM.UndoManager.GetRollbackPoint();

         // Act
         EmployeeVM.Projects.Add(new ProjectVM());

         EmployeeVM.Name = "ModifiedName";

         ((IViewModel)EmployeeVM).Kernel.Refresh();

         var project1VM = EmployeeVM
            .Projects
            .Single(x => x.Title == originalProject1Title);
         project1VM.Title = "ModifiedTitle";

         project1VM.Customer.Name = "ModifiedName";

         EmployeeVM.Projects.RemoveAt(0);

         EmployeeVM.Projects.Clear();

         EmployeeVM.UndoManager.RollbackTo(rollbackPoint);

         // Assert
         Assert.AreEqual(originalEmployeeName, EmployeeVM.Name);
         Assert.AreEqual(project1VM.Title, originalProject1Title);
         Assert.AreEqual(project1VM.Customer.Name, originalCustomer1Name);
         CollectionAssert.AreEqual(expectedProjectVMColl, EmployeeVM.Projects);
      }

      [TestMethod]
      public void MultipleRollbackTo_WithMultipleRollbackPoints_RestoresRollbackPoints() {
         // Arrange
         var originalCustomer1Name = "Customer1";
         var customer1 = CreateCustomer(originalCustomer1Name);
         var customer2 = CreateCustomer();

         var originalProject1Title = "Project1";
         var projects = new Project[] {
            CreateProject(originalProject1Title, customer1),
            CreateProject("Project2", customer2)
         };
         var originalEmployeeName = "Name";
         var employee = CreateEmployee(name: originalEmployeeName, projects: projects);

         EmployeeVM.InitializeFrom(employee);

         ProjectVM[] expectedProjectVMColl = new ProjectVM[EmployeeVM.Projects.Count];
         EmployeeVM.Projects.CopyTo(expectedProjectVMColl, 0);

         // Act - Assert
         var employeeVMRollbackPoint = EmployeeVM.UndoManager.GetRollbackPoint();

         EmployeeVM.Projects.Add(new ProjectVM());

         EmployeeVM.Name = "ModifiedName";

         ((IViewModel)EmployeeVM).Kernel.Refresh();

         var project1VM = EmployeeVM
            .Projects
            .Single(x => x.Title == originalProject1Title);

         var projectVMRollbackPoint = project1VM.UndoManager.GetRollbackPoint();

         project1VM.Title = "ModifiedTitle";

         var customerVMRollbackPoint = project1VM.Customer.UndoManager.GetRollbackPoint();

         project1VM.Customer.Name = "ModifiedName";

         project1VM.Customer.UndoManager.RollbackTo(customerVMRollbackPoint);
         Assert.AreEqual(project1VM.Customer.Name, originalCustomer1Name);

         project1VM.UndoManager.RollbackTo(projectVMRollbackPoint);
         Assert.AreEqual(project1VM.Title, originalProject1Title);

         EmployeeVM.Projects.RemoveAt(0);

         EmployeeVM.Projects.Clear();

         EmployeeVM.UndoManager.RollbackTo(employeeVMRollbackPoint);

         Assert.AreEqual(originalEmployeeName, EmployeeVM.Name);
         CollectionAssert.AreEqual(expectedProjectVMColl, EmployeeVM.Projects);
      }

      [TestMethod]
      public void RollbackTo_ModificationInvalidatesViewModel_RollbackValidatesViewModel() {
         EmployeeVM.InitializeFrom(CreateEmployee());

         EmployeeVM.Revalidate();

         Assert.IsTrue(EmployeeVM.IsValid);

         var rollbackPoint = EmployeeVM.UndoManager.GetRollbackPoint();

         EmployeeVM.Name = null;

         Assert.IsFalse(EmployeeVM.IsValid);

         EmployeeVM.UndoManager.RollbackTo(rollbackPoint);

         Assert.IsTrue(EmployeeVM.IsValid);
      }

      private Employee CreateEmployee(
         string name = "Employee",
         Project currentProject = null,
         params Project[] projects
      ) {
         var employee = new Employee {
            Name = name,
            CurrentProject = currentProject
         };
         employee.Projects = new List<Project>(projects);
         return employee;
      }

      private Project CreateProject(
        string title = "ProjectTitle",
        Customer customer = null
      ) {
         return new Project {
            Title = title,
            Customer = customer
         };
      }

      private Customer CreateCustomer(string name = "CustomersName") {
         return new Customer { Name = name };
      }
   }
}