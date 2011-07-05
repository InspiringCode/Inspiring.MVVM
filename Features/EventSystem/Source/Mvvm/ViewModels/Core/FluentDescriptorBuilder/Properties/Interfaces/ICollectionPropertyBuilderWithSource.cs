namespace Inspiring.Mvvm.ViewModels.Core {
   using Inspiring.Mvvm.Common;

   /// <summary>
   ///   Provides a fluent interface to create collection <see cref="IVMPropertyDescriptor"/>
   ///   objects. This interface is returned by <see cref="ICollectionPropertyBuilder"/>.
   /// </summary>
   /// <typeparam name="TItemSource">
   ///   The type of the source value from which a collection item VM is initialized.
   /// </typeparam>
   public interface ICollectionPropertyBuilderWithSource<TItemSource> :
      IHideObjectMembers {

      /// <summary>
      ///   Selects the type of the collection item.
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
      IVMPropertyDescriptor<IVMCollection<TItemVM>> With<TItemVM>(IVMDescriptor itemDescriptor)
         where TItemVM : IViewModel, IHasSourceObject<TItemSource>;
   }
}
