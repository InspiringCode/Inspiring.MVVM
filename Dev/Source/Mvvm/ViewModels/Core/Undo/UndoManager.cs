namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Collections.Generic;
   using System.Linq;

   internal sealed class UndoManager {

      public static UndoRootBehavior GetUndoRootBehavior(IViewModel vm) {

         IEnumerable<IViewModel> currentLevel = new IViewModel[] { vm };
         List<UndoRootBehavior> undoRootBehaviors = new List<UndoRootBehavior>();

         do {
            undoRootBehaviors.Clear();
            currentLevel
               .Select(x => x.Descriptor.Behaviors)
               .ForEach((b) => {
                  UndoRootBehavior undoRoot = null;
                  if (b.TryGetBehavior(out undoRoot)) {
                     undoRootBehaviors.Add(undoRoot);
                  }
               });

            if (undoRootBehaviors.Count == 0) {
               currentLevel = currentLevel
                  .SelectMany(x => x.Kernel.Parents)
                  .ToArray();
            } else if (undoRootBehaviors.Count == 1) {
               break;
            } else {
               throw new NotSupportedException(ExceptionTexts.MultipleUndoRoots);
            }

         } while (currentLevel.Any());

         return undoRootBehaviors.SingleOrDefault();
      }
   }
}