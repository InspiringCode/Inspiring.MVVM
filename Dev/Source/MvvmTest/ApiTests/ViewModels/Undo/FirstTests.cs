namespace Inspiring.MvvmTest.ApiTests.ViewModels.Undo {
   using System.Linq;
   using Inspiring.Mvvm.ViewModels;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class FirstTests {

      [TestMethod]
      public void MyTestMethod() {
         Employee employee = new Employee { Name = "Employee" };

         Project project1 = new Project { Title = "Project1" };
         Project project2 = new Project { Title = "Project2" };

         Customer customer = new Customer(name: "Customer1");
         project1.Customer = customer;
         employee.Projects.Add(project1);
         employee.Projects.Add(project2);

         var vm = new EmployeeVM();
         vm.InitializeFrom(employee);

         vm.SetValue(x => x.Name, "NewName");
         var project1VM = vm.GetValue(x => x.Projects).SingleOrDefault(x => x.GetValue(d => d.Title) == "Project1");
         vm.SetValue(x => x.SelectedProject, project1VM);
         project1VM.SetValue(x => x.Title, "NewProject1");

         project1VM.GetValue(x => x.Customer).SetValue(x => x.Name, "New Customer");
      }
   }
}
