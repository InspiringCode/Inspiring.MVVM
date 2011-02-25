﻿namespace Inspiring.Mvvm.ViewModels.Fluent {
   using Inspiring.Mvvm.Common;
   using Inspiring.Mvvm.ViewModels.Core;

   /// <summary>
   ///   Creates <see cref="IVMProperty"/> objects of type <see 
   ///   cref="IVMCollection"/> that is synchronized with a source collection.
   /// </summary>
   /// <typeparam name="TVM">
   ///   The type of the VM for which properties are created.
   /// </typeparam>
   /// <typeparam name="TItemSource">
   ///   The item type of the source collection with which the <see 
   ///   cref="IVMCollection"/> is synchronized.
   /// </typeparam>
   /// <remarks>
   ///   Create an extension method on this interface if you want to support
   ///   synchronized properties for you own collection property type.
   /// </remarks>
   public interface IVMCollectionPropertyFactoryWithSource<TItemSource> :
      IHideObjectMembers,
      IConfigurationProvider {

      /// <summary>
      ///   Creates a the property.
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
      IVMProperty<IVMCollection<TItemVM>> Of<TItemVM>(VMDescriptorBase itemDescriptor)
         where TItemVM : IViewModel, ICanInitializeFrom<TItemSource>;
   }
}
