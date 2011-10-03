namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Collections.Generic;
   using System.Linq;
   using Inspiring.Mvvm.ViewModels.Tracing;

   internal sealed class RefreshAction : DependencyAction {
      private readonly PathDefinition _targetPath;
      private readonly IList<IPropertySelector> _targetProperties;

      public RefreshAction(
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

      public override void Execute(
         IViewModel ownerVM,
         ChangeArgs args,
         DeclarativeDependency dependency
      ) {
         RefreshTrace.BeginRefresh(dependency);

         if (TargetPath.IsEmpty) {
            RefreshProperties(ownerVM);
         } else {
            var viewModels = TargetPath.GetDescendants(ownerVM);

            foreach (var viewModel in viewModels) {
               if (_targetProperties.Count > 0) {
                  RefreshProperties(viewModel);
               } else {
                  viewModel.Kernel.RefreshInternal();
               }
            }
         }

         RefreshTrace.EndLastRefresh();
      }

      public override string ToString() {
         var quotedProps = TargetProperties.Select(x => String.Format("'{0}'", x));

         return String.Format(
            "refresh path '{0}' and properties {1}",
            TargetPath,
            String.Join(", ", quotedProps)
         );
      }

      private void RefreshProperties(IViewModel ownerVM) {
         foreach (var propertySelector in _targetProperties) {
            var property = propertySelector.GetProperty(ownerVM.Descriptor);
            ownerVM.Kernel.RefreshInternal(property);
         }
      }
   }
}
