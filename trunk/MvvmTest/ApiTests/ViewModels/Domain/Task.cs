namespace Inspiring.MvvmTest.ApiTests.ViewModels.Domain {
   using System;
   using System.Collections.Generic;
   using System.Linq;

   public sealed class Task {
      public Task() {
      }

      public Task(string title, string descriptionHtml) {
         Title = title;
         Description = new RichText(descriptionHtml);
      }

      public string Title { get; set; }
      public RichText Description { get; set; }
   }
}
