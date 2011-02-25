namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Collections.Generic;
   using System.Linq;

   internal interface ISupportsValidation {
      bool IsValid(bool validateChildren);
   }
}
