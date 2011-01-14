namespace Inspiring.Mvvm.Screens {
   using System.Collections.Generic;
   using System.Linq;

   internal sealed class LifecycleTreeWalker {
      /// <summary>
      /// Breadth-first
      /// </summary>
      public static IEnumerable<IScreenLifecycle> GetDescendants(IScreenLifecycle root) {
         Queue<IScreenLifecycle> queue = new Queue<IScreenLifecycle>();
         queue.Enqueue(root);

         while (queue.Count > 0) {
            IScreenLifecycle current = queue.Dequeue();
            yield return current;
            GetChildren(current).ForEach(queue.Enqueue);
         }
      }

      public static IEnumerable<IScreenLifecycle> GetChildren(IScreenLifecycle handler) {
         IScreen parent = handler as IScreen;
         return parent != null ?
            parent.Children.Items :
            Enumerable.Empty<IScreenLifecycle>();
      }

      public static IEnumerable<IScreenLifecycle> GetSelfAndChildren(IScreenLifecycle handler) {
         IScreen parent = handler as IScreen;

         if (parent == null) {
            return Enumerable.Empty<IScreenLifecycle>();
         }

         List<IScreenLifecycle> children = new List<IScreenLifecycle>();
         children.Add(parent);
         children.AddRange(parent.Children.Items);
         return children;
      }
   }
}
