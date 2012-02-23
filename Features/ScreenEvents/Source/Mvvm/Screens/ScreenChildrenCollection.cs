namespace Inspiring.Mvvm.Screens {
   using System;
   using System.Collections;
   using System.Collections.Generic;
   using System.Collections.ObjectModel;
   using System.Diagnostics.Contracts;

   public class ScreenChildrenCollection<T> :
      ICollection<T> {

      private const bool ContineInvocations = true;
      private IScreenBase _parent;

      internal ScreenChildrenCollection(IScreenBase parent) {
         _parent = parent;
         ObservableItems = new ObservableCollection<T>();
      }
      
      int ICollection<T>.Count {
         get { return ObservableItems.Count; }
      }

      bool ICollection<T>.IsReadOnly {
         get { return true; }
      }

      internal ObservableCollection<T> ObservableItems { get; private set; }

      public void Add(T handler) {
         Contract.Requires<ArgumentNullException>(handler != null);

         // HACK
         IScreenBase screen = handler as IScreenBase;
         if (screen != null) {
            screen.Parent = _parent;
         }

         ObservableItems.Add(handler);
      }

      public TScreen AddNew<TScreen>(IScreenFactory<TScreen> screen)
         where TScreen : T, IScreenBase {
         TScreen s = screen.Create(null); // HACK

         // The screen is added AFTER it was initialized by the screen
         // factory.
         Add(s);

         return s;
      }

      //public bool Contains<TChild>()
      //   where TChild : IScreenLifecycle {
      //   return ObservableItems.OfType<TChild>().Any();
      //}

      //public TChild Expose<TChild>()
      //   where TChild : IScreenLifecycle {
      //   IEnumerable<TChild> children = ObservableItems.OfType<TChild>();
      //   switch (children.Count()) {
      //      case 0:
      //         throw new ArgumentException(
      //            ExceptionTexts.LifecycleTypeNotFound.FormatWith(typeof(TChild).Name)
      //         );
      //      case 1:
      //         return children.Single();
      //      default:
      //         throw new ArgumentException(
      //            ExceptionTexts.MoreThanOneLifecycleTypeFound.FormatWith(typeof(TChild).Name)
      //         );
      //   }
      //}

      public bool Remove(T handler) {
         return ObservableItems.Remove(handler);
      }

      //internal void ActivateAll(Action parentCallback = null) {
      //   Invoke("Activate", h => h.Activate(), parentCallback);
      //}

      //internal void DeactivateAll(Action parentCallback = null) {
      //   Invoke("Deactivate", h => h.Deactivate(), parentCallback);
      //}

      //internal bool RequestCloseAll(Func<bool> parentCallback = null) {
      //   bool result = true;

      //   Invoke(
      //      lifecycleMethodName: "RequestClose",
      //      methodInvocation: h => {
      //         bool okToClose = h.RequestClose();
      //         result &= okToClose;
      //         return okToClose;
      //      },
      //      parentCallback: () => {
      //         if (parentCallback != null) {
      //            bool okToClose = parentCallback();
      //            result &= okToClose;
      //            return okToClose;
      //         }

      //         return true;
      //      }
      //   );

      //   return result;
      //}

      //internal void CloseAll(Action parentCallback = null) {
      //   Invoke("Close", h => h.Close(), parentCallback);
      //}

      //internal void CorruptAll(object data = null, Action parentCallback = null) {
      //   Invoke("Corrupt", h => h.Corrupt(data), parentCallback);
      //}

      //internal void Invoke(
      //   string lifecycleMethodName,
      //   Action<T> methodInvocation,
      //   Action parentCallback
      //) {
      //   Func<T, bool> advancedMethodInvocation = target => {
      //      methodInvocation(target);
      //      return true;
      //   };

      //   Func<bool> advancedParentCallback = null;
      //   if (parentCallback != null) {
      //      advancedParentCallback = () => {
      //         parentCallback();
      //         return true;
      //      };
      //   }

      //   Invoke(
      //      lifecycleMethodName,
      //      advancedMethodInvocation,
      //      advancedParentCallback
      //   );
      //}

      ///// <param name="methodInvocation">
      /////   Return true to continue invocations.
      ///// </param>
      //internal void Invoke(
      //   string lifecycleMethodName,
      //   Func<T, bool> methodInvocation,
      //   Func<bool> parentCallback
      //) {
      //   parentCallback = parentCallback ?? (() => true);

      //   var methodArgs = ObservableItems
      //      .Select(target => new {
      //         Target = target,
      //         Order = InvocationOrderAttribute.GetOrder(target, lifecycleMethodName)
      //      })
      //      .OrderBy(i => i.Order)
      //      .ToArray();

      //   var beforeParent = methodArgs
      //      .Where(i => i.Order < InvocationOrder.Parent)
      //      .Select(i => new Func<bool>(() => methodInvocation(i.Target)));

      //   var afterParent = methodArgs
      //      .Where(i => i.Order >= InvocationOrder.Parent)
      //      .Select(i => new Func<bool>(() => methodInvocation(i.Target)));

      //   IEnumerable<Func<bool>> invocations = beforeParent
      //      .Union(new[] { parentCallback })
      //      .Union(afterParent);

      //   foreach (var i in invocations) {
      //      bool stop = !i();
      //      if (stop) {
      //         return;
      //      }
      //   }
      //}

      public IEnumerator<T> GetEnumerator() {
         throw new NotImplementedException();
      }

      IEnumerator IEnumerable.GetEnumerator() {
         throw new NotImplementedException();
      }
      
      void ICollection<T>.Clear() {
         ObservableItems.Clear();
      }

      bool ICollection<T>.Contains(T item) {
         return ObservableItems.Contains(item);
      }

      void ICollection<T>.CopyTo(T[] array, int arrayIndex) {
         ObservableItems.CopyTo(array, arrayIndex);
      }
   }
}
