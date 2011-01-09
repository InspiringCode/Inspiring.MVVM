namespace Inspiring.Mvvm.ViewModels.Core {
   /// <summary>
   ///   Provides a fluent interface to create <see cref="IVMPropertyDescriptor"/> objects.
   ///   This interface is returned by <see cref="IViewModelPropertyBuilder"/>.
   /// </summary>
   /// <typeparam name="TSourceValue">
   ///   The type of the source value from which the VM is initialized.
   /// </typeparam>
   public interface IViewModelPropertyBuilderWithSource<TSourceValue> {
      /// <summary>
      ///   Selects the type of the child VM (for example PersonVM).
      /// </summary>
      /// <typeparam name="TChildVM">
      ///   A new instance of <typeparamref name="TChildVM"/> is created using 
      ///   the <see cref="IServiceLocator"/> of this VM. <see 
      ///   cref="ICanInitializeFrom.InitializeFrom"/> is called with the source 
      ///   object just selected.
      /// </typeparam>
      IVMPropertyDescriptor<TChildVM> With<TChildVM>() where TChildVM : IViewModel, ICanInitializeFrom<TSourceValue>, IVMCollectionItem<TSourceValue>; // TODO: Interface!
   }
}
