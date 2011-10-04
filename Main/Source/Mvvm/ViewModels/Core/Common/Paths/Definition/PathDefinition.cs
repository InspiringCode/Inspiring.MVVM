namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Collections.Generic;
   using System.Linq;

   internal sealed class PathDefinition {
      public static readonly PathDefinition Empty = new PathDefinition(new PathDefinitionStep[0]);

      private readonly PathDefinitionStep[] _steps;

      private PathDefinition(PathDefinitionStep[] steps) {
         _steps = steps;
      }

      public int Length {
         get { return _steps.Length; }
      }

      public bool IsEmpty {
         get { return _steps.Length == 0; }
      }

      public PathDefinition Append(PathDefinitionStep step) {
         return new PathDefinition(ArrayUtils.Append(_steps, step));
      }

      public PathDefinition Append<TDescriptor, TValue>(
         Func<TDescriptor, IVMPropertyDescriptor<TValue>> propertySelector
      ) where TDescriptor : IVMDescriptor {
         return Append(new PropertyStep<TDescriptor>(propertySelector));
      }

      public PathDefinition AppendCollection<TDescriptor, TItemVM>(
         Func<TDescriptor, IVMPropertyDescriptor<IVMCollectionExpression<TItemVM>>> collectionPropertySelector
      )
         where TDescriptor : IVMDescriptor
         where TItemVM : IViewModel {

         return Append(new CollectionStep<TDescriptor, TItemVM>(collectionPropertySelector));
      }

      public PathDefinition AppendCollectionProperty<TDescriptor, TValue>(
         Func<TDescriptor, IVMPropertyDescriptor<TValue>> propertySelector
      ) where TDescriptor : IVMDescriptor {
         return Append(new CollectionPropertyStep<TDescriptor, TValue>(propertySelector));
      }

      public PathMatch Matches(Path path) {
         var it = new PathDefinitionIterator(_steps);
         return it.MatchesNext(path.GetIterator());
      }

      public IViewModel[] GetDescendants(IViewModel rootVM) {
         var it = new PathDefinitionIterator(_steps);
         return it.GetDescendantNext(rootVM);
      }

      public override string ToString() {
         IEnumerable<PathDefinitionStep> steps = _steps;

         if (!_steps.Any()) {
            return "[empty]";
         }

         return String.Format(
            "[{0}]",
            String.Join(".", steps)
         );
      }
   }
}
