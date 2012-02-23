namespace Inspiring.Mvvm.Screens {
   using System.Collections.Generic;
   using System.Linq;

   internal static class ScreenTreeHelper {
      public static IEnumerable<IScreenBase> GetChildrenOf(IScreenBase screen, bool includeSelf = false) {
         IEnumerable<IScreenBase> children = screen
            .Children
            .OfType<IScreenBase>();

         return includeSelf ?
            new[] { screen }.Union(children) :
            children;
      }

      public static IEnumerable<IScreenBase> GetAncestorsOf(IScreenBase screen, bool includeSelf = false) {
         if (!includeSelf) {
            screen = screen.Parent;
         }

         for (IScreenBase s = screen; s != null; s = s.Parent) {
            yield return s;
         }
      }

      /// <summary>
      ///   Breadth-first traversation.
      /// </summary>
      public static IEnumerable<IScreenBase> GetDescendantsOf(IScreenBase root, bool includeSelf = false) {
         if (includeSelf) {
            yield return root;
         }

         Queue<IScreenBase> queue = new Queue<IScreenBase>();
         queue.Enqueue(root);

         while (queue.Count > 0) {
            IScreenBase current = queue.Dequeue();

            foreach (IScreenBase child in GetChildrenOf(current)) {
               yield return current;
               queue.Enqueue(child);
            }
         }
      }
   }
}