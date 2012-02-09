namespace Inspiring.Mvvm.ViewModels.Core {
   using System;

   public interface IPathDefinitionBuilder<TDescriptor>
      where TDescriptor : IVMDescriptor {

      void Properties(params Func<TDescriptor, IVMPropertyDescriptor>[] targetPropertySelectors);

      /// <summary>
      /// </summary>
      /// <param name="viewModelSelector">
      ///   The given function should return a child VM property.
      /// </param>
      /// <typeparam name="D">
      ///   The descriptor type of the child VM. Can be inferred by the compiler.
      /// </typeparam>
      IPathDefinitionBuilder<D> Descendant<D>(
         Func<TDescriptor, IVMPropertyDescriptor<IViewModel<D>>> viewModelSelector
      ) where D : IVMDescriptor;

      /// <summary>
      /// </summary>
      /// <param name="viewModelSelector">
      ///   The given function should return a collection VM property.
      /// </param>
      /// <typeparam name="D">
      ///   The descriptor type of the collection VM. Can be inferred by the compiler.
      /// </typeparam>
      IPathDefinitionBuilder<D> Descendant<D>(
         Func<TDescriptor, IVMPropertyDescriptor<IVMCollectionExpression<IViewModelExpression<D>>>> collectionSelector
      ) where D : IVMDescriptor;
   }
}
