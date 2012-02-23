namespace Inspiring.Mvvm.Screens {
   using System.Collections.Generic;
   using Inspiring.Mvvm.Common;

   public sealed class ScreenEvent<TArgs> :
      HierarchicalEvent<IScreenBase, TArgs>
      where TArgs : ScreenEventArgs {

      protected override IEnumerable<IScreenBase> GetHierarchyNodes(IScreenBase root) {
         return ScreenTreeHelper.GetDescendantsOf(root, includeSelf: true);
      }
   }
}
