namespace Inspiring.MvvmContribTest.ApiTests.ViewModels {
   using System;

   internal sealed class Department {
      public Department(string name, bool isActive = true) {
         Name = name;
         IsActive = isActive;
      }

      public string Name { get; set; }

      public bool IsActive {
         get;
         set;
      }

      public override string ToString() {
         return String.Format("{{Department: {0}}}", Name);
      }
   }
}
