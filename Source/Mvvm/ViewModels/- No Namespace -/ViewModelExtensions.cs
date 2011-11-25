namespace Inspiring.Mvvm.ViewModels {
   using System.Collections.Generic;
   using System.Linq;

   public static class ViewModelExtensions {

      public static IEnumerable<IViewModel> GetAnchestors(this IViewModel vm) {
         List<IViewModel> parents = vm.Kernel.Parents.ToList();
         int i = 0;

         while (i < parents.Count) {
            yield return parents[i];
            parents.AddRange(parents[i].Kernel.Parents);
            i++;
         }
      }
   }
}
