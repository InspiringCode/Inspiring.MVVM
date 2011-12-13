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
         Assert.IsInstanceOfType(EmployeeVM.UndoManager.TopMostAction, typeof(InitialPseudoAction));
      }

      [TestMethod]
      public void GetRollbackPoint_PropertyWasModifiedBefore_ReturnsLastAction() {
         var rollbackPoint = EmployeeVM.UndoManager.GetRollbackPoint();

         EmployeeVM.Name = "ModifiedName";

         Assert.IsInstanceOfType(EmployeeVM.UndoManager.TopMostAction, typeof(SetValueAction<string>));
      }

      [TestMethod]
      public void RollbackTo_ModifyProperty_RestoresRollbackPoint() {
         var originalEmployeeName = "Name";
         EmployeeVM.InitializeFrom(CreateEmployee(originalEmployeeName));

         var rollbackPoint = EmployeeVM.UndoManager.GetRollbackPoint();

         EmployeeVM.Name = "ModifiedName";

         EmployeeVM.UndoManager.RollbackTo(rollbackPoint);

         Assert.AreEqual(originalEmployeeName, EmployeeVM.Name);
         Assert.AreSame(rollbackPoint, EmployeeVM.UndoManager.TopMostAction);
      }

      [TestMethod]
      public void UndoManager_HasNoRollbackPoint_DoesNotTrackUndoActions() {
         EmployeeVM.InitializeFrom(CreateEmployee());

         EmployeeVM.Name = "ModifiedName";

         Assert.IsInstanceOfType(EmployeeVM.UndoManager.TopMostAction, typeof(InitialPseudoAction));

         var rollbackPoint = EmployeeVM.UndoManager.GetRollbackPoint();

         EmployeeVM.Name = "OtherName";

         Assert.IsInstanceOfType(EmployeeVM.UndoManager.TopMostAction, typeof(SetValueAction<string>));

         EmployeeVM.UndoManager.RollbackTo(rollbackPoint);

         EmployeeVM.Name = "OnMoreOtherName";

         Assert.IsInstanceOfType(EmployeeVM.UndoManager.TopMostAction, typeof(InitialPseudoAction));
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

         ProjectVM[] expectedProjectVMColl = EmployeeVM.Projects.ToArray();

         var rollbackPoint = EmployeeVM.UndoManager.GetRollbackPoint();

         // Act
         EmployeeVM.Projects.Add(new ProjectVM());

         EmployeeVM.UndoManager.RollbackTo(rollbackPoint);

         CollectionAssert.AreEqual(expectedProjectVMColl, EmployeeVM.Projects);
         Assert.AreSame(rollbackPoint, EmployeeVM.UndoManager.TopMostAction);
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

         ProjectVM[] expectedProjectVMColl = EmployeeVM.Projects.ToArray();

         var rollbackPoint = EmployeeVM.UndoManager.GetRollbackPoint();

         // Act
         var itemToRemove = EmployeeVM.Projects.Single(x => x.Title == projectTitle);
         EmployeeVM.Projects.Remove(itemToRemove);

         EmployeeVM.UndoManager.RollbackTo(rollbackPoint);

         CollectionAssert.AreEqual(expectedProjectVMColl, EmployeeVM.Projects);
         Assert.AreSame(rollbackPoint, EmployeeVM.UndoManager.TopMostAction);
      }

      [TestMethod]
      public void RollbackTo_SetCollectionItem_RestoresRollbackPoint() {
         var projects = new Project[] {
            CreateProject("Project1"),
            CreateProject("Project2"),
            CreateProject("Project3"),
         };

         EmployeeVM.InitializeFrom(CreateEmployee(projects: projects));

         ProjectVM[] expectedProjectVMColl = EmployeeVM.Projects.ToArray();

         var rollbackPoint = EmployeeVM.UndoManager.GetRollbackPoint();

         // Act
         EmployeeVM.Projects[0] = new ProjectVM();
         EmployeeVM.UndoManager.RollbackTo(rollbackPoint);

         CollectionAssert.AreEqual(expectedProjectVMColl, EmployeeVM.Projects);
         Assert.AreSame(rollbackPoint, EmployeeVM.UndoManager.TopMostAction);
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

         ProjectVM[] expectedProjectVMColl = EmployeeVM.Projects.ToArray();

         var rollbackPoint = EmployeeVM.UndoManager.GetRollbackPoint();

         // Act
         EmployeeVM.Projects.Clear();
         EmployeeVM.UndoManager.RollbackTo(rollbackPoint);

         CollectionAssert.AreEqual(expectedProjectVMColl, EmployeeVM.Projects);
         Assert.AreSame(rollbackPoint, EmployeeVM.UndoManager.TopMostAction);
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

         ProjectVM[] expectedProjectVMColl = EmployeeVM.Projects.ToArray();

         var rollbackPoint = EmployeeVM.UndoManager.GetRollbackPoint();

         // Act
         ((IViewModel)EmployeeVM).Kernel.Refresh(EmployeeVM.Descriptor.Projects);
         EmployeeVM.UndoManager.RollbackTo(rollbackPoint);

         CollectionAssert.AreEqual(expectedProjectVMColl, EmployeeVM.Projects);
         Assert.AreSame(rollbackPoint, EmployeeVM.UndoManager.TopMostAction);
      }

      [TestMethod]
      public void RollbackTo_RefreshCollectionWhenSourceCollectionChanges_RestoresRollbackPoint() {
         var projects = new Project[] {
            CreateProject("Project1"),
            CreateProject("Project2"),
            CreateProject("Project3"),
            CreateProject("Project4"),
         };

         Employee emp = CreateEmployee(projects: projects);

         EmployeeVM.InitializeFrom(emp);

         List<ProjectVM> expectedProjectVMColl = EmployeeVM.Projects.ToList();

         var rollbackPoint = EmployeeVM.UndoManager.GetRollbackPoint();

         // Act
         emp.Projects.RemoveAt(0);
         ((IViewModel)EmployeeVM).Kernel.Refresh(EmployeeVM.Descriptor.Projects);
         EmployeeVM.UndoManager.RollbackTo(rollbackPoint);

         CollectionAssert.AreEqual(expectedProjectVMColl, EmployeeVM.Projects);
         Assert.AreSame(rollbackPoint, EmployeeVM.UndoManager.TopMostAction);
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


         ProjectVM[] expectedProjectVMColl = EmployeeVM.Projects.ToArray();

         var rollbackPoint = EmployeeVM.UndoManager.GetRollbackPoint();

         // Act
         var newProject = new ProjectVM();
         newProject.InitializeFrom(new Project());
         EmployeeVM.Projects.Add(newProject);

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

         ProjectVM[] expectedProjectVMColl = EmployeeVM.Projects.ToArray();

         // Act - Assert
         var employeeVMRollbackPoint = EmployeeVM.UndoManager.GetRollbackPoint();

         var newProject = new ProjectVM();
         newProject.InitializeFrom(new Project());
         EmployeeVM.Projects.Add(newProject);

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
      public void RollbackTo_PropertyModificationInvalidatesViewModel_RollbackValidatesViewModel() {
         EmployeeVM.InitializeFrom(CreateEmployee());

         Assert.IsTrue(EmployeeVM.IsValid);

         var rollbackPoint = EmployeeVM.UndoManager.GetRollbackPoint();

         EmployeeVM.Name = null;

         Assert.IsFalse(EmployeeVM.IsValid);

         EmployeeVM.UndoManager.RollbackTo(rollbackPoint);

         Assert.IsTrue(EmployeeVM.IsValid);
      }

      [TestMethod]
      public void RollbackTo_CollectionModificationInvalidatesViewModel_RollbackValidatesViewModel() {
         var project1Title = "Title of project1";
         var project1 = CreateProject(project1Title);

         EmployeeVM.InitializeFrom(CreateEmployee(projects: project1));

         EmployeeVM.Revalidate();

         Assert.IsTrue(EmployeeVM.IsValid);

         var rollbackPoint = EmployeeVM.UndoManager.GetRollbackPoint();

         var project2 = CreateProject(project1Title);
         var project2VM = new ProjectVM();
         project2VM.InitializeFrom(project2);
         EmployeeVM.Projects.Add(project2VM);

         Assert.IsFalse(EmployeeVM.IsValid);

         EmployeeVM.UndoManager.RollbackTo(rollbackPoint);

         Assert.IsTrue(EmployeeVM.IsValid);
      }

      [TestMethod]
      public void RollbackTo_WithMultipleUndoRoots_RestoresCorrectChanges() {
         // Arrange
         var originalCustomer1Name = "Customer1";
         var customer1 = CreateCustomer(originalCustomer1Name);
         var customer2 = CreateCustomer();

         var originalProject1Title = "Project1";
         var project2Title = "Project2";
         var projects = new Project[] {
            CreateProject(originalProject1Title, customer1),
            CreateProject(project2Title, customer2)
         };
         var originalEmployeeName = "Name";
         var employee = CreateEmployee(name: originalEmployeeName, projects: projects);

         EmployeeVM = new EmployeeVM(ProjectVM.ClassDescriptorWithUndoRoot);
         EmployeeVM.InitializeFrom(employee);

         var expectedOriginalProjectVMColl = EmployeeVM.Projects.ToArray();

         var employeeVMRollbackPoint = EmployeeVM.UndoManager.GetRollbackPoint();

         EmployeeVM.Projects.Remove(EmployeeVM.Projects.Single(x => x.Title == project2Title));

         var expectedModifiedProjectVMColl = EmployeeVM.Projects.ToArray();

         var project1VM = EmployeeVM
            .Projects
            .Single(x => x.Title == originalProject1Title);

         var projectVMRollbackPoint = project1VM.UndoManager.GetRollbackPoint();

         project1VM.Title = "ModifiedTitle";

         project1VM.UndoManager.RollbackTo(projectVMRollbackPoint);

         Assert.AreEqual(project1VM.Title, originalProject1Title);
         CollectionAssert.AreEqual(expectedModifiedProjectVMColl, EmployeeVM.Projects);

         EmployeeVM.UndoManager.RollbackTo(employeeVMRollbackPoint);
         CollectionAssert.AreEqual(expectedOriginalProjectVMColl, EmployeeVM.Projects);
      }


      //[TestMethod]
      //public void TempTest() {
      //   var projects = new Project[] {
      //      CreateProject("Project1"),
      //      CreateProject("Project2")
      //   };

      //   //EmployeeVM.InitializeFrom(CreateEmployee(projects: projects));

      //   EmployeeVM.SetSource(CreateEmployee(projects: projects), () => { });

      //   var rollbackPoint = EmployeeVM.UndoManager.GetRollbackPoint();


      //   EmployeeVM.Projects[0].Title = "dvsdvsv";
      //}

      //[TestMethod]
      //public void SetSource_AccessChildVMInSetSource_UndoManagerFromRootViewModelIsReachable() {
      //   EmployeeVM.SetSource(
      //      CreateEmployee(currentProject: CreateProject()),
      //      () => {
      //         EmployeeVM.SelectedProject.Title = string.Empty;
      //      });
      //}

      [TestMethod]
      public void AddItemToCollection_HandleItemAddedChangeAndModifiyItem_ModifiyActionIsOnTopOfAddedAction() {
         EmployeeVM = new EmployeeVM((vm, args) => {
            if (args.ChangeType == ChangeType.AddedToCollection) {
               var addedProjectVM = (ProjectVM)args.NewItems.Single();
               addedProjectVM.Title = "NewProjectTitle";
            }
         });

         EmployeeVM.InitializeFrom(CreateEmployee());

         var rollbackPoint = EmployeeVM.UndoManager.GetRollbackPoint();

         EmployeeVM.Projects.Add(new ProjectVM());

         var topMostUndoAction = EmployeeVM.UndoManager.TopMostAction;

         Assert.IsInstanceOfType(topMostUndoAction, typeof(SetValueAction<string>));
      }

      [TestMethod]
      public void RemoveItemToCollection_HandleItemRemovedChangeAndModifiyItem_VMofUndoActionDoesntHaveParentsAnymore() {
         EmployeeVM = new EmployeeVM((vm, args) => {
            if (args.ChangeType == ChangeType.RemovedFromCollection) {
               var removedProjectVM = (ProjectVM)args.OldItems.Single();
               removedProjectVM.Title = "this project will be removed";
            }
         });

         var projectTitle = "Title of project";
         EmployeeVM.InitializeFrom(CreateEmployee(projects: CreateProject(projectTitle)));

         var rollbackPoint = EmployeeVM.UndoManager.GetRollbackPoint();

         var projectToRemove = EmployeeVM.Projects.Single(x => x.Title == projectTitle);
         EmployeeVM.Projects.Remove(projectToRemove);

         var topMostUndoAction = EmployeeVM.UndoManager.TopMostAction;

         // ToDo: use own utils
         PrivateObject accessor = new PrivateObject(topMostUndoAction);
         IViewModel _vmOfUndoAction = (IViewModel)accessor.GetField("_vm");

         Assert.AreEqual(0, _vmOfUndoAction.Kernel.Parents.Count());
      }

      [TestMethod]
      public void RollbackTo_WithUserDefinedCompensationAction_ExecutesAction() {
         bool compensationActionCalled = false;

         Employee employee = CreateEmployee();

         EmployeeVM.InitializeFrom(employee);

         var rollbackPoint = EmployeeVM.UndoManager.GetRollbackPoint();

         Project newProject = EmployeeVM.Source.AddProjekt();

         Assert.IsTrue(EmployeeVM.Source.Projects.Contains(newProject));

         EmployeeVM.UndoManager.AddCompensationAction(() => {
            compensationActionCalled = true;
            EmployeeVM.Source.RemoveProjekt(newProject);
         });

         EmployeeVM.UndoManager.RollbackTo(rollbackPoint);

         Assert.IsTrue(compensationActionCalled);
         Assert.IsFalse(EmployeeVM.Source.Projects.Contains(newProject));
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