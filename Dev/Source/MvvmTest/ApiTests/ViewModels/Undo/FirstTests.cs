namespace Inspiring.MvvmTest.ApiTests.ViewModels.Undo {
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class FirstTests {

      [TestMethod]
      public void MyTestMethod() {
         Employee employee = new Employee { Name = "Employee" };

         Project project1 = new Project { Title = "Project1" };
         Project project2 = new Project { Title = "Project2" };

         employee.Projects.Add(project1);
         employee.Projects.Add(project2);

         var vm = new EmployeeVM();
         vm.InitializeFrom(employee);
      }
   }
}
