﻿namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Collections.Generic;
   using System.Linq;

   public sealed class UndoManager {
      private Stack<IUndoableAction> _actionStack = new Stack<IUndoableAction>();
      private HashSet<IRollbackPoint> _rollbackPoints = new HashSet<IRollbackPoint>();
      private bool _undoing;

      internal UndoManager() {
         _actionStack.Push(new InitialPseudoAction());
      }

      /// <summary>
      ///  Returns an <see cref="IUndoableAction"/> at the top of the undo stack without removing it.
      /// </summary>
      internal IUndoableAction TopMostAction {
         get { return _actionStack.Peek(); }
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

         //throw new InvalidOperationException(ExceptionTexts.NoUndoRootManagerFound);
         return null;
      }

      /// <summary>
      ///  Returns an <see cref="IRollbackPoint"/> at the top of the undo stack without removing it.
      /// </summary>
      public IRollbackPoint GetRollbackPoint() {
         var rollbackPoint = _actionStack.Peek();
         _rollbackPoints.Add(rollbackPoint);
         return rollbackPoint;
      }

      /// <summary>
      ///    Removes and executes an <see cref="IUndoableAction"/> beginning from the top of undo stack
      ///    till the rollback point is reached.
      /// </summary>
      /// <exception cref="ArgumentException">
      ///   Undo stack doesn't contain <see cref="IRollbackPoint"/>. 
      /// </exception>
      public void RollbackTo(IRollbackPoint toPoint) {
         Check.Requires(
            ContainsRollbackPoint(toPoint),
            ExceptionTexts.RollbackPointNotFound
          );
         _rollbackPoints.Remove(toPoint);
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
         Check.NotNull(point, nameof(point));
         return _actionStack.Contains(point);
      }

      public void AddCompensationAction(Action action) {
         PushAction(new UserDefinedCompensationAction(action));
      }

      /// <summary>
      ///   Inserts an <see cref="IUndoableAction"/> at the top of the undo stack.
      /// </summary>
      internal void PushAction(IUndoableAction action) {
         if (_undoing || _rollbackPoints.Count == 0) {
            return;
         }
         _actionStack.Push(action);
      }
   }
}