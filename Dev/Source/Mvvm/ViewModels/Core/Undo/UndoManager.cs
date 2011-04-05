namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Collections.Generic;
   using System.Linq;

   internal sealed class UndoManager {
      private Stack<IUndoableAction> _actionStack = new Stack<IUndoableAction>();

      public UndoManager() {
         _actionStack.Push(new InitialPseudoAction());
      }

      /// <summary>
      ///  Returns the action at the top of the undo stack without removing it.
      /// </summary>
      public IUndoableAction GetCurrentAction() {
         return _actionStack.Peek();
      }

      /// <summary>
      ///   Sucht in dieser VM und allen Parents nach einem Undo Root Behavior.
      ///   Wenn in der gleichen Parent Ebene mehrer Undo Roots gefunden werden => exception
      ///   Wenn kein Undo Root gefunden wird => exception.
      ///   Sonst wird der Undo Root Manager in der am nächsten gelegen Ebene zurück gegeben.
      /// </summary>
      /// <exception cref="NotSupportedException">
      ///   More than one <cref name="UndoManager"/> is defined in the same ancestor level.
      /// </exception>
      /// <exception cref="InvalidOperationException">
      ///   Undo is enabled but no <cref name="UndoManager"/> is defined in the view model hierarchy.
      /// </exception>
      public static UndoManager GetManager(IViewModel vm) {
         IEnumerable<IViewModel> currentLevel = new IViewModel[] { vm };

         do {
            var undoRootBehaviors = currentLevel
               .Select(x => {
                  UndoRootBehavior b;
                  x.Descriptor.Behaviors.TryGetBehavior(out b);
                  return new { VM = x, Behavior = b };
               })
               .Where(x => x.Behavior != null)
               .ToArray();


            switch (undoRootBehaviors.Length) {
               case 0:
                  currentLevel = currentLevel
                     .SelectMany(x => x.Kernel.Parents)
                     .ToArray();
                  break;
               case 1:
                  var match = undoRootBehaviors.Single();
                  return match.Behavior.GetUndoManager(match.VM.GetContext());
               default:
                  throw new NotSupportedException(ExceptionTexts.MultipleUndoRoots);
            }
         } while (currentLevel.Any());

         throw new InvalidOperationException(ExceptionTexts.NoUndoRootManagerFound);
      }
   }
}