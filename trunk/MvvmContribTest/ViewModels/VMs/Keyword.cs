namespace Inspiring.MvvmContribTest.ViewModels {
   using System;

   internal sealed class Keyword {
      public Keyword(string name) {
         Name = name;
      }

      public string Name { get; set; }

      public bool IsActive() {
         return true;
      }

      public override string ToString() {
         return String.Format("{{Keyword: {0}}}", Name);
      }
   }
}
