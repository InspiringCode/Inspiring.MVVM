namespace Inspiring.MvvmTest.ApiTests.ViewModels.Domain {
   using System;
   using System.Collections.Generic;
   using System.Linq;

   public sealed class RichText {
      public RichText(string html) {
         Html = html;
      }

      public string Html { get; private set; }
   }
}
