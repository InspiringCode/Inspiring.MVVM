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
   public interface IVMCollectionPropertyFactory<TVM> :
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
   }
}