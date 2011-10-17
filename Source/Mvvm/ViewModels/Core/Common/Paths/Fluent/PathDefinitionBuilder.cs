namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Diagnostics.Contracts;
   using System.Linq;
   using Inspiring.Mvvm.Common;

   internal class PathDefinitionBuilder<TDescriptor> :
      IPathDefinitionBuilder<TDescriptor>
      where TDescriptor : IVMDescriptor {

      private readonly Reference<PathDefinition> _selectedPath;
      private readonly Reference<IPropertySelector[]> _selectedProperties;

      public PathDefinitionBuilder(
         Reference<PathDefinition> selectedPath,
         Reference<IPropertySelector[]> selectedProperties
      ) {
         Contract.Requires(selectedPath != null);
         Contract.Requires(selectedPath.Value != null);
         Contract.Requires(selectedProperties != null);

         _selectedPath = selectedPath;
         _selectedProperties = selectedProperties;
      }

      public IPathDefinitionBuilder<D> Descendant<D>(
         Func<TDescriptor, IVMPropertyDescriptor<IViewModel<D>>> viewModelSelector
      ) where D : IVMDescriptor {
         _selectedPath.Replace(current => current.Append(viewModelSelector));
         return new PathDefinitionBuilder<D>(_selectedPath, _selectedProperties);
      }

      public IPathDefinitionBuilder<D> Descendant<D>(
         Func<TDescriptor, IVMPropertyDescriptor<IVMCollectionExpression<IViewModelExpression<D>>>> collectionSelector
      ) where D : IVMDescriptor {
         _selectedPath.Replace(current => current.Append(collectionSelector));
         return new PathDefinitionBuilder<D>(_selectedPath, _selectedProperties);
      }

      public void Properties(params Func<TDescriptor, IVMPropertyDescriptor>[] propertySelectors) {
         _selectedProperties.Value = propertySelectors
            .Select(x => new PropertySelector<TDescriptor>(x))
            .ToArray();
      }
   }
}
