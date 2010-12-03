using Inspiring.Mvvm.Common;
namespace Inspiring.Mvvm.ViewModels.Fluent {
   /// <summary>
   ///   Creates <see cref="VMProperty"/> objects that store their value in the
   ///   VM.
   /// </summary>
   /// <typeparam name="TVM">
   ///   The type of the VM for which properties are created.
   /// </typeparam>
   /// <remarks>
   ///   Create an extension method on this interface if you want to support
   ///   local properties for you own property type.
   /// </remarks>
   public interface ILocalVMPropertyFactory<TVM> :
      IHideObjectMembers,
      IConfigurationProvider
      where TVM : IViewModel {

      /// <summary>
      ///   Creates a simple <see cref="VMProperty"/>.
      /// </summary>
      /// <typeparam name="T">
      ///   The type of the property (for example <see cref="System.String"/>).
      /// </typeparam>
      VMProperty<T> Property<T>();

      /// <summary>
      ///   Creates a <see cref="VMProperty"/> that holds a child view model. The
      ///   child VM is resolved via the service locator of the parent VM and is
      ///   initialized properly (for example its Parent is set).
      /// </summary>
      /// <typeparam name="TChildVM">
      ///   The type of the child VM (for example PersonVM).
      /// </typeparam>
      VMProperty<TChildVM> VM<TChildVM>() where TChildVM : IViewModel;
   }
}
