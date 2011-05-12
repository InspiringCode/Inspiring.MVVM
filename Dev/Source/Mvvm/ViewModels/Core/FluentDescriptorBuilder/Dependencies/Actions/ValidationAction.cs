namespace Inspiring.Mvvm.ViewModels.Core {
   using System.Collections.Generic;

   internal sealed class ValidationAction : DependencyAction {
      private readonly PathDefinition _targetPath;
      private readonly IList<IPropertySelector> _targetProperties;

      public ValidationAction(
         PathDefinition targetPath,
         IList<IPropertySelector> targetProperties
      ) {
         _targetPath = targetPath;
         _targetProperties = targetProperties;
      }

      internal PathDefinition TargetPath { get { return _targetPath; } }

      internal IList<IPropertySelector> TargetProperties {
         get { return _targetProperties; }
      }

      public override void Execute(IViewModel ownerVM, ChangeArgs args) {
         throw new System.NotImplementedException();
      }
   }
}
