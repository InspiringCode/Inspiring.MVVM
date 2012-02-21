namespace Inspiring.Mvvm.Screens {
   using System.Collections.Generic;
   using System.Linq;
   using Inspiring.Mvvm.Common;

   public sealed class ScreenEvent<TArgs> :
      HierarchicalEvent<IScreenBase, TArgs>
      where TArgs : ScreenEventArgs {

      protected override IEnumerable<IScreenBase> GetHierarchyNodes(IScreenBase root) {
         return LifecycleTreeWalker
            .GetDescendants(root)
            .OfType<IScreenBase>();
      }
   }
}
