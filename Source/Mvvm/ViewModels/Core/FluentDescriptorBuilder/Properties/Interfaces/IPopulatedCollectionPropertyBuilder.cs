namespace Inspiring.Mvvm.ViewModels.Core {
   using Inspiring.Mvvm.Common;

   /// <summary>
   ///   Provides a fluent interface to create collection <see cref="IVMPropertyDescriptor{T}"/>
   ///   objects. This interface is returned by <see cref="ICollectionPropertyBuilder{TSourceObject}"/>.
   /// </summary>
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
      IVMPropertyDescriptor<IVMCollection<TItemVM>> With(IVMDescriptor itemDescriptor);
   }
}
