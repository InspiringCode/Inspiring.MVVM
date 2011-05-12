using System.Collections.Generic;
using System.Collections.ObjectModel;
namespace Inspiring.Mvvm.ViewModels.Core {

   internal sealed class DeclarativeDependency {
      private readonly PathDefinition _sourcePath;
      private readonly DependencyAction _action;
      private readonly List<ChangeType> _changeTypes;

      public DeclarativeDependency(
         PathDefinition sourcePath,
         List<ChangeType> changeTypes,
         DependencyAction action
      ) {
         _sourcePath = sourcePath;
         _changeTypes = changeTypes;
         _action = action;
      }

      internal PathDefinition SourcePath {
         get { return _sourcePath; }
      }

      internal ReadOnlyCollection<ChangeType> ChangeTypes {
         get { return _changeTypes.AsReadOnly(); }
      }

      internal DependencyAction Action {
         get { return _action; }
      }

      public void HandleChange(IViewModel ownerVM, ChangeArgs args) {
         bool isExpectedChangeType = _changeTypes.Contains(args.ChangeType);
         if (!isExpectedChangeType) {
            return;
         }

         PathMatch sourcePathMatch = _sourcePath.Matches(args.ChangedPath);
         if (sourcePathMatch.Success) {
            _action.Execute(ownerVM, args);
         }
      }
   }
}