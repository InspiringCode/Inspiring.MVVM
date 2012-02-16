namespace Inspiring.Mvvm.Screens {
   using System.Collections.Generic;
   using Inspiring.Mvvm.Common;

   public sealed class ScreenEvent<TArgs> :
      HierarchicalEvent<IScreenLifecycle, ScreenEventArgs>
      where TArgs : ScreenEventArgs {


      protected override IEnumerable<IScreenLifecycle> GetHierarchyNodes(IScreenLifecycle root) {
         throw new System.NotImplementedException();
      }
   }
}
