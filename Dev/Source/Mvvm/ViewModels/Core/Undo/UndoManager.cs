namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Collections.Generic;
   using System.Diagnostics.Contracts;
   using System.Linq;

   internal sealed class UndoManager {
      private Stack<IUndoableAction> _actionStack = new Stack<IUndoableAction>();
      private bool _undoing;

      public UndoManager() {
         _actionStack.Push(new InitialPseudoAction());
      }

      /// <summary>
      ///   Inserts an <see cref="IUndoableAction"> at the top of the undo stack.
      /// </summary>
      public void PushAction(IUndoableAction action) {
         if (_undoing) {
            return;
         }
         _actionStack.Push(action);
      }

      /// <summary>
      ///  Returns an <see cref="IUndoableAction"> at the top of the undo stack without removing it.
      /// </summary>
      public IUndoableAction GetCurrentAction() {
         return _actionStack.Peek();
      }

      /// <summary>
      ///    Removes and executes an <see cref="IUndoableAction"> beginning from the top of undo stack
      ///    till the rollback point is reached.
      /// </summary>
      /// <exception cref="ArgumentException">
      ///   Undo stack doesn't contain <see creff="IRollbackPoint"/>. 
      /// </exception>
      public void Undo(IRollbackPoint toPoint) {
         Contract.Requires<ArgumentException>(
            ContainsRollbackPoint(toPoint),
            ExceptionTexts.RollbackPointNotFound
          );

         _undoing = true;
         IUndoableAction action;
         action = _actionStack.Pop();
         while (!Object.ReferenceEquals(action, toPoint)) {
            action.Undo();
            action = _actionStack.Pop();
         }
         _actionStack.Push(action);
         _undoing = false;
      }

      public bool ContainsRollbackPoint(IRollbackPoint point) {
         Contract.Requires<ArgumentNullException>(point != null);
         return _actionStack.Contains(point);
      }

      /// <summary>
      ///   Returns the <see cref="UndoManager"/> defined in the view model or
      ///   nearest ancestor hierachy. 
      /// </summary>
      /// <exception cref="NotSupportedException">
      ///   More than one <see cref="UndoManager"/> is defined in the same ancestor level.
      /// </exception>
      /// <exception cref="InvalidOperationException">
      ///   Undo is enabled but no <see cref="UndoManager"/> is defined in the view model hierarchy.
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