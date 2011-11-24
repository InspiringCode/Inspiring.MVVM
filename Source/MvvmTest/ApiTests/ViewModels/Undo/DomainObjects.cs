namespace Inspiring.MvvmTest.ApiTests.ViewModels.Undo {
   using System;
   using System.Collections.Generic;
   using System.Diagnostics.Contracts;

   public class Employee {

      public Employee() {
         Projects = new List<Project>();
      }

      public string Name { get; set; }
      public Project CurrentProject { get; set; }
      public List<Project> Projects { get; set; }

      public Project AddProjekt() {
         var projekt = new Project();
         Projects.Add(projekt);

         return projekt;
      }

      public void RemoveProjekt(Project projekt) {
         Projects.Remove(projekt);
      }

      [Pure]
      public bool Contains(Project projekt) {
         Contract.Requires<ArgumentNullException>(projekt != null);
         return Projects.Contains(projekt);
      }
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