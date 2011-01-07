namespace Inspiring.Mvvm.ViewModels.Core {
   using Inspiring.Mvvm.Common;

   /// <summary>
   ///   Provides a fluent interface to create collection <see cref="IVMProperty"/>
   ///   objects. This interface is returned by <see cref="ICollectionPropertyBuilder"/>.
   /// </summary>
   /// <typeparam name="TItemSource">
   ///   The type of the source value from which a collection item VM is initialized.
   /// </typeparam>
   public interface IPopulatedCollectionPropertyBuilder<TItemVM> :
      IHideObjectMembers
      where TItemVM : IViewModel {

      /// <summary>
      ///   Selects item descriptor and creates the property.
      /// </summary>
      /// <param name="itemDescriptor">
      ///   Specifies the VM descriptor that should be used for the collection 
      ///   items. All items must have the same descriptor.
      /// </param>
      IVMProperty<IVMCollection<TItemVM>> With(VMDescriptorBase itemDescriptor);
   }
}
