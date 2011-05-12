namespace Inspiring.Mvvm.ViewModels.Core {
   using System;

   internal sealed class DependencySourceBuilder<TRootVM, TSourceVM, TRootDescriptor, TSourceDescriptor> :
      IDependencySelfSourceBuilder<TRootVM, TSourceVM, TRootDescriptor, TSourceDescriptor>,
      IDependencySourceBuilder<TRootVM, TSourceVM, TRootDescriptor, TSourceDescriptor>,
      IDependencySourceOrAnyDescendantBuilder<TRootVM, TSourceVM, TRootDescriptor, TSourceDescriptor>,
      IDependencyActionBuilder<TRootVM, TRootDescriptor>,
      IActionOrAnyDescendantBuilder<TRootVM, TRootDescriptor>

      where TRootVM : IViewModel
      where TSourceVM : IViewModel
      where TRootDescriptor : IVMDescriptor
      where TSourceDescriptor : IVMDescriptor {

      private readonly DependencyBuilderOperation _context;

      public DependencySourceBuilder(DependencyBuilderOperation context) {
         _context = context;
      }

      public IActionOrAnyDescendantBuilder<TRootVM, TRootDescriptor> Self {
         get {
            _context.AddSelfStep<TRootDescriptor>();
            return this;
         }
      }

      public IDependencyActionBuilder<TRootVM, TRootDescriptor> Properties(
         params Func<TSourceDescriptor, IVMPropertyDescriptor>[] sourcePropertySelectors
      ) {
         _context.AddProperties(sourcePropertySelectors);
         return this;
      }

      public IDependencySourceOrAnyDescendantBuilder<TRootVM, IViewModel<D>, TRootDescriptor, D> Descendant<D>(
         Func<TSourceDescriptor, IVMPropertyDescriptor<IViewModel<D>>> viewModelSelector
      ) where D : IVMDescriptor {
         _context.AddDescendantStep<TSourceDescriptor, IViewModel<D>, D>(viewModelSelector);
         return new DependencySourceBuilder<TRootVM, IViewModel<D>, TRootDescriptor, D>(_context);
      }

      public IDependencySourceOrAnyDescendantBuilder<TRootVM, IViewModel<D>, TRootDescriptor, D> Descendant<D>(
         Func<TSourceDescriptor, IVMPropertyDescriptor<IVMCollectionExpression<IViewModelExpression<D>>>> collectionSelector
      ) where D : IVMDescriptor {
         _context.AddDescendantStep<TSourceDescriptor, IVMCollectionExpression<IViewModelExpression<D>>, D>(
            collectionSelector
         );
         return new DependencySourceBuilder<TRootVM, IViewModel<D>, TRootDescriptor, D>(_context);
      }

      public IDependencyActionBuilder<TRootVM, TRootDescriptor> Collection<D>(
         Func<TSourceDescriptor, IVMPropertyDescriptor<IVMCollectionExpression<IViewModelExpression<D>>>> collectionSelector
      ) where D : IVMDescriptor {
         _context.AddCollectionStep<TSourceDescriptor, IViewModelExpression<D>, D>(
            collectionSelector
         );
         return new DependencySourceBuilder<TRootVM, IViewModel<D>, TRootDescriptor, D>(_context);
      }

      public IDependencyActionBuilder<TRootVM, TRootDescriptor> OrAnyDescendant {
         get {
            _context.AddAnyStepsStep<TSourceDescriptor>();
            return this;
         }
      }

      //
      //   A C T I O N S
      //

      public IDependencyTargetBuilder<TRootVM, TRootVM, TRootDescriptor, TRootDescriptor> Refresh {
         get {
            _context.AddRefreshAction();
            return new DependencyTargetBuilder<TRootVM, TRootVM, TRootDescriptor, TRootDescriptor>(_context);
         }
      }

      public IDependencyTargetBuilder<TRootVM, TRootVM, TRootDescriptor, TRootDescriptor> Revalidate {
         get {
            _context.AddValidationAction();
            return new DependencyTargetBuilder<TRootVM, TRootVM, TRootDescriptor, TRootDescriptor>(_context);
         }
      }

      public void Execute(Action<TRootVM, ChangeArgs> changeAction) {
         _context.AddExecuteAction<TRootVM>(changeAction);
      }
   }
}