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

      public override bool Equals(object obj) {
         Department other = obj as Department;
         return other != null && other.Name == Name;
      }

      public override int GetHashCode() {
         return (Name ?? String.Empty).GetHashCode();
      }

      public override string ToString() {
         return String.Format("{{Department: {0}}}", Name);
      }
   }
}
