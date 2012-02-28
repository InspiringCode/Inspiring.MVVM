namespace Inspiring.Mvvm.Screens {
   using System;
   using System.Collections.Generic;
   using Inspiring.Mvvm.Common;

   public sealed class ScreenLifecycleEvent<TArgs> :
      HierarchicalEvent<IScreenBase, TArgs>
      where TArgs : ScreenEventArgs {

      private readonly string _name;

      public ScreenLifecycleEvent(string name) {
         _name = name;
      }

      public override string ToString() {
         return String.Format("{{LifecycleEvent {0}}}", _name);
      }

      protected override IEnumerable<IScreenBase> GetHierarchyNodes(IScreenBase root) {
         return ScreenTreeHelper.GetDescendantsOf(root, includeSelf: true);
      }
   }
}
