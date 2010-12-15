using System;
using System.Collections.Generic;
using Inspiring.Mvvm.Common;
using Inspiring.Mvvm.ViewModels.Core;
namespace Inspiring.Mvvm.ViewModels.Fluent {
   /// <summary>
   ///   Creates <see cref="VMProperty"/> objects of type <see 
   ///   cref="IVMCollection"/> that store their value in the VM.
   /// </summary>
   /// <typeparam name="TVM">
   ///   The type of the VM for which properties are created.
   /// </typeparam>
   /// <remarks>
   ///   Create an extension method on this interface if you want to support
   ///   local properties for you own collection property type.
   /// </remarks>
   public interface IVMCollectionPropertyFactory<TVM, TSource> :
      IHideObjectMembers,
      IConfigurationProvider
      where TVM : IViewModel {

      /// <summary>
      ///   Creates a the property.
      /// </summary>
      /// <typeparam name="TItemVM">
      ///   The type of a collection item VM.
      /// </typeparam>
      /// <remarks>
      ///   Create an extension method on this interface if you want to support
      ///   local properties for you own collection property type.
      /// </remarks>
      VMProperty<IVMCollection<TItemVM>> Of<TItemVM>(VMDescriptorBase itemDescriptor) where TItemVM : IViewModel;

      /// <summary>
      ///   Creates a <see cref="VMProperty"/> of type <see cref="IVMCollection"/>
      ///   whos items are synchronized with a source collection returned by the 
      ///   passed <paramref name="sourceCollectionSelector"/>. A collection property 
      ///   ensures that its item VMs are properly initialized (for example its 
      ///   Parent is set).
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
      IVMCollectionPropertyFactoryWithSource<TVM, TItemSource> Wraps<TItemSource>(
         Func<TSource, IEnumerable<TItemSource>> sourceCollectionSelector
      );

      /// <summary>
      ///   Creates a <see cref="VMProperty"/> of type <see cref="IVMCollection"/>
      ///   that is initialized with the items returned by the passed <paramref 
      ///   name="itemsProvider"/>.
      /// </summary>
      /// <param name="itemsProvider">
      ///   A function that returns the contents of the VM collection. It is called
      ///   the first time the collection is accessed or when <see 
      ///   cref="VMKernel.UpdateFromSource"/> is called.
      /// </param>
      VMProperty<IVMCollection<TItemVM>> InitializedBy<TItemVM>(
         Func<TSource, IEnumerable<TItemVM>> itemsProvider
      ) where TItemVM : IViewModel;
   }
}