namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Collections.Generic;

   /// <summary>
   ///   Provides a fluent interface to create collection <see cref="IVMPropertyDescriptor"/>
   ///   objects.
   /// </summary>
   /// <typeparam name="TSourceObject">
   ///   The type of source objects as selected by the <see 
   ///   cref="IVMPropertyBuilderProvider.GetPropertyBuilder"/> method call 
   ///   used to create the <see cref="IVMPropertyBuilder"/>.
   /// </typeparam>
   public interface ICollectionPropertyBuilder<TSourceObject> {
      /// <summary>
      ///   Creates a <see cref="IVMPropertyDescriptor"/> of type <see cref="IVMCollection"/>
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
      /// <param name="cacheSourceCollection">
      ///   <para>If false, the <paramref name="sourceCollectionSelector"/> is executed 
      ///      for each collection operation (population, add, remove, ...). This is 
      ///      appropriate if the source collection instance may be replaced behind the 
      ///      scenes (e.g. when the collection is mapped with NHibernate).</para>
      ///   <para>If true, the <paramref name="sourceCollectionSelector"/> is only 
      ///      executed when the collection is populated or refreshed. This is 
      ///      appropriate if the source collection is constructed on the fly.</para>
      /// </param>
      ICollectionPropertyBuilderWithSource<TItemSource> Wraps<TItemSource>(
         Func<TSourceObject, IEnumerable<TItemSource>> sourceCollectionSelector,
         bool cacheSourceCollection = false
      );

      /// <summary>
      ///   Creates a <see cref="IVMPropertyDescriptor"/> of type <see cref="IVMCollection"/>
      ///   that is initialized with the items returned by the passed <paramref 
      ///   name="itemsProvider"/>.
      /// </summary>
      /// <param name="itemsProvider">
      ///   A function that returns the contents of the VM collection. It is called
      ///   the first time the collection is accessed or when <see 
      ///   cref="VMKernel.UpdateFromSource"/> is called.
      /// </param>
      IPopulatedCollectionPropertyBuilder<TItemVM> PopulatedWith<TItemVM>(
         Func<TSourceObject, IEnumerable<TItemVM>> itemsProvider
      ) where TItemVM : IViewModel;

      /// <summary>
      ///   Creates a <see cref="IVMPropertyDescriptor"/> of type <see cref="IVMCollection"/>.
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
      IVMPropertyDescriptor<IVMCollection<TItemVM>> Of<TItemVM>(
         IVMDescriptor itemDescriptor
      ) where TItemVM : IViewModel;
   }
}
