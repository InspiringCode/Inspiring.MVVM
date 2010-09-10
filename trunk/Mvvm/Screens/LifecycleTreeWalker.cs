namespace Inspiring.Mvvm.Screens {
   using System.Collections.Generic;
   using System.Linq;

   internal sealed class LifecycleTreeWalker {
      /// <summary>
      /// Breadth-first
      /// </summary>
      public static IEnumerable<ILifecycleHandler> GetDescendants(ILifecycleHandler root) {
         Queue<ILifecycleHandler> queue = new Queue<ILifecycleHandler>();
         queue.Enqueue(root);

         while (queue.Count > 0) {
            ILifecycleHandler current = queue.Dequeue();
            yield return current;
            GetChildren(current).ForEach(queue.Enqueue);
         }
      }

      public static IEnumerable<ILifecycleHandler> GetChildren(ILifecycleHandler handler) {
         ParentLifecycleHandler parent = handler as ParentLifecycleHandler;
         return parent != null ?
            parent.Children.Items :
            Enumerable.Empty<ILifecycleHandler>();
      }
   }
}
