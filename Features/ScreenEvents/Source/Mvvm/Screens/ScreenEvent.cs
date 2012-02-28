namespace Inspiring.Mvvm.Screens {
   using System;
   using System.Collections.Generic;
   using Inspiring.Mvvm.Common;

   public sealed class ScreenEvent<TArgs> :
      HierarchicalEvent<IScreenBase, TArgs>
      where TArgs : ScreenEventArgs {

      private readonly string _name;

      public ScreenEvent(string name) {
         _name = name;
      }

      public override string ToString() {
         return String.Format("{{ScreenEvent {0}}}", _name);
      }

      protected override IEnumerable<IScreenBase> GetHierarchyNodes(IScreenBase root) {
         return ScreenTreeHelper.GetDescendantsOf(root, includeSelf: true);
      }
   }
}
