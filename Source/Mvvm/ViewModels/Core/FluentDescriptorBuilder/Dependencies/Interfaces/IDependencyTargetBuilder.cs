namespace Inspiring.Mvvm.ViewModels.Core {
   using System;

   public interface IDependencyTargetBuilder<TRootVM, TSourceVM, TRootDescriptor, TSourceDescriptor>
      where TRootVM : IViewModel
      where TSourceVM : IViewModel
      where TRootDescriptor : IVMDescriptor
      where TSourceDescriptor : IVMDescriptor {

      void Properties(params Func<TSourceDescriptor, IVMPropertyDescriptor>[] targetPropertySelectors);

      void Self();

      /// <summary>
      /// </summary>
      /// <param name="viewModelSelector">
      ///   The given function should return a child VM property.
      /// </param>
      /// <typeparam name="D">
      ///   The descriptor type of the child VM. Can be inferred by the compiler.
      /// </typeparam>
      IDependencyTargetBuilder<TRootVM, IViewModel<D>, TRootDescriptor, D> Descendant<D>(
         Func<TSourceDescriptor, IVMPropertyDescriptor<IViewModel<D>>> viewModelSelector
      ) where D : IVMDescriptor;

      /// <summary>
      /// </summary>
      /// <param name="viewModelSelector">
      ///   The given function should return a collection VM property.
      /// </param>
      /// <typeparam name="D">
      ///   The descriptor type of the collection VM. Can be inferred by the compiler.
      /// </typeparam>
      IDependencyTargetBuilder<TRootVM, IViewModel<D>, TRootDescriptor, D> Descendant<D>(
         Func<TSourceDescriptor, IVMPropertyDescriptor<IVMCollectionExpression<IViewModelExpression<D>>>> collectionSelector
      ) where D : IVMDescriptor;
   }
}
