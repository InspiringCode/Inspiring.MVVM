namespace Inspiring.Mvvm.ViewModels {
   using System;
   using System.Collections.Generic;
   using System.Linq;

   public static class ViewModelTreeHelper {
      public static IEnumerable<IViewModel> GetParents(IViewModel vm) {
         return vm.Kernel.Parents;
      }

      public static IEnumerable<IViewModel> GetAncestors(IViewModel vm) {
         return vm.TraverseBreadthFirst(x => GetParents(x));
      }
   }
}
