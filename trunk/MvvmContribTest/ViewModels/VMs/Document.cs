namespace Inspiring.MvvmContribTest.ViewModels {
   using System;
   using System.Collections.Generic;

   internal sealed class Document {
      public Document(string name) {
         Name = name;
         Keywords = new List<Keyword>();
      }

      public string Name { get; set; }

      public ICollection<Keyword> Keywords { get; private set; }

      public override string ToString() {
         return String.Format("{{Document: {0}}}", Name);
      }
   }
}
