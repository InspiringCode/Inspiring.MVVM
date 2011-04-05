namespace Inspiring.MvvmTest.ApiTests.ViewModels.Undo {
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class UndoTests {
      private EmployeeVM EmployeeVM { get; set; }

      [TestInitialize]
      public void Setup() {
         Employee employee = new Employee { Name = "Employee" };

         Project project1 = new Project { Title = "Project1" };
         Project project2 = new Project { Title = "Project2" };

         Customer customer = new Customer(name: "Customer1");
         project1.Customer = customer;
         employee.Projects.Add(project1);
         employee.Projects.Add(project2);

         EmployeeVM = new EmployeeVM();
         EmployeeVM.InitializeFrom(employee);
      }

      [TestMethod]
      public void GetRollbackPoint_NoModifications_ReturnsPseudoAction() {
         var rollbackPoint = ((IViewModel)EmployeeVM).Kernel.GetRollbackPoint();
         Assert.IsInstanceOfType(rollbackPoint, typeof(InitialPseudoAction));
      }
   }
}
