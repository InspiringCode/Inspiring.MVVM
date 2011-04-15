﻿namespace Inspiring.Mvvm.ViewModels {
   using System.Collections;
   using System.Collections.Generic;
   using Inspiring.Mvvm.ViewModels.Core;

   public interface IVMCollection : IEnumerable {
      /// <summary>
      ///   Gets the view model instance that holds this collection instance. 
      ///   The <see cref="OwnerVM"/> is the <see cref="IViewModel.Parent"/>
      ///   of all items.
      /// </summary>
      IViewModel OwnerVM { get; }

      // TODO: Comment.
      IVMPropertyDescriptor OwnerProperty { get; }
   }

   /// <summary>
   ///   An interface that should be implemented by collection classes that are
   ///   be used in VMs to hold a collection of child VMs. This interface is
   ///   especially required by the predefined collection behaviors.
   /// </summary>
   public interface IVMCollection<TItemVM> : IVMCollection, IList<TItemVM>, IList, IVMCollectionExpression<TItemVM> {
      int Count { get; }
      void Clear();
      void RemoveAt(int index);
      /// <summary>
      ///   Clears the collections and adds the <paramref name="newItems"/>.
      /// </summary>
      /// <param name="newItems"></param>
      void ReplaceItems(IEnumerable<TItemVM> newItems);
      TItemVM this[int index] { get; set; }
   }
}
