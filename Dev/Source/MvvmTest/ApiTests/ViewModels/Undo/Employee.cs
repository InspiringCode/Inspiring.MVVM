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
}
