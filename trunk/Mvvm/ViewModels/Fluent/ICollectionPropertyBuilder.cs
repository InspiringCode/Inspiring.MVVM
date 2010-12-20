﻿namespace Inspiring.Mvvm.ViewModels.Fluent {
   using System;
   using System.Collections.Generic;
   using Inspiring.Mvvm.ViewModels.Core;

   /// <summary>
   ///   Provides a fluent interface to create collection <see cref="VMProperty"/>
   ///   objects.
   /// </summary>
   /// <typeparam name="TSourceObject">
   ///   The type of source objects as selected by the <see 
   ///   cref="IVMPropertyBuilderProvider.GetPropertyBuilder"/> method call 
   ///   used to create the <see cref="IVMPropertyBuilder"/>.
   /// </typeparam>
   public interface ICollectionPropertyBuilder<TSourceObject> {
      /// <summary>
      ///   Creates a <see cref="VMProperty"/> of type <see cref="IVMCollection"/>
      ///   whos items are synchronized with a source collection returned by the 
      ///   passed <paramref name="sourceCollectionSelector"/>.
      /// </summary>
      /// <param name="sourceCollectionSelector">
      ///   <para>A function that should return the source collection with which 
      ///      the <see cref="IVMCollection"/> is synchronized. This may be the
      ///      value of a source object collection or you may create and return
      ///      a new collection instance.</para> 
      ///   <para>The <see cref="IViewModel"/> or some object referenced by it (as 
      ///      defined by the <see cref="IVMPropertyFactoryProvider.GetFactory"/>
      ///      method) is passed to the delegate.</para>  
      /// </param>
      ICollectionPropertyBuilderWithSource<TItemSource> Wraps<TItemSource>(
         Func<TSourceObject, IEnumerable<TItemSource>> sourceCollectionSelector
      );

      /// <summary>
      ///   Creates a <see cref="VMProperty"/> of type <see cref="IVMCollection"/>
      ///   that is initialized with the items returned by the passed <paramref 
      ///   name="itemsProvider"/>.
      /// </summary>
      /// <typeparam name="TItemVM">
      ///   The type of the collection item VM (for example PersonVM). A 
      ///   new instance of <typeparamref name="TVM"/> is created for each item 
      ///   of the source collection using the service locator of the parent VM,
      ///   <see cref="ICanInitializeFrom.InitializeFrom"/> is called with the 
      ///   source item and the item VM is added to the collection.
      /// </typeparam>
      /// <param name="initialItemsProvider">
      ///   A function that returns the contents of the VM collection. It is called
      ///   the first time the collection is accessed or when <see 
      ///   cref="VMKernel.UpdateFromSource"/> is called.
      /// </param>
      /// <param name="itemDescriptor">
      ///   Specifies the VM descriptor that should be used for the collection 
      ///   items. All items must have the same descriptor.
      /// </param>
      VMProperty<IVMCollection<TItemVM>> InitializedWith<TItemVM>(
         Func<TSourceObject, IEnumerable<TItemVM>> initialItemsProvider,
         VMDescriptorBase itemDescriptor
      ) where TItemVM : IViewModel;

      /// <summary>
      ///   Creates a <see cref="VMProperty"/> of type <see cref="IVMCollection"/>.
      /// </summary>
      /// <typeparam name="TItemVM">
      ///   The type of the collection item VM (for example PersonVM). A 
      ///   new instance of <typeparamref name="TVM"/> is created for each item 
      ///   of the source collection using the service locator of the parent VM,
      ///   <see cref="ICanInitializeFrom.InitializeFrom"/> is called with the 
      ///   source item and the item VM is added to the collection.
      /// </typeparam>
      /// <param name="itemDescriptor">
      ///   Specifies the VM descriptor that should be used for the collection 
      ///   items. All items must have the same descriptor.
      /// </param>
      VMProperty<IVMCollection<TItemVM>> Of<TItemVM>(
         VMDescriptorBase itemDescriptor
      ) where TItemVM : IViewModel;
   }
}
