namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Collections.Generic;
   using System.Linq;

   internal interface IPathDefinitionBuilderContext {
      QualifiedProperties GetPath();
   }

   internal class PathDefinitionBuilderContext : IPathDefinitionBuilderContext {
      private readonly List<QualifiedProperties> _paths = new List<QualifiedProperties>();

      public IEnumerable<QualifiedProperties> Paths {
         get { return _paths; }
      }

      public QualifiedProperties GetPath() {
         var path = new QualifiedProperties();
         _paths.Add(path);
         return path;
      }
   }

   internal class PathDefinitionBuilder<TDescriptor> :
      IPathDefinitionBuilder<TDescriptor>
      where TDescriptor : IVMDescriptor {

      private readonly IPathDefinitionBuilderContext _context;

      public PathDefinitionBuilder(IPathDefinitionBuilderContext context) {
         Check.NotNull(context, nameof(context));
         _context = context;
      }

      public PathDefinitionBuilder(QualifiedProperties path) {
         Check.NotNull(path, nameof(path));
         _context = new NestedContext(path);
      }

      public IPathDefinitionBuilder<D> Descendant<D>(
         Func<TDescriptor, IVMPropertyDescriptor<IViewModel<D>>> viewModelSelector
      ) where D : IVMDescriptor {
         var path = _context.GetPath();
         path.Path = path.Path.Append(viewModelSelector);
         return new PathDefinitionBuilder<D>(path);
      }

      public IPathDefinitionBuilder<D> Descendant<D>(
         Func<TDescriptor, IVMPropertyDescriptor<IVMCollectionExpression<IViewModelExpression<D>>>> collectionSelector
      ) where D : IVMDescriptor {
         var path = _context.GetPath();
         path.Path = path.Path.Append(collectionSelector);
         return new PathDefinitionBuilder<D>(path);
      }

      public void Properties(params Func<TDescriptor, IVMPropertyDescriptor>[] propertySelectors) {
         var path = _context.GetPath();
         path.Properties = propertySelectors
            .Select(x => new PropertySelector<TDescriptor>(x))
            .ToArray();
      }

      private class NestedContext : IPathDefinitionBuilderContext {
         private readonly QualifiedProperties _path;

         public NestedContext(QualifiedProperties path) {
            _path = path;
         }

         public QualifiedProperties GetPath() {
            return _path;
         }
      }
   }
}
