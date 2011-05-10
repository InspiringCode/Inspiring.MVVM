using System;
namespace Inspiring.Mvvm.ViewModels.Core {

   internal sealed class VMDependencyBuilder<TVM, TDescriptor>
      : IVMDependencyBuilder<TVM, TDescriptor>
      where TVM : IViewModel
      where TDescriptor : VMDescriptor {

      private BehaviorChainConfiguration _viewModelConfiguration;

      public VMDependencyBuilder(BehaviorChainConfiguration viewModelConfiguration) {
         _viewModelConfiguration = viewModelConfiguration;
      }

      public IDependencySelfSourceBuilder<TVM, TVM, TDescriptor, TDescriptor> OnChangeOf {
         get { return new SourceDependencyBuilder<TVM, TVM, TDescriptor, TDescriptor>(); }
      }
   }

   internal sealed class SourceDependencyBuilder<TRootVM, TSourceVM, TRootDescriptor, TSourceDescriptor> :
      IDependencySelfSourceBuilder<TRootVM, TSourceVM, TRootDescriptor, TSourceDescriptor>
      where TRootVM : IViewModel
      where TSourceVM : IViewModel
      where TRootDescriptor : IVMDescriptor
      where TSourceDescriptor : IVMDescriptor {

      public IActionOrAnyDescendantBuilder<TRootVM, TRootDescriptor> Self {
         get { throw new System.NotImplementedException(); }
      }

      public IDependencyActionBuilder<TRootVM, TRootDescriptor> Properties(params Func<TSourceDescriptor, IVMPropertyDescriptor>[] sourcePropertySelectors) {
         throw new System.NotImplementedException();
      }

      public IDependencySourceOrAnyDescendantBuilder<TRootVM, IViewModel<D>, TRootDescriptor, D> Descendant<D>(System.Func<TSourceDescriptor, IVMPropertyDescriptor<IViewModel<D>>> viewModelSelector) where D : IVMDescriptor {
         throw new System.NotImplementedException();
      }

      public IDependencySourceOrAnyDescendantBuilder<TRootVM, IViewModel<D>, TRootDescriptor, D> Descendant<D>(System.Func<TSourceDescriptor, IVMPropertyDescriptor<IVMCollectionExpression<IViewModelExpression<D>>>> collectionSelector) where D : IVMDescriptor {
         throw new System.NotImplementedException();
      }

      public IDependencyActionBuilder<TRootVM, TRootDescriptor> Collection<D>(Func<TSourceDescriptor, IVMPropertyDescriptor<IVMCollectionExpression<IViewModelExpression<D>>>> collectionSelector) where D : IVMDescriptor {
         throw new System.NotImplementedException();
      }
   }

   class DependencyBuilderContext {
      private PathDefinition _path = PathDefinition.Empty;
      private bool _isProperlyTerminated;
      private PathDefinitionStep _terminatingAnyPropertyStep;


      public DependencyBuilderContext() {

      }

      public void AddSelfStep<TRootDescriptor>()
         where TRootDescriptor : IVMDescriptor {
         _terminatingAnyPropertyStep = CreateAnyPropertyStep<TRootDescriptor>();
      }

      public void AddDescendantStep<TDescriptor, TDescendant, TDescendantDescriptor>(
         Func<TDescriptor, IVMPropertyDescriptor<TDescendant>> descendatPropertySelector
      )
         where TDescriptor : IVMDescriptor
         where TDescendantDescriptor : IVMDescriptor {
         _path = _path.Append(descendatPropertySelector);
         _terminatingAnyPropertyStep = CreateAnyPropertyStep<TDescendantDescriptor>();
      }

      public void AddStep<TDescriptor>(
         Func<TDescriptor, IVMPropertyDescriptor> propertySelector
      ) where TDescriptor : IVMDescriptor {
         //_path = _path.Append(propertySelector);
      }

      public void AddAnyStepsStep<TDescriptor>()
         where TDescriptor : IVMDescriptor {
         _path = _path.Append(new OptionalStep(new AnyStepsStep<TDescriptor>()));
         _isProperlyTerminated = true;
      }

      public PathDefinition GetPathDefinition() {
         if (!_isProperlyTerminated) {
            _path = _path.Append(_terminatingAnyPropertyStep);
         }
         return _path;
      }

      private PathDefinitionStep CreateAnyPropertyStep<TDescriptor>()
         where TDescriptor : IVMDescriptor {
         return new OptionalStep(new AnyPropertyStep<TDescriptor>());
      }
   }
}
