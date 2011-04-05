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
         var rollbackPoint = ((IViewModel)EmployeeVM).Kernel.GetRollbackPoint();
         Assert.IsInstanceOfType(rollbackPoint, typeof(InitialPseudoAction));
      }

      [TestMethod]
      public void GetRollbackPoint_PropertyWasModifiedBefore_ReturnsLastAction() {
         var originalEmployeeName = EmployeeVM.GetValue(x => x.Name);
         EmployeeVM.SetValue(x => x.Name, string.Format("Modified{0}", originalEmployeeName));

         var rollbackPoint = ((IViewModel)EmployeeVM).Kernel.GetRollbackPoint();

         Assert.IsInstanceOfType(rollbackPoint, typeof(SetValueAction<string>));
      }

      [TestMethod]
      public void RollbackTo_ModifyProperty_RestoresRollbackPoint() {
         var employee = CreateEmployee();
         EmployeeVM.InitializeFrom(employee);

         var rollbackPoint = ((IViewModel)EmployeeVM).Kernel.GetRollbackPoint();

         var originalEmployeeName = EmployeeVM.GetValue(x => x.Name);
         EmployeeVM.SetValue(x => x.Name, string.Format("Modified{0}", originalEmployeeName));

         ((IViewModel)EmployeeVM).Kernel.RollbackTo(rollbackPoint);

         Assert.AreEqual(originalEmployeeName, EmployeeVM.GetValue(x => x.Name));
         Assert.AreSame(rollbackPoint, ((IViewModel)EmployeeVM).Kernel.GetRollbackPoint());
      }

      [TestMethod]
      public void RollbackTo_AddNewItemToCollection_RestoresRollbackPoint() {
         var projects = new Project[] {
            CreateProject("Project1"),
            CreateProject("Project2")
         };

         var employee = CreateEmployee(projects: projects);

         EmployeeVM.InitializeFrom(employee);

         var projectVMColl = EmployeeVM.GetValue(x => x.Projects);

         ProjectVM[] expectedProjectVMColl = new ProjectVM[projectVMColl.Count];
         projectVMColl.CopyTo(expectedProjectVMColl, 0);

         var rollbackPoint = ((IViewModel)EmployeeVM).Kernel.GetRollbackPoint();

         // Act
         projectVMColl.Add(new ProjectVM());

         ((IViewModel)EmployeeVM).Kernel.RollbackTo(rollbackPoint);

         CollectionAssert.AreEqual(expectedProjectVMColl, projectVMColl);
         Assert.AreSame(rollbackPoint, ((IViewModel)EmployeeVM).Kernel.GetRollbackPoint());
      }

      [TestMethod]
      public void RollbackTo_RemoveItemFromCollection_RestoresRollbackPoint() {
         string projectTitle = "Project2";

         var projects = new Project[] {
            CreateProject("Project1"),
            CreateProject(projectTitle),
            CreateProject("Project3"),
         };

         var employee = CreateEmployee(projects: projects);

         EmployeeVM.InitializeFrom(employee);

         var projectVMColl = EmployeeVM.GetValue(x => x.Projects);

         ProjectVM[] expectedProjectVMColl = new ProjectVM[projectVMColl.Count];
         projectVMColl.CopyTo(expectedProjectVMColl, 0);

         var rollbackPoint = ((IViewModel)EmployeeVM).Kernel.GetRollbackPoint();

         // Act
         var itemToRemove = projectVMColl.Single(x => x.GetValue(d => d.Title) == projectTitle);
         projectVMColl.Remove(itemToRemove);

         ((IViewModel)EmployeeVM).Kernel.RollbackTo(rollbackPoint);

         CollectionAssert.AreEqual(expectedProjectVMColl, projectVMColl);
         Assert.AreSame(rollbackPoint, ((IViewModel)EmployeeVM).Kernel.GetRollbackPoint());
      }

      [TestMethod]
      public void RollbackTo_SetCollectionItem_RestoresRollbackPoint() {
         var projects = new Project[] {
            CreateProject("Project1"),
            CreateProject("Project2"),
            CreateProject("Project3"),
         };

         var employee = CreateEmployee(projects: projects);

         EmployeeVM.InitializeFrom(employee);

         var projectVMColl = EmployeeVM.GetValue(x => x.Projects);

         ProjectVM[] expectedProjectVMColl = new ProjectVM[projectVMColl.Count];
         projectVMColl.CopyTo(expectedProjectVMColl, 0);

         var rollbackPoint = ((IViewModel)EmployeeVM).Kernel.GetRollbackPoint();

         // Act
         projectVMColl[0] = new ProjectVM();
         ((IViewModel)EmployeeVM).Kernel.RollbackTo(rollbackPoint);

         CollectionAssert.AreEqual(expectedProjectVMColl, projectVMColl);
         Assert.AreSame(rollbackPoint, ((IViewModel)EmployeeVM).Kernel.GetRollbackPoint());
      }

      [TestMethod]
      public void RollbackTo_ClearCollection_RestoresRollbackPoint() {
         var projects = new Project[] {
            CreateProject("Project1"),
            CreateProject("Project2"),
            CreateProject("Project3"),
            CreateProject("Project4"),
         };

         var employee = CreateEmployee(projects: projects);

         EmployeeVM.InitializeFrom(employee);

         var projectVMColl = EmployeeVM.GetValue(x => x.Projects);

         ProjectVM[] expectedProjectVMColl = new ProjectVM[projectVMColl.Count];
         projectVMColl.CopyTo(expectedProjectVMColl, 0);

         var rollbackPoint = ((IViewModel)EmployeeVM).Kernel.GetRollbackPoint();

         // Act
         projectVMColl.Clear();
         ((IViewModel)EmployeeVM).Kernel.RollbackTo(rollbackPoint);

         CollectionAssert.AreEqual(expectedProjectVMColl, projectVMColl);
         Assert.AreSame(rollbackPoint, ((IViewModel)EmployeeVM).Kernel.GetRollbackPoint());
      }

      [TestMethod]
      public void RollbackTo_RefreshCollection_RestoresRollbackPoint() {
         var projects = new Project[] {
            CreateProject("Project1"),
            CreateProject("Project2"),
            CreateProject("Project3"),
            CreateProject("Project4"),
         };

         var employee = CreateEmployee(projects: projects);

         EmployeeVM.InitializeFrom(employee);

         var projectVMColl = EmployeeVM.GetValue(x => x.Projects);

         ProjectVM[] expectedProjectVMColl = new ProjectVM[projectVMColl.Count];
         projectVMColl.CopyTo(expectedProjectVMColl, 0);

         var rollbackPoint = ((IViewModel)EmployeeVM).Kernel.GetRollbackPoint();

         // Act
         ((IViewModel)EmployeeVM).Kernel.Refresh(EmployeeVM.ClassDescriptor.Projects);
         ((IViewModel)EmployeeVM).Kernel.RollbackTo(rollbackPoint);

         CollectionAssert.AreEqual(expectedProjectVMColl, projectVMColl);
         Assert.AreSame(rollbackPoint, ((IViewModel)EmployeeVM).Kernel.GetRollbackPoint());
      }

      [TestMethod]
      public void RollbackTo_ModifyingSourceObjectAndCallRefresh_DoesNotUndo() {
         var employee = CreateEmployee();

         EmployeeVM.InitializeFrom(employee);
         var originalEmployeeName = EmployeeVM.GetValue(x => x.Name);

         var rollbackPoint = ((IViewModel)EmployeeVM).Kernel.GetRollbackPoint();

         var modifiedName = string.Format("Modified{0}", originalEmployeeName);
         employee.Name = modifiedName;

         ((IViewModel)EmployeeVM).Kernel.Refresh();

         ((IViewModel)EmployeeVM).Kernel.RollbackTo(rollbackPoint);

         Assert.AreEqual(modifiedName, EmployeeVM.GetValue(x => x.Name));
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