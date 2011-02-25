﻿namespace Inspiring.Mvvm.Screens {
   using System;
   using System.Collections.Generic;
   using System.Collections.ObjectModel;
   using System.Diagnostics.Contracts;
   using System.Linq;

   public class ScreenLifecycleCollection<T> where T : IScreenLifecycle {
      private IScreenLifecycle _parent;

      internal ScreenLifecycleCollection(IScreenLifecycle parent) {
         _parent = parent;
         Items = new ObservableCollection<T>();
      }

      internal ObservableCollection<T> Items { get; private set; }

      public void Add(T handler) {
         Contract.Requires<ArgumentNullException>(handler != null);
         handler.Parent = _parent;
         Items.Add(handler);
      }

      public TScreen AddNew<TScreen>(IScreenFactory<TScreen> screen)
         where TScreen : T, IScreenBase {
         return screen.Create(s => {
            Add(s);
         });
      }

      public bool Contains<TChild>()
         where TChild : IScreenLifecycle {
         return Items.OfType<TChild>().Any();
      }

      public TChild Expose<TChild>()
         where TChild : IScreenLifecycle {
         IEnumerable<TChild> children = Items.OfType<TChild>();
         switch (children.Count()) {
            case 0:
               throw new ArgumentException(
                  ExceptionTexts.LifecycleTypeNotFound.FormatWith(typeof(TChild).Name)
               );
            case 1:
               return children.Single();
            default:
               throw new ArgumentException(
                  ExceptionTexts.MoreThanOneLifecycleTypeFound.FormatWith(typeof(TChild).Name)
               );
         }
      }

      public void Remove(T handler) {
         Items.Remove(handler);
      }

      internal void ActivateAll(Action parentCallback = null) {
         Invoke("Activate", h => h.Activate(), parentCallback);
      }

      internal void DeactivateAll(Action parentCallback = null) {
         Invoke("Deactivate", h => h.Deactivate(), parentCallback);
      }

      internal bool RequestCloseAll(Func<bool> parentCallback = null) {
         List<bool> results = new List<bool>();

         Invoke(
            lifecycleMethodName: "RequestClose",
            methodInvocation: h => results.Add(h.RequestClose()),
            parentCallback: () => {
               if (parentCallback != null) {
                  results.Add(parentCallback());
               }
            }
         );

         return results.All(r => r);
      }

      internal void CloseAll(Action parentCallback = null) {
         Invoke("Close", h => h.Close(), parentCallback);
      }

      internal void Invoke(
         string lifecycleMethodName,
         Action<T> methodInvocation,
         Action parentCallback
      ) {
         var invocations = Items
            .Select(h => new {
               Handler = h,
               Order = InvocationOrderAttribute.GetOrder(h, lifecycleMethodName)
            })
            .OrderBy(i => i.Order)
            .ToArray();

         invocations
            .Where(i => i.Order < InvocationOrder.Parent)
            .ForEach(i => methodInvocation(i.Handler));

         if (parentCallback != null) {
            parentCallback();
         }

         invocations
            .Where(i => i.Order >= InvocationOrder.Parent)
            .ForEach(i => methodInvocation(i.Handler));
      }
   }
}
