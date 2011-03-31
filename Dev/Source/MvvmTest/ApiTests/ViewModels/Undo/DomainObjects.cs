namespace Inspiring.MvvmTest.ApiTests.ViewModels.Undo {
   using System.Collections.Generic;

   public class Employee {

      public Employee() {
         Projects = new List<Project>();
      }

      public string Name { get; set; }
      public Project CurrentProject { get; set; }
      public List<Project> Projects { get; set; }
   }

   public class Project {
      public string Title { get; set; }
      public Customer Customer { get; set; }
   }

   public sealed class Customer {
      public Customer(string name = null) {
         Name = name;
      }

      public string Name { get; set; }
   }
}