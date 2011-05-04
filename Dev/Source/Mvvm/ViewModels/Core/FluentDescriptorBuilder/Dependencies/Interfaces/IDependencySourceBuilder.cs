namespace Inspiring.Mvvm.ViewModels.Core {
   using System;

   public interface IDependencySourceBuilder<TRootVM, TSourceVM, TRootDescriptor, TSourceDescriptor>
      where TRootVM : IViewModel
      where TSourceVM : IViewModel
      where TRootDescriptor : IVMDescriptor
      where TSourceDescriptor : IVMDescriptor {

      /// <summary>
      ///   Selects one or more properties for attaching a change action.
      /// </summary>
      IDependencyActionBuilder<TRootVM, TRootDescriptor> Properties(
         params Func<TSourceDescriptor, IVMPropertyDescriptor>[] sourcePropertySelectors
      );

      /// <summary>
      ///   Selects a child VM to specifiy which change of one or more of its properties or any descandants
      ///   is of interest
      ///   Calls may be chained to select any descendant VM.
      /// </summary>
      /// <param name="viewModelSelector">
      ///   The given function should return a child VM property.
      /// </param>
      /// <typeparam name="D">
      ///   The descriptor type of the child VM. Can be inferred by the compiler.
      /// </typeparam>
      IDependencySourceOrAnyDescendantBuilder<TRootVM, IViewModel<D>, TRootDescriptor, D> Descendant<D>(
         Func<TSourceDescriptor, IVMPropertyDescriptor<IViewModel<D>>> viewModelSelector
      ) where D : IVMDescriptor;

      /// <summary>
      ///   Selects a collection VM to specifiy which change of one or more of its properties or any descandants
      ///   is of interest.
      ///   Calls may be chained to select any descendant VM.
      /// </summary>
      /// <param name="viewModelSelector">
      ///   The given function should return a collection VM property.
      /// </param>
      /// <typeparam name="D">
      ///   The descriptor type of the collection VM. Can be inferred by the compiler.
      /// </typeparam>
      IDependencySourceOrAnyDescendantBuilder<TRootVM, IViewModel<D>, TRootDescriptor, D> Descendant<D>(
         Func<TSourceDescriptor, IVMPropertyDescriptor<IVMCollectionExpression<IViewModelExpression<D>>>> collectionSelector
      ) where D : IVMDescriptor;

      IDependencyActionBuilder<TRootVM, TRootDescriptor> Collection<D>(
         Func<TSourceDescriptor, IVMPropertyDescriptor<IVMCollectionExpression<IViewModelExpression<D>>>> collectionSelector
      ) where D : IVMDescriptor;

      //IChangeActionBuilder<TRootVM, TRootDescriptor> AnyProperty { get; }

      //IChangeActionBuilder<TRootVM, TRootDescriptor> AnyPropertyOrDescendant { get; }

      //IDependencyActionBuilder<TRootVM, TRootDescriptor> Self { get; }

      //IDependencyActionBuilder<TRootVM, TRootDescriptor> SelfOrDescendant { get; }
   }
}