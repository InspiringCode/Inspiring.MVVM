namespace Inspiring.MvvmContribTest.ApiTests.ViewModels {
   using System;

   internal sealed class Group {
      public Group(string name, bool isActive = true) {
         Name = name;
         IsActive = isActive;
      }

      public string Name { get; set; }

      public bool IsActive {
         get;
         set;
      }

      public override string ToString() {
         return String.Format("{{Group: {0}}}", Name);
      }
   }
}
