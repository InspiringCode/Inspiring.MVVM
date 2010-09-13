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
         ParentScreenLifecycle parent = handler as ParentScreenLifecycle;
         return parent != null ?
            parent.Children.Items :
            Enumerable.Empty<IScreenLifecycle>();
      }
   }
}
