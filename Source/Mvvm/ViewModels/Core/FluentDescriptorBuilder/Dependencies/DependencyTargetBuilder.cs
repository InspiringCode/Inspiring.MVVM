namespace Inspiring.Mvvm.ViewModels.Core {
   using System;

   internal sealed class DependencyTargetBuilder<TRootVM, TSourceVM, TRootDescriptor, TSourceDescriptor> :
      //IDependencyTargetBuilder<TRootVM, TSourceVM, TRootDescriptor, TSourceDescriptor>,
      IRefreshTargetBuilder<TRootVM, TSourceVM, TRootDescriptor, TSourceDescriptor>
      where TRootVM : IViewModel
      where TSourceVM : IViewModel
      where TRootDescriptor : IVMDescriptor
      where TSourceDescriptor : IVMDescriptor {

      private readonly DependencyBuilderOperation _context;

      public DependencyTargetBuilder(DependencyBuilderOperation context) {
         _context = context;
      }

      public void Properties(params Func<TSourceDescriptor, IVMPropertyDescriptor>[] targetPropertySelectors) {
         _context.AddTargetsProperties(targetPropertySelectors);
      }

      public IDependencyTargetBuilder<TRootVM, IViewModel<D>, TRootDescriptor, D> Descendant<D>(
         Func<TSourceDescriptor, IVMPropertyDescriptor<IViewModel<D>>> viewModelSelector
      ) where D : IVMDescriptor {
         _context.AddDescendantTargetStep<TSourceDescriptor, IViewModel<D>, D>(viewModelSelector);
         return new DependencyTargetBuilder<TRootVM, IViewModel<D>, TRootDescriptor, D>(_context);
      }

      public IDependencyTargetBuilder<TRootVM, IViewModel<D>, TRootDescriptor, D> Descendant<D>(
         Func<TSourceDescriptor, IVMPropertyDescriptor<IVMCollectionExpression<IViewModelExpression<D>>>> collectionSelector
      ) where D : IVMDescriptor {
         _context.AddDescendantTargetStep<TSourceDescriptor, IVMCollectionExpression<IViewModelExpression<D>>, D>(collectionSelector);
         return new DependencyTargetBuilder<TRootVM, IViewModel<D>, TRootDescriptor, D>(_context);
      }

      public IDependencyTargetBuilder<TRootVM, TSourceVM, TRootDescriptor, TSourceDescriptor> AndExecuteRefreshDependencies {
         get { 
            _context.ExecuteRefreshDependencies = true;
            return this;
         }
      }
   }
}