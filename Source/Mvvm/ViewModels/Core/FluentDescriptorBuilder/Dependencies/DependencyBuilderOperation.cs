namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Collections.Generic;
   using System.Linq;

   internal sealed class DependencyBuilderOperation {
      private BehaviorChainConfiguration _viewModelConfiguration;
      private PathDefinition _sourcePath = PathDefinition.Empty;
      private PathDefinition _targetPath = PathDefinition.Empty;
      private bool _isProperlyTerminated;
      private PathDefinitionStep _terminatingAnyPropertyStep;
      private List<ChangeType> _changesTypes = new List<ChangeType>();
      private Func<DependencyAction> _actionCreator;
      private List<IPropertySelector> _targetProperties = new List<IPropertySelector>();

      public DependencyBuilderOperation(BehaviorChainConfiguration viewModelConfiguration) {
         _viewModelConfiguration = viewModelConfiguration;
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
         _sourcePath = _sourcePath.Append(descendatPropertySelector);
         _terminatingAnyPropertyStep = CreateAnyPropertyStep<TDescendantDescriptor>();
      }

      public void AddCollectionStep<TDescriptor, TDescendant, TDescendantDescriptor>(
         Func<TDescriptor, IVMPropertyDescriptor<IVMCollectionExpression<TDescendant>>> collectionPropertySelector,
         bool includeCollectionPopulated
      )
         where TDescriptor : IVMDescriptor
         where TDescendant : IViewModel
         where TDescendantDescriptor : IVMDescriptor {
         _sourcePath = _sourcePath.AppendCollection(collectionPropertySelector);
         _isProperlyTerminated = true;
         _changesTypes.Add(ChangeType.AddedToCollection);
         _changesTypes.Add(ChangeType.RemovedFromCollection);
         if (includeCollectionPopulated) {
            _changesTypes.Add(ChangeType.CollectionPopulated);
         }
      }

      public void AddProperties<TDescriptor>(
         params Func<TDescriptor, IVMPropertyDescriptor>[] propertySelector
      ) where TDescriptor : IVMDescriptor {
         _sourcePath = _sourcePath.Append(
            new OrStep(propertySelector.Select(x => new PropertyStep<TDescriptor>(x)).ToArray())
         );
         _isProperlyTerminated = true;
         _changesTypes.Add(ChangeType.PropertyChanged);
         _changesTypes.Add(ChangeType.ValidationResultChanged);
      }

      public void AddAnyStepsStep<TDescriptor>()
         where TDescriptor : IVMDescriptor {
         _sourcePath = _sourcePath.Append(new OptionalStep(new AnyStepsStep<TDescriptor>()));
         _isProperlyTerminated = true;
         AddAllChangeTypes();
      }

      public void AddDescendantTargetStep<TDescriptor, TDescendant, TDescendantDescriptor>(
         Func<TDescriptor, IVMPropertyDescriptor<TDescendant>> descendatPropertySelector
      )
         where TDescriptor : IVMDescriptor
         where TDescendantDescriptor : IVMDescriptor {
         _targetPath = _targetPath.Append(descendatPropertySelector);
      }

      public void AddTargetsProperties<TDescriptor>(
         params Func<TDescriptor, IVMPropertyDescriptor>[] propertySelectors
      ) where TDescriptor : IVMDescriptor {
         _targetProperties = propertySelectors
            .Select(p => new PropertySelector<TDescriptor>(p))
            .ToList<IPropertySelector>();
      }

      public void AddValidationAction() {
         _actionCreator = () => {
            return new ValidationAction(_targetPath, _targetProperties);
         };
      }

      public void AddRefreshAction() {
         _actionCreator = () => {
            return new RefreshAction(_targetPath, _targetProperties);
         };
      }

      public void AddExecuteAction<TOwnerVM>(
         Action<TOwnerVM, ChangeArgs> action
      ) where TOwnerVM : IViewModel {
         _actionCreator = () => {
            return new ExecuteAction<TOwnerVM>(action);
         };
      }

      public void Perform() {
         var dependency = CreateDependency();
         if (dependency == null) {
            return;
         }

         _viewModelConfiguration
            .ConfigureBehavior<DeclarativeDependencyBehavior>(
               ViewModelBehaviorKeys.DeclarativeDependencies,
               b => b.AddDependency(dependency)
          );
      }


      private PathDefinition GetSourcePath() {
         if (!_isProperlyTerminated) {
            _sourcePath = _sourcePath.Append(_terminatingAnyPropertyStep);
            _changesTypes.Add(ChangeType.PropertyChanged);
            _changesTypes.Add(ChangeType.ValidationResultChanged);
         }

         return _sourcePath;
      }

      private PathDefinitionStep CreateAnyPropertyStep<TDescriptor>()
         where TDescriptor : IVMDescriptor {
         return new OptionalStep(new AnyPropertyStep<TDescriptor>());
      }

      private DeclarativeDependency CreateDependency() {
         if (_actionCreator == null) {
            throw new ArgumentException(ExceptionTexts.IncompleteDependencySetupMissingAction);
         }
         var sourcePath = GetSourcePath();

         var action = _actionCreator();
         return new DeclarativeDependency(
            sourcePath,
            _changesTypes,
            _actionCreator()
         );
      }

      private void AddAllChangeTypes() {
         _changesTypes.Add(ChangeType.AddedToCollection);
         _changesTypes.Add(ChangeType.PropertyChanged);
         _changesTypes.Add(ChangeType.RemovedFromCollection);
         _changesTypes.Add(ChangeType.ValidationResultChanged);
      }
   }
}